import { PaginatedResult } from '../../core/models/PaginationModel';

export function paginate<T>(
  items: T[],
  page: number,
  pageSize: number
): PaginatedResult<T> {
  const totalItems = items.length;
  const totalPages = Math.ceil(totalItems / pageSize);
  const currentPage = Math.min(Math.max(page, 1), totalPages);
  const start = (currentPage - 1) * pageSize;
  const data = items.slice(start, start + pageSize);
  return { data, currentPage, totalPages, totalItems };
}
