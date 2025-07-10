import { PropertyFeatureRequest } from "./property-feature-request.model";
import { PropertyLocationModel } from "./property-location.model";

export interface PropertyAddRequest {
  title: string;
  description?: string;
  price: number;
  listerType: string;
  propertyType: string; 
  listingPurpose: string;
  areaSqFt: number;
  location: PropertyLocationModel;
  features: PropertyFeatureRequest[];
}

