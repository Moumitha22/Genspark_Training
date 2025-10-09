export interface PaginationModel {
  page: number;
  pageSize: number;
}

export interface PaginationInfo {
  currentPage: number,
  pageSize: number,
  totalItems: number,
  totalPages: number
};