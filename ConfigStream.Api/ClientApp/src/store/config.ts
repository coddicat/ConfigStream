import { createOrUpdateConfig, deleteConfig, getConfigs } from '@/api/config';
import { defineStore } from 'pinia';
import type { ToastServiceMethods } from 'primevue/toastservice';

export type Config = {
  configName: string;
  groupName: string;
  description?: string;
  allowedValues?: string[];
  defaultValue?: string;
  deleted?: boolean;
};

type Store = {
  _toast?: ToastServiceMethods;
  configs: Config[];
  loading: boolean;
};

export const useConfigStore = defineStore('config', {
  state: (): Store => ({
    configs: [],
    loading: false
  }),
  actions: {
    initToast(toast: ToastServiceMethods) {
      this._toast = toast;
    },
    async requestConfigList() {
      try {
        this.loading = true;
        this.configs = await getConfigs();
      } catch (error) {
        this._toast?.add({
          severity: 'error',
          summary: 'Failed to load configs',
          detail: error,
          closable: true
        });
      } finally {
        this.loading = false;
      }
    },
    async createOrUpdateConfig(config: Config) {
      try {
        this.loading = true;
        await createOrUpdateConfig(config);
        await this.requestConfigList();
        this._toast?.add({
          severity: 'success',
          detail: 'The config was successfully created or updated',
          life: 3000
        });
      } catch (error) {
        this._toast?.add({
          severity: 'error',
          summary: 'Failed to create or update config',
          detail: error,
          closable: true
        });
      } finally {
        this.loading = false;
      }
    },
    async deleteConfig(config: Config) {
      try {
        this.loading = true;
        await deleteConfig(config);
        await this.requestConfigList();
        this._toast?.add({
          severity: 'success',
          detail: 'The config was successfully deleted',
          life: 3000
        });
      } catch (error) {
        this._toast?.add({
          severity: 'error',
          summary: 'Failed to delete config',
          detail: error,
          closable: true
        });
      } finally {
        this.loading = false;
      }
    }
  }
});
