export interface PaginatedAuction {
  page: number;
  pageSize: number;
  name?: string;
  startTime?: string;
  endTime?: string;
  sortBy?: string;
  sortDirection?: string;
  status?: string;
  sellerId?: string;
  startingPriceMin : number;
  startingPriceMax : number;
  reservePriceMin : number;
  reservePriceMax : number;
  hasFileAttachments : boolean;
  fileName?: string;
  }