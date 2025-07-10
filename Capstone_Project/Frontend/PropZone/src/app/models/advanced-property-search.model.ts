import { DynamicFeatureFilterModel } from "./dynamic-feature-search.model";

export interface AdvancedPropertySearchModel {
  // 1. Core Filters
  listingPurpose?: 'Sale' | 'Rent';
  propertyTypes?: string[];
  listerId?: string;

  // 2. Location & Keyword
  locality?: string;
  city?: string;
  state?: string;
  keyword?: string;

  // 3. Price & Area
  priceRange?: {
    min?: number;
    max?: number;
  };
  areaRange?: {
    min?: number;
    max?: number;
  };

  // 4. Posting & Status
  postedBy?: string[];          // e.g., ['Owner', 'Agent']
  statuses?: string[];          // e.g., ['Available', 'Sold']
  postedAfter?: string;         // ISO string date
  postedBefore?: string;

  // 5. Optional Derived Filters
  hasImages?: boolean;

  // 6. Dynamic Feature Filters
  featureFilters?: DynamicFeatureFilterModel[];
}
