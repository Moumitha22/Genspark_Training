import { FeatureApplicabilityModel } from "./feature-applicability.model";
import { FeatureOption } from "./feature-option.model";

export interface FeatureAdminModel {
  id: string;
  name: string;
  dataType: string;
  filterMode: string;
  options: FeatureOption[];
  applicability: FeatureApplicabilityModel[];
}