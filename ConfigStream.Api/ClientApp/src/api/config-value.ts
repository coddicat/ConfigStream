import { ConfigValue, SubmitConfigValue } from '@/store/config-value';
import { instance } from './instance';

export async function getConfigValues(environments: string[], search?: string) {
  const response = await instance.get<ConfigValue[]>('/value', {
    params: {
      environments,
      search
    },
    paramsSerializer: {
      indexes: null
    }
  });
  return response.data;
}

export function submitConfigValue(configValue: SubmitConfigValue) {
  return instance.put('/value', configValue);
}
