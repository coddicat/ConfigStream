import { ConfigEnvironment } from '@/store/config-environment';
import { instance } from './instance';

export async function getConfigEnvironments(search?: string) {
  const response = await instance.get<ConfigEnvironment[]>(`/environment`, {
    params: { search }
  });
  return response.data;
}

export function createConfigEnvironment(configEnvironment: ConfigEnvironment) {
  return instance.put('/environment', configEnvironment);
}

export function deleteConfigEnvironment(configEnvironment: ConfigEnvironment) {
  return instance.delete(`/environment/${configEnvironment.name}`);
}
