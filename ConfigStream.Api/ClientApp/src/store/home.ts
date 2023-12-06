import { defineStore } from 'pinia';
import type { ConfigValue } from './config-value';
import { sort } from '@/utils/array';
import { useConfigStore, type Config } from './config';

type EditConfigValue = {
  groupName: string;
  configName: string;
  targetName?: string;
};

type Store = {
  selectedEnvironments: string[];
  environments: string[];
  editingConfigValues: Record<string, string>;
  expandedNodes: Record<string, boolean>;
  setupConfigDialog: boolean | Config;
};

const configStore = useConfigStore();
export const useHomeStore = defineStore('home', {
  state: (): Store => ({
    environments: [],
    selectedEnvironments: [],
    editingConfigValues: {},
    expandedNodes: {},
    setupConfigDialog: false
  }),
  actions: {
    openSetupConfigDialog(config?: Config) {
      this.setupConfigDialog = config ?? true;
    },
    closeSetupConfigDialog() {
      this.setupConfigDialog = false;
    },
    setEnvironmentsFromConfigValues(values: ConfigValue[]) {
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
    isEditing:
      state =>
      (data: EditConfigValue): boolean => {
        const key = `${data.groupName}:${data.configName}:${data.targetName}`;
        return Object.getOwnPropertyNames(state.editingConfigValues).includes(
          key
        );
      },
    disabledEnvironments: state =>
      configStore.configs.length === 0 ||
      Object.getOwnPropertyNames(state.editingConfigValues).length > 0,
    sortedEnvironments: state => sort(state.environments, x => x),
    sortedSelectedEnvironments: state =>
      sort(state.selectedEnvironments, x => x)
  }
});
