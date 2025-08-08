import { Component, inject, OnInit } from '@angular/core';
import { CartItem } from '../../models/cart-item';
import { CartService } from '../../services/cart.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { debounceTime, Subject } from 'rxjs';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-cart',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './cart.html',
  styleUrl: './cart.css'
})
export class CartComponent implements OnInit {
  cartItems: CartItem[] = [];

  private cartService = inject(CartService) ;
  quantityChanges = new Subject<{ productId: number, quantity: number }>();


  ngOnInit(): void {
    this.loadCart();

    this.quantityChanges
      .pipe(debounceTime(500))
      .subscribe(({ productId, quantity }) => {
        this.updateQuantity(productId, quantity);
      });
    }

  loadCart() {
    this.cartService.getCart().subscribe(items => {
      this.cartItems = items;
    });
  }

  increment(item: CartItem) {
    const newQty = item.quantity + 1;
    this.quantityChanges.next({ productId: item.productId, quantity: newQty });
  }

  decrement(item: CartItem) {
    if (item.quantity > 1) {
      const newQty = item.quantity - 1;
      this.quantityChanges.next({ productId: item.productId, quantity: newQty });
    }
  }

  updateQuantity(productId: number, qty: number) {
    this.cartService.updateQuantity(productId, qty).subscribe(() => this.loadCart());
  }

  removeItem(productId: number) {
    this.cartService.removeItem(productId).subscribe(() => this.loadCart());
  }

  clearCart() {
    this.cartService.clearCart().subscribe(() => this.loadCart());
  }

  getGrandTotal(): number {
    return this.cartItems.reduce((sum, item) => sum + item.unitPrice * item.quantity, 0);
  }

}
