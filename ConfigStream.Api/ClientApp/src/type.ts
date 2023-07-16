export type TableHeader = {
  key: string;
  title: string;
  fixed?: boolean;
  align?: 'start' | 'end' | 'center';
  width?: number | string;
  sortable?: boolean;
};

export type SortItem = {
  key: string;
  order?: boolean | 'asc' | 'desc';
};
