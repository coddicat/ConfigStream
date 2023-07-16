import {
  /* createOrUpdateConfig, deleteConfig, */ getConfigValues
} from '@/api/config-value';
import { defineStore } from 'pinia';
import { DebouncedFunc, debounce } from 'lodash';
import { SortItem } from '@/type';

export type ConfigValue = {
  configName: string;
  groupName: string;
  defaultValue?: string;
  value?: string;
};
// export type ConfigFormDialog = {
//   open: boolean;
//   loading: boolean;
//   config?: Config;
//   new?: boolean;
// };

type Store = {
  items: ConfigValue[];
  total: number;
  page: number;
  itemsPerPage: number;
  loading: boolean;
  search?: string;
  sortBy?: SortItem;

  // formDialog: ConfigFormDialog;
};

let debouncedRequestConfigValueList:
  | undefined
  | DebouncedFunc<() => Promise<void>>;

export const useConfigValueStore = defineStore('configValue', {
  state: (): Store => ({
    items: [],
    total: 0,
    page: 1,
    itemsPerPage: 10,
    loading: false,
    // formDialog: {
    //   open: false,
    //   loading: false
    // },
    search: undefined,
    sortBy: undefined
  }),
  actions: {
    debouncedRequestConfigList() {
      if (!debouncedRequestConfigValueList) {
        debouncedRequestConfigValueList = debounce(
          this.requestConfigValueList,
          300
        );
      }
      debouncedRequestConfigValueList();
    },
    async requestConfigValueList() {
      try {
        this.loading = true;
        const items = await getConfigValues(this.search);
        this.items = items;
        this.total = items.length;
      } catch (error) {
        console.error(error);
      } finally {
        this.loading = false;
      }
    },
    updateItemsPerPage(v: number) {
      this.itemsPerPage = v;
      this.debouncedRequestConfigList();
    },
    updatePage(v: number) {
      this.page = v;
      this.debouncedRequestConfigList();
    },
    updateSearch(v?: string) {
      this.search = v;
      this.debouncedRequestConfigList();
    },
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
    async setConfigValue(configValue: ConfigValue, value: string) {
      // try {
      //   this.formDialog.loading = true;
      //   if (createGroup) {
      //     await createConfigGroup({
      //       name: config.groupName
      //     });
      //   }
      //   await createOrUpdateConfig(config);
      //   this.formDialog.open = false;
      //   this.requestConfigList();
      // } catch (error) {
      //   console.error(error);
      // } finally {
      //   this.formDialog.loading = false;
      // }
    },
    async resetConfigValue(configValue: ConfigValue) {
      // try {
      //   this.loading = true;
      //   await deleteConfig(config);
      //   this.requestConfigList();
      // } catch (error) {
      //   console.error(error);
      // } finally {
      //   this.loading = false;
      // }
    }
  }
});
