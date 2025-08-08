import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { CartItem } from '../models/cart-item';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { tap } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class CartService {
  private baseUrl = `${environment.apiBaseUrl}/api/Cart`;
  private http = inject(HttpClient);

  private cartCountSubject = new BehaviorSubject<number>(0);
  cartCount$ = this.cartCountSubject.asObservable();

  constructor() {
    this.loadCartCount(); // Initialize on app load
  }

  private loadCartCount(): void {
    this.getCount().subscribe({
      next: (res) => this.cartCountSubject.next(res.count),
      error: () => this.cartCountSubject.next(0),
    });
  }

  getCart(): Observable<CartItem[]> {
    return this.http.get<CartItem[]>(this.baseUrl, { withCredentials: true });
  }

  addToCart(item: { productId: number; quantity: number }): Observable<any> {
    return this.http.post(this.baseUrl, item, { withCredentials: true }).pipe(
      tap(() => this.loadCartCount()) // update after adding
    );
  }

  updateQuantity(productId: number, quantity: number): Observable<any> {
    return this.http.put(`${this.baseUrl}/${productId}`, quantity, { withCredentials: true }).pipe(
      tap(() => this.loadCartCount())
    );
  }

  removeItem(productId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${productId}`, { withCredentials: true }).pipe(
      tap(() => this.loadCartCount())
    );
  }

  clearCart(): Observable<any> {
    console.log('Clear cart')
    return this.http.delete(this.baseUrl, { withCredentials: true }).pipe(
      tap(() => this.loadCartCount())
    );
  }

  getTotal(): Observable<{ total: number }> {
    return this.http.get<{ total: number }>(`${this.baseUrl}/total`, { withCredentials: true });
  }

  getCount(): Observable<{ count: number }> {
    return this.http.get<{ count: number }>(`${this.baseUrl}/count`, { withCredentials: true });
  }
}
