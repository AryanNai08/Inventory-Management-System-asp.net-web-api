export interface APIResponse<T> {
  status: boolean;      // Matches backend 'Status' (case-insensitive in most JSON configurations)
  statusCode: number;   // Matches backend 'StatusCode'
  message: string;      // Matches backend 'Message'
  data: T;              // Matches backend 'Data'
  error?: string;       // Matches backend 'Error'
}

export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface PaginationParams {
  pageNumber: number;
  pageSize: number;
  sortColumn?: string;
  sortOrder?: 'asc' | 'desc';
}
