import {
  createConfigTarget,
  getConfigTargets,
  deleteConfigTarget
} from '@/api/config-target';
import { defineStore } from 'pinia';

export type ConfigTarget = {
  name: string;
};

type Store = {
  items: ConfigTarget[];
  loading: boolean;
  search?: string;
};

export const useConfigTargetStore = defineStore('configTarget', {
  state: (): Store => ({
    items: [],
    loading: false,
    search: ''
  }),
  actions: {
    async createConfigTarget(target: ConfigTarget) {
      try {
        this.loading = true;
        await createConfigTarget(target);
        await this.requestConfigTargets();
      } catch (error) {
        console.error(error);
      } finally {
        this.loading = false;
      }
    },
    async deleteConfigTarget(target: ConfigTarget) {
      try {
        this.loading = true;
        await deleteConfigTarget(target);
        await this.requestConfigTargets();
      } catch (error) {
        console.error(error);
      } finally {
        this.loading = false;
      }
    },
    async requestConfigTargets() {
      try {
        this.loading = true;
        const response = await getConfigTargets(this.search);
        this.items = response;
      } catch (error) {
        console.error(error);
      } finally {
        this.loading = false;
      }
    }
  }
});
