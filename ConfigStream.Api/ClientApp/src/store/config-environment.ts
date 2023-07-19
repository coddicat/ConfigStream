import {
  createConfigEnvironment,
  getConfigEnvironments,
  deleteConfigEnvironment
} from '@/api/config-environment';
import { defineStore } from 'pinia';

export type ConfigEnvironment = {
  name: string;
};

type Store = {
  items: ConfigEnvironment[];
  loading: boolean;
  search?: string;
};

export const useConfigEnvironmentStore = defineStore('configEnvironment', {
  state: (): Store => ({
    items: [],
    loading: false,
    search: ''
  }),
  actions: {
    async createConfigEnvironment(environment: ConfigEnvironment) {
      try {
        this.loading = true;
        await createConfigEnvironment(environment);
        await this.requestConfigEnvironments();
      } catch (error) {
        console.error(error);
      } finally {
        this.loading = false;
      }
    },
    async deleteConfigEnvironment(environment: ConfigEnvironment) {
      try {
        this.loading = true;
        await deleteConfigEnvironment(environment);
        await this.requestConfigEnvironments();
      } catch (error) {
        console.error(error);
      } finally {
        this.loading = false;
      }
    },
    async requestConfigEnvironments() {
      try {
        this.loading = true;
        const response = await getConfigEnvironments(this.search);
        this.items = response;
      } catch (error) {
        console.error(error);
      } finally {
        this.loading = false;
      }
    }
  }
});
