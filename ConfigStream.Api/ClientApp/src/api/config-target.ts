import { ConfigTarget } from '@/store/config-target';
import { instance } from './instance';

export async function getConfigTargets(search?: string) {
  const response = await instance.get<ConfigTarget[]>(`/target`, {
    params: { search }
  });
  return response.data;
}

export function createConfigTarget(configTarget: ConfigTarget) {
  return instance.put('/target', configTarget);
}

export function deleteConfigTarget(configTarget: ConfigTarget) {
  return instance.delete(`/target/${configTarget.name}`);
}
