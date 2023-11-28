import { type ConfigValue } from '@/store/config-value';
import { instance } from './instance';

export async function getConfigValues(/*search?: string*/): Promise<
  ConfigValue[]
> {
  const response = await instance.get<ConfigValue[]>('/value', {
    // params: {
    //   search
    // },
    paramsSerializer: {
      indexes: null
    }
  });
  return response.data;
}

export function submitConfigValues(configValues: ConfigValue[]): Promise<void> {
  return instance.put('/value', configValues);
}
