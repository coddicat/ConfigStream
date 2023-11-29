import { createOrUpdateConfig, deleteConfig, getConfigs } from '@/api/config';
import { defineStore } from 'pinia';
import { type DebouncedFunc, debounce } from 'lodash';

export type Config = {
  name: string;
  groupName: string;
  description?: string;
  allowedValues?: string[];
  defaultValue?: string;
};
// export type ConfigFormDialog = {
//   open: boolean;
//   loading: boolean;
//   config?: Config;
//   new?: boolean;
// };

type Store = {
  items: Config[];
  loading: boolean;
  // search?: string;
  // formDialog: ConfigFormDialog;
};

let debouncedRequestConfigList: undefined | DebouncedFunc<() => Promise<void>>;

export const useConfigStore = defineStore('config', {
  state: (): Store => ({
    items: [],
    loading: false
    // formDialog: {
    //   open: false,
    //   loading: false
    // },
    // search: undefined
  }),
  actions: {
    debouncedRequestConfigList() {
      if (!debouncedRequestConfigList) {
        debouncedRequestConfigList = debounce(this.requestConfigList, 300);
      }
      debouncedRequestConfigList();
    },
    async requestConfigList() {
      try {
        this.loading = true;
        const items = await getConfigs(/*this.search*/);
        this.items = items;
      } catch (error) {
        console.error(error);
      } finally {
        this.loading = false;
      }
    },
    // updateSearch(v?: string) {
    //   this.search = v;
    //   this.debouncedRequestConfigList();
    // },
    // openEditConfigDialog(config: Config) {
    //   this.formDialog = {
    //     config,
    //     open: true,
    //     loading: false
    //   };
    // },
    // openCreateConfigDialog() {
    //   this.formDialog = {
    //     new: true,
    //     open: true,
    //     loading: false
    //   };
    // },
    // closeDialog() {
    //   this.formDialog.open = false;
    // },
    async createOrUpdateConfig(config: Config) {
      try {
        // this.formDialog.loading = true;
        await createOrUpdateConfig(config);
        // this.formDialog.open = false;
        this.requestConfigList();
      } catch (error) {
        console.error(error);
      } finally {
        // this.formDialog.loading = false;
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
