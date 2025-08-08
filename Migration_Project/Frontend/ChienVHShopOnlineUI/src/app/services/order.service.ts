import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { OrderModel } from '../models/order';
import { OrderRequestModel } from '../models/order-request';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  private baseUrl = `${environment.apiBaseUrl}/api/Order`;

  constructor(private http: HttpClient) {}

  createOrder(orderDto: OrderRequestModel): Observable<OrderModel> {
    return this.http.post<OrderModel>(`${this.baseUrl}`, orderDto);
  }

  getAllOrders(): Observable<OrderModel[]> {
    return this.http.get<OrderModel[]>(`${this.baseUrl}`);
  }

  getOrderById(id: number): Observable<OrderModel> {
    return this.http.get<OrderModel>(`${this.baseUrl}/${id}`);
  }

  getOrdersByUserId(userId: number): Observable<OrderModel[]> {
    return this.http.get<OrderModel[]>(`${this.baseUrl}/user/${userId}`);
  }

  cancelOrder(id: number): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}/cancel`, {});
  }

  updateOrderStatus(id: number, statusData: { status: string }): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}/status`, statusData);
  }

  updateOrderAddress(id: number, addressData: {
    customerAddress: string;
    customerPhone: string;
    customerEmail: string;
  }): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}/address`, addressData);
  }

  deleteOrder(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  downloadPdf(orderId: number): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/export/${orderId}`, {
      responseType: 'blob'
    });
  }

}
