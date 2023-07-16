import { ConfigValue } from '@/store/config-value';
import { instance } from './instance';

export async function getConfigValues(search?: string) {
  const environment = 'Development';
  const response = await instance.get<ConfigValue[]>('/value', {
    params: {
      environment,
      search
    }
  });
  return response.data;
}

// export function createOrUpdateConfig(config: Config) {
//   return instance.put('/config', config);
// }

// export function deleteConfig(config: Config) {
//   return instance.delete(`/config/${config.groupName}/${config.name}`);
// }
