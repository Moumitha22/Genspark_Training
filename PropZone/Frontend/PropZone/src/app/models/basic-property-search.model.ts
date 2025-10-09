export interface BasicPropertySearchModel {
  locality?: string;
  city?: string;
  listingPurpose?: 'Sale' | 'Rent'; 
  propertyTypes?: string[]; 
  listerTypes?: string[]; 
  minPrice?: number;
  maxPrice?: number;
  keyword?: string;
  minArea?: number;
  maxArea?: number;
  hasImages?: boolean;
  sortBy?: 'CreatedAt' | 'Price';
  ascending?: boolean;
  status?: 'Available' | 'Sold' | 'Rented';
  isDiscountAvailable?:boolean;

  // admin
  listerId?: string;
  statuses?: string[];

}