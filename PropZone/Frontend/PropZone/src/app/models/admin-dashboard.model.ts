import { ChartItemModel } from "./chart-item.model";

export interface AdminDashboardModel {
  totalUsers: number;
  totalProperties: number;
  totalInquiries: number;
  totalActiveListers: number;
  propertyTypeChart: ChartItemModel[];
  propertyPurposeChart: ChartItemModel[];
  propertyStatusChart: ChartItemModel[];
}
