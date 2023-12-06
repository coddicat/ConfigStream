import { createOrUpdateConfig, deleteConfig, getConfigs } from '@/api/config';
import { defineStore } from 'pinia';

export type Config = {
  name: string;
  groupName: string;
  description?: string;
  allowedValues?: string[];
  defaultValue?: string;
};

type Store = {
  configs: Config[];
  loading: boolean;
};

export const useConfigStore = defineStore('config', {
  state: (): Store => ({
    configs: [],
    loading: false
  }),
  actions: {
    async requestConfigList() {
      try {
        this.loading = true;
        this.configs = await getConfigs();
      } catch (error) {
        console.error(error);
      } finally {
        this.loading = false;
      }
    },
    async createOrUpdateConfig(config: Config) {
      try {
        await createOrUpdateConfig(config);
        this.requestConfigList();
      } catch (error) {
        console.error(error);
      } finally {
        //ingore
      }
    },
    async deleteConfig(config: Config) {
      try {
        this.loading = true;
        await deleteConfig(config);
        this.requestConfigList();
      } catch (error) {
        console.error(error);
      } finally {
        this.loading = false;
      }
    }
  }
});
