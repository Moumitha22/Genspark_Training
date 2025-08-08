import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { environment } from '../../environments/environment';
import { ProductModel } from '../../models/product';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-product-card',
  imports: [RouterLink, CommonModule],
  templateUrl: './product-card.html',
  styleUrl: './product-card.css'
})
export class ProductCard {  
  @Input() product!: ProductModel;
  @Input() role: 'Admin' | 'User' | null = null;

  @Output() delete = new EventEmitter<number>();
  private router = inject(Router);
  private cartService = inject(CartService);

  imageBaseUrl = environment.apiBaseUrl+"/";

  goToDetails(event: MouseEvent) {
    this.router.navigate(['/product-details', this.product.id]);
  }

  deleteProduct(event: MouseEvent): void {
    event.stopPropagation(); 
    if (confirm('Are you sure you want to delete this product?')) {
      this.delete.emit(this.product.id);
    }
  }

  addToCart() {
    this.cartService.addToCart({ productId: this.product.id, quantity: 1 }).subscribe({
      next: res => alert(res.message || 'Added to cart'),
      error: err => {
        const message = err?.error?.message || 'Failed to add to cart';
        alert(message);
      }
    });
  }

}