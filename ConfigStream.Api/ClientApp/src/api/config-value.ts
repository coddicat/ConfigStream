import { type ConfigValue } from '@/store/config-value';
import { instance } from './instance';

export async function getConfigValues(): Promise<ConfigValue[]> {
  const response = await instance.get<ConfigValue[]>('/value', {
    paramsSerializer: {
      indexes: null
    }
  });
  return response.data;
}

export function submitConfigValues(configValues: ConfigValue[]): Promise<void> {
  return instance.put('/value', configValues);
}
