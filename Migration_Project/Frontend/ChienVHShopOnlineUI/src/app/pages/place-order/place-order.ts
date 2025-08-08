import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { CartService } from '../../services/cart.service';
import { OrderService } from '../../services/order.service';
import { CartItem } from '../../models/cart-item';
import { OrderRequestModel } from '../../models/order-request';
import { UserService } from '../../services/user.service';
import { UserModel } from '../../models/user';

@Component({
  selector: 'app-place-order',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './place-order.html',
  styleUrl: './place-order.css',
})
export class PlaceOrderComponent implements OnInit {
  cartItems: CartItem[] = [];
  currentUser: UserModel | null = null;
  total: number = 0;

  customerName = '';
  customerPhone = '';
  customerEmail = '';
  customerAddress = '';
  paymentType = 'COD';

  private cartService = inject(CartService);
  private orderService = inject(OrderService);
  private userService = inject(UserService);
  private router = inject(Router);

  ngOnInit(): void {
    this.cartService.getCart().subscribe(items => this.cartItems = items);
    this.cartService.getTotal().subscribe(res => this.total = res.total);
    this.userService.user$.subscribe(user => {
      if (user) {
        this.currentUser = user;
      }
    });
  }

  placeOrder(form: any) {
    if (form.invalid) {
      form.control.markAllAsTouched();
      alert('Please fill in all required fields.');
      return;
    }

    if (!this.currentUser) {
      alert('User is not logged in.');
      return;
    }

    const orderRequest: OrderRequestModel = {
      orderName: 'Order - ' + new Date().getTime(),
      paymentType: this.paymentType,
      userId: this.currentUser?.id,
      customerName: this.customerName,
      customerPhone: this.customerPhone,
      customerEmail: this.customerEmail,
      customerAddress: this.customerAddress,
      orderDetails: this.cartItems.map(item => ({
        productId: item.productId,
        price: item.unitPrice,
        quantity: item.quantity
      }))
    };

    this.orderService.createOrder(orderRequest).subscribe(() => {
      alert('Order placed successfully!');
      this.cartService.clearCart().subscribe(() => this.router.navigate(['/my-orders']));
    });
  }
}
