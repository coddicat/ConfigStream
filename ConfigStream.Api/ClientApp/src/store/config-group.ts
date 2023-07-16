import {
  createConfigGroup,
  getConfigGroups,
  deleteConfigGroup
} from '@/api/config-group';
import { defineStore } from 'pinia';

export type ConfigGroup = {
  name: string;
};

type Store = {
  items: ConfigGroup[];
  loading: boolean;
  search?: string;
};

export const useConfigGroupStore = defineStore('configGroup', {
  state: (): Store => ({
    items: [],
    loading: false,
    search: ''
  }),
  actions: {
    async createConfigGroup(group: ConfigGroup) {
      try {
        this.loading = true;
        await createConfigGroup(group);
        await this.requestConfigGroups();
      } catch (error) {
        console.error(error);
      } finally {
        this.loading = false;
      }
    },
    async deleteConfigGroup(group: ConfigGroup) {
      try {
        this.loading = true;
        await deleteConfigGroup(group);
        await this.requestConfigGroups();
      } catch (error) {
        console.error(error);
      } finally {
        this.loading = false;
      }
    },
    async requestConfigGroups() {
      try {
        this.loading = true;
        const response = await getConfigGroups(this.search);
        this.items = response;
      } catch (error) {
        console.error(error);
      } finally {
        this.loading = false;
      }
    }
  }
});
