export interface ApiResponse<T> {
  data: T
}

export interface Pagination {
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
}

export interface PagedResponse<T> {
  data: T[]
  pagination: Pagination
}