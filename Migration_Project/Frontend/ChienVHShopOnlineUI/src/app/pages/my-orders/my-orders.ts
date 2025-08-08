import { Component, inject, OnInit } from '@angular/core';
import { OrderService } from '../../services/order.service';
import { OrderModel } from '../../models/order';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { UserModel } from '../../models/user';

@Component({
  selector: 'app-my-orders',
  imports: [CommonModule],
  templateUrl: './my-orders.html',
  styleUrls: ['./my-orders.css']
})
export class MyOrdersComponent implements OnInit {
  orders: OrderModel[] = [];
  currentUser: UserModel | null = null;
  userId: number| null = null; 

  private orderService = inject(OrderService);
  private userService = inject(UserService);

  ngOnInit(): void {
    this.userService.user$.subscribe(user => {
      if (user) {
        this.currentUser = user;
        this.userId = user.id;
        this.loadOrders(); 
      } else {
        alert('User is not logged in.');
      }
    });
  }


  loadOrders(): void {
    if (!this.currentUser) {
      alert('User is not logged in.');
      return;
    }
    this.orderService.getOrdersByUserId(this.currentUser.id).subscribe({
      next: (orders) => {
        this.orders = orders;
      },
      error: (err) => {
        console.error('Failed to load orders', err);
      }
    });
  }

  downloadOrderPdf(orderId: number): void {
    this.orderService.downloadPdf(orderId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Order_${orderId}.pdf`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        console.error('Failed to download order PDF', err);
      }
    });
  }


}
