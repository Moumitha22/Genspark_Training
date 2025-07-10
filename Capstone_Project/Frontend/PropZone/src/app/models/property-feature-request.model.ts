export interface PropertyFeatureRequest {
  featureId: string;
  value?: string;     // for Text or Boolean or Dropdown
  optionId?: string;  // for Dropdown or Multi-select
  dataType: string;   // "Text", "Boolean", "Dropdown", "Multiselect"
}