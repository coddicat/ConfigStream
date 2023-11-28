import { getConfigValues, submitConfigValues } from '@/api/config-value';
import { defineStore } from 'pinia';
import { type DebouncedFunc, debounce } from 'lodash';

export type ConfigValue = {
  configName: string;
  groupName: string;
  environmentName: string;
  targetName?: string;
  value?: string;
};

// export type ConfigValueFormDialog = {
//   open: boolean;
//   loading: boolean;
//   configValue?: ConfigValue;
// };

type Store = {
  items: ConfigValue[];
  loading: boolean;
  // search?: string;
  selectedEnvironments: string[];
  // formDialog: ConfigValueFormDialog;
};

let debouncedRequestConfigValueList:
  | undefined
  | DebouncedFunc<() => Promise<void>>;

export const useConfigValueStore = defineStore('configValue', {
  state: (): Store => ({
    selectedEnvironments: [],
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
        const items = await getConfigValues(/*this.search*/);
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
    // openEditConfigDialog(configValue: ConfigValue) {
    //   this.formDialog = {
    //     configValue,
    //     open: true,
    //     loading: false
    //   };
    // },
    // closeDialog() {
    //   this.formDialog.open = false;
    // },
    async submitConfigValues(configValues: ConfigValue[]) {
      try {
        // this.formDialog.loading = true;
        await submitConfigValues(configValues);
        // this.formDialog.open = false;
        // this.requestConfigValueList();
      } catch (error) {
        console.error(error);
      } finally {
        // this.formDialog.loading = false;
      }
    }
  }
});
