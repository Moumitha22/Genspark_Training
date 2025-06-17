import { Component, HostListener, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Product } from '../product/product';
import { ProductService } from '../services/product.service';
import { ProductModel } from '../models/product.model';
import { BehaviorSubject, debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, FormsModule, Product],
  templateUrl: './products.html',
  styleUrl: './products.css'
})

export class Products implements OnInit {
  products: ProductModel[] = [];
  searchTerm: string = '';
  searchSubject = new BehaviorSubject<string>('');
  skip = 10;
  limit = 10;
  loading: boolean = false;
  showBackToTop: boolean = false; 
  
  private productService = inject(ProductService);
    
  ngOnInit(): void {
    this.searchSubject.pipe(
      debounceTime(500),
      distinctUntilChanged(),
      tap(() => {
        this.products = [];
        this.skip = 0;
        this.loading = true;
      }),
      switchMap(term =>
        this.productService.getProductsBySearch(term, this.limit, this.skip)
      ),
      tap(() => (this.loading = false))
    ).subscribe({
      next: (res) => {
        this.products = res.products;
        this.loading = false;
      },
      error: () => (this.loading = false),
    });

    this.searchSubject.next('');
  }

  onSearchChange() {
    this.searchSubject.next(this.searchTerm);
  }

  @HostListener('window:scroll', [])
  onWindowScroll() {
    
    const position = window.innerHeight + window.scrollY;
    const threshold = 200; 

    this.showBackToTop = window.scrollY > 300;

    const scrollHeight = Math.max(
      document.documentElement.scrollHeight,
      document.body.scrollHeight
    );


    if (position + threshold >= scrollHeight && !this.loading) {
      this.loadMore();
    }
  }

  scrollToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  loadMore() {
    this.loading = true;
    this.skip += this.limit;
    this.productService
      .getProductsBySearch(this.searchTerm, this.limit, this.skip)
      .subscribe({
        next: (res:any) => {
          this.products = [...this.products, ...res.products];
          // this.products = res.products;
          this.loading = false;
        },
        error: () => (this.loading = false),
      });
  }
}
