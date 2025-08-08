export interface OrderRequestModel {
  orderName: string;
  paymentType: string;
  userId: number;
  customerName: string;
  customerPhone: string;
  customerEmail: string;
  customerAddress: string;
  orderDetails: {
    productId: number;
    price: number;
    quantity: number;
  }[];
}
