import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductItem } from '../product-item/product-item';

@Component({
  selector: 'app-products',
  imports: [CommonModule, ProductItem],
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class Products {
  cartCount = 0;

  products = [
    { id: 1, name: 'Mouse', image: 'https://uniquec.com/wp-content/uploads/235.jpg', price: 4999 },
    { id: 2, name: 'Headphones', image: 'https://shop.zebronics.com/cdn/shop/files/Zeb-Thunderpro-blue-pic-1.jpg?v=1709202990', price: 2599 },
    { id: 3, name: 'Smartphone', image: 'https://i5.walmartimages.com/seo/Apple-iPhone-X-64GB-Unlocked-GSM-Phone-w-Dual-12MP-Camera-Space-Gray-B-Grade-Used_15c2b968-bb85-41a4-9292-b017f78fe797.a66ebbf32b6d53b6d6eb14c47434ac04.jpeg', price: 59999 }
  ];

  handleAddToCart() {
    this.cartCount++;
  }
}
