interface BasicDiscountFilter {
  code?: string;
  minDiscountValue: number | null;
  maxDiscountValue: number | null;
  isPercentage: boolean | null;
  fromDate: Date | null;
  toDate: Date | null;
  isDeleted: boolean | null;
  isActive: boolean | null;
  typeOfProperty?: string | null;
  purpose?: 'Sale' | 'Rent' | null;
  minPrice?: number;
  maxPrice?: number;
  sortBy:string,
  ascending: boolean,
}