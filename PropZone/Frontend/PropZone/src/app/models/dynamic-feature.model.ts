import { FeatureOption } from "./feature-option.model";

export interface DynamicFeatureModel {
  id: string;
  name: string;
  dataType: string; 
  filterMode: 'Exact' | 'Boolean' | 'Range';
  options: FeatureOption[];
}