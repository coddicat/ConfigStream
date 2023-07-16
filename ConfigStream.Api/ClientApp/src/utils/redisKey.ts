export function isValidRedisKey(key: string): boolean {
  const MAX_KEY_LENGTH = 100;
  const INVALID_CHAR_REGEX = /[\p{Cc}\p{Z}]/gu;

  if (typeof key !== 'string') {
    return false;
  }

  if (key.length === 0 || key.length > MAX_KEY_LENGTH) {
    return false;
  }

  if (INVALID_CHAR_REGEX.test(key)) {
    return false;
  }

  return true;
}
