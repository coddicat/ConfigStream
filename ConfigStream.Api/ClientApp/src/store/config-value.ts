import { getConfigValues, submitConfigValues } from '@/api/config-value';
import { defineStore } from 'pinia';
import type { ToastServiceMethods } from 'primevue/toastservice';

export type ConfigValue = {
  configName: string;
  groupName: string;
  environmentName: string;
  targetName?: string;
  value?: string;
};

type Store = {
  _toast?: ToastServiceMethods;
  configValues: ConfigValue[];
  loading: boolean;
};

export const useConfigValueStore = defineStore('configValue', {
  state: (): Store => ({
    configValues: [],
    loading: false
  }),
  actions: {
    initToast(toast: ToastServiceMethods) {
      this._toast = toast;
    },
    async requestConfigValueList() {
      try {
        this.loading = true;
        this.configValues = await getConfigValues();
      } catch (error) {
        this._toast?.add({
          severity: 'error',
          summary: 'Failed to load configs values',
          detail: error,
          closable: true
        });
      } finally {
        this.loading = false;
      }
    },
    async submitConfigValues(configValues: ConfigValue[]) {
      try {
        this.loading = true;
        await submitConfigValues(configValues);
        this._toast?.add({
          severity: 'success',
          detail: 'The config values were successfully saved',
          life: 3000
        });
      } catch (error) {
        this._toast?.add({
          severity: 'error',
          summary: 'Failed to save config values',
          detail: error,
          closable: true
        });
      } finally {
        this.loading = false;
      }
    },
    addConfigValue(configValue: ConfigValue) {
      this.configValues.push(configValue);
    }
  }
});
