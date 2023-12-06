import { getConfigValues, submitConfigValues } from '@/api/config-value';
import { defineStore } from 'pinia';

export type ConfigValue = {
  configName: string;
  groupName: string;
  environmentName: string;
  targetName?: string;
  value?: string;
};

type Store = {
  configValues: ConfigValue[];
};

export const useConfigValueStore = defineStore('configValue', {
  state: (): Store => ({
    configValues: []
  }),
  actions: {
    async requestConfigValueList() {
      try {
        this.configValues = await getConfigValues();
      } catch (error) {
        console.error(error);
      } finally {
        //
      }
    },
    async submitConfigValues(configValues: ConfigValue[]) {
      try {
        await submitConfigValues(configValues);
      } catch (error) {
        console.error(error);
      } finally {
        // ignore
      }
    },
    addConfigValue(configValue: ConfigValue) {
      this.configValues.push(configValue);
    }
  }
});
