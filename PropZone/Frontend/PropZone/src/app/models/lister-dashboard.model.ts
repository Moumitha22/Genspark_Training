import { ChartItemModel } from "./chart-item.model";

export interface ListerDashboardModel {
  totalPropertiesListed: number;
  totalForSale: number;
  totalForRent: number;
  totalSoldOut: number;
  totalRented: number;
  totalAvailable: number;
  totalInquiriesReceived: number;
  propertyTypeChart: ChartItemModel[];
  propertyPurposeChart: ChartItemModel[];
  propertyStatusChart: ChartItemModel[];
}
