import { type Config } from '@/store/config';
import { instance } from './instance';

export async function getConfigs(/*search?: string*/) {
  const response = await instance.get<Config[]>('/config', {
    //params: { search }
  });
  return response.data;
}

export function createOrUpdateConfig(config: Config) {
  return instance.put('/config', config);
}

export function deleteConfig(config: Config) {
  return instance.delete(`/config/${config.groupName}/${config.name}`);
}
