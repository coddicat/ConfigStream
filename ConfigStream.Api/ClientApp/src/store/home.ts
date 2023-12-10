import { defineStore } from 'pinia';
import { useConfigValueStore } from './config-value';
import { sort } from '@/utils/array';
import { useConfigStore, type Config } from './config';
import type { ToastServiceMethods } from 'primevue/toastservice';

type EditConfigValue = {
  groupName: string;
  configName: string;
  targetName?: string;
};

type Store = {
  _toast?: ToastServiceMethods;
  _loading: boolean;
  selectedEnvironments: string[];
  environments: string[];
  editingConfigValues: Record<string, string>;
  expandedNodes: Record<string, boolean>;
  setupConfigDialog: boolean | Config;
};

export const useHomeStore = defineStore('home', {
  state: (): Store => ({
    _loading: false,
    _toast: undefined,
    environments: [],
    selectedEnvironments: [],
    editingConfigValues: {},
    expandedNodes: {},
    setupConfigDialog: false
  }),
  actions: {
    initToast(toast: ToastServiceMethods) {
      this._toast = toast;
      useConfigStore().initToast(toast);
      useConfigValueStore().initToast(toast);
    },
    async load() {
      try {
        this._loading = true;
        const configTask = useConfigStore().requestConfigList();
        const valueTask = useConfigValueStore().requestConfigValueList();
        await Promise.all([configTask, valueTask]);
        this.setEnvironmentsFromConfigValues();
      } catch (error) {
        this._toast?.add({
          severity: 'error',
          summary: 'Failed to load data',
          detail: error,
          closable: true
        });
      } finally {
        this._loading = false;
      }
    },
    openSetupConfigDialog(config?: Config) {
      this.setupConfigDialog = config ?? true;
    },
    closeSetupConfigDialog() {
      this.setupConfigDialog = false;
    },
    setEnvironmentsFromConfigValues() {
      const values = useConfigValueStore().configValues;
      this.environments = Array.from(
        new Set(values.map(value => value.environmentName))
      );
      this.selectedEnvironments =
        this.sortedEnvironments.length > 0 ? [this.sortedEnvironments[0]] : [];
    },
    addEnvironment(name: string) {
      if (
        this.environments.map(x => x.toLowerCase()).includes(name.toLowerCase())
      ) {
        return;
      }
      this.environments.push(name);
      this.selectedEnvironments.push(name);
    },
    editConfigValues(data: EditConfigValue, originalValuesJson: string) {
      const key = `${data.groupName}:${data.configName}:${data.targetName}`;
      this.editingConfigValues[key] = originalValuesJson;
    },
    cancelEditConfigValues(data: EditConfigValue): string | undefined {
      const key = `${data.groupName}:${data.configName}:${data.targetName}`;
      const originValuesJson = this.editingConfigValues[key];
      if (!originValuesJson) return undefined;

      delete this.editingConfigValues[key];
      return originValuesJson;
    },
    expand(groupName: string, configName?: string) {
      this.expandedNodes[groupName] = true;
      if (!configName) return;
      this.expandedNodes[`${groupName}:${configName}`] = true;
    }
  },
  getters: {
    version: () => {
      const v = import.meta.env.VITE_APP_VERSION;
      return v;
    },
    loading: store =>
      store._loading ||
      useConfigValueStore().loading ||
      useConfigStore().loading,
    isEditing:
      state =>
      (data: EditConfigValue): boolean => {
        const key = `${data.groupName}:${data.configName}:${data.targetName}`;
        return Object.getOwnPropertyNames(state.editingConfigValues).includes(
          key
        );
      },
    disabledEnvironments: state =>
      useConfigStore().configs.length === 0 ||
      Object.getOwnPropertyNames(state.editingConfigValues).length > 0,
    sortedEnvironments: state => sort(state.environments, x => x),
    sortedSelectedEnvironments: state =>
      sort(state.selectedEnvironments, x => x)
  }
});
