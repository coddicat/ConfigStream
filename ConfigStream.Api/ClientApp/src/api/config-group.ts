import { ConfigGroup } from '@/store/config-group';
import { instance } from './instance';

export async function getConfigGroups(search?: string) {
  const response = await instance.get<ConfigGroup[]>(`/group`, {
    params: { search }
  });
  return response.data;
}

export function createConfigGroup(configGroup: ConfigGroup) {
  return instance.put('/group', configGroup);
}

export function deleteConfigGroup(configGroup: ConfigGroup) {
  return instance.delete(`/group/${configGroup.name}`);
}
