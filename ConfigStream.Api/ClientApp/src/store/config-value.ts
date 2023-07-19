import { getConfigValues, submitConfigValue } from '@/api/config-value';
import { defineStore } from 'pinia';
import { DebouncedFunc, debounce } from 'lodash';
import { SortItem } from '@/type';

export type SubmitConfigValue = {
  configName: string;
  groupName: string;
  environmentValues: Record<string, string>;
};
export type ConfigValue = {
  configName: string;
  groupName: string;
  defaultValue?: string;
  allowedValues: string[];
  environmentValues: Record<string, string>;
};
export type ConfigValueFormDialog = {
  open: boolean;
  loading: boolean;
  configValue?: ConfigValue;
};

type Store = {
  items: ConfigValue[];
  total: number;
  page: number;
  itemsPerPage: number;
  loading: boolean;
  search?: string;
  sortBy?: SortItem;
  selectedEnvironments: string[];
  formDialog: ConfigValueFormDialog;
};

let debouncedRequestConfigValueList:
  | undefined
  | DebouncedFunc<() => Promise<void>>;

export const useConfigValueStore = defineStore('configValue', {
  state: (): Store => ({
    selectedEnvironments: [],
    items: [],
    total: 0,
    page: 1,
    itemsPerPage: 10,
    loading: false,
    formDialog: {
      open: false,
      loading: false
    },
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
        const items = await getConfigValues(
          this.selectedEnvironments,
          this.search
        );
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
    openEditConfigDialog(configValue: ConfigValue) {
      this.formDialog = {
        configValue,
        open: true,
        loading: false
      };
    },
    closeDialog() {
      this.formDialog.open = false;
    },
    async submitConfigValue(configValue: SubmitConfigValue) {
      try {
        this.formDialog.loading = true;
        await submitConfigValue(configValue);
        this.formDialog.open = false;
        this.requestConfigValueList();
      } catch (error) {
        console.error(error);
      } finally {
        this.formDialog.loading = false;
      }
    }
  }
});
