export function sort<T>(arr: T[], predicate: (v: T) => string): T[] {
  return arr.map(x => x).sort((x, y) => (predicate(x) > predicate(y) ? 1 : -1));
}
