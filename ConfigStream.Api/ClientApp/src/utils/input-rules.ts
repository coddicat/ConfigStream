import { isValidRedisKey } from '@/utils/redisKey';

export const requiredRule = (v?: string) =>
  (v?.trim().length ?? 0) > 0 ? true : 'The field is required';

export const redisKeyRule = (v?: string) =>
  (v?.trim().length ?? 0) > 0 && !isValidRedisKey(v?.trim() ?? '')
    ? 'The value is not valid'
    : true;
