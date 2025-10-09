export interface DynamicFeatureFilterModel {
  featureId: string;
  values: string[];
  filterMode: 'Exact' | 'Range' | 'Boolean';
}
