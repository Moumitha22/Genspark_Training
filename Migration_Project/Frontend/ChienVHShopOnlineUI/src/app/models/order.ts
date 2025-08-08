import { OrderDetailModel } from "./order-detail";

export interface OrderModel {
  id: number;
  orderName: string;
  orderDate: string;
  paymentType: string;
  status: string;
  customerName: string;
  customerPhone: string;
  customerEmail: string;
  customerAddress: string;
  orderDetails: OrderDetailModel[];
}