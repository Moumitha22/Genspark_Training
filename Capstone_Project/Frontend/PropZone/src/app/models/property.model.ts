import { PropertyLocationModel } from "./property-location.model";
import { PropertyFeatureModel } from './property-feature.model';

export interface PropertyModel {
  id: string;
  listerId: string;
  title: string;
  description?: string;
  price: number;
  location: PropertyLocationModel;
  propertyType: string;
  listingPurpose: string;
  listerType: string;
  bedrooms?: number;
  bathrooms?: number;
  areaSqFt: number;
  createdAt: string; 
  status: string;
  imageUrls: string[];
  featureSummary: PropertyFeatureModel[];
}
