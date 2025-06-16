// import { Component, HostListener, OnInit, OnDestroy } from '@angular/core';
// import { CommonModule } from '@angular/common';
// import { FormsModule } from '@angular/forms';
// import { HttpClient } from '@angular/common/http';
// import { Product } from '../product/product';
// import { ProductModel } from '../models/product.model';
// import { Subject, debounceTime, distinctUntilChanged, switchMap, takeUntil } from 'rxjs';

// @Component({
//   selector: 'app-home',
//   standalone: true,
//   imports: [CommonModule, FormsModule, Product],
//   templateUrl: './home.html',
//   styleUrls: ['./home.css']
// })
// export class Home implements OnInit, OnDestroy {
//   searchTerm: string = '';
//   products: Product[] = [];
//   skip = 0;
//   limit = 15;
//   isLoading = false;
//   showBackToTop = false;

//   private destroy$ = new Subject<void>();
//   private searchSubject = new Subject<string>();

//   constructor(private http: HttpClient) {}

//   ngOnInit() {
//     this.searchSubject.pipe(
//       debounceTime(400),
//       distinctUntilChanged(),
//       switchMap(term => {
//         this.skip = 0;
//         this.isLoading = true;
//         return this.http.get<any>(
//           `https://dummyjson.com/products/search?q=${term}&limit=${this.limit}&skip=${this.skip}`
//         );
//       }),
//       takeUntil(this.destroy$)
//     ).subscribe(res => {
//       this.products = res.products;
//       this.isLoading = false;
//     });

//     this.searchSubject.next('');
//   }

//   onSearch(term: string) {
//     this.searchTerm = term;
//     this.searchSubject.next(term);
//   }

//   @HostListener('window:scroll', [])
//   onWindowScroll() {
//     const scrollPosition = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;

//     // Back to Top Button Visibility
//     this.showBackToTop = scrollPosition > 300;

//     // Infinite Scroll Trigger
//     if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight - 500 && !this.isLoading) {
//       this.loadMore();
//     }
//   }

//   loadMore() {
//     this.isLoading = true;
//     this.skip += this.limit;
//     this.http.get<any>(
//       `https://dummyjson.com/products/search?q=${this.searchTerm}&limit=${this.limit}&skip=${this.skip}`
//     ).subscribe(res => {
//       this.products = [...this.products, ...res.products];
//       this.isLoading = false;
//     });
//   }

//   scrollToTop() {
//     window.scrollTo({ top: 0, behavior: 'smooth' });
//   }

//   ngOnDestroy() {
//     this.destroy$.next();
//     this.destroy$.complete();
//   }
// }


// <div class="container py-4 px-3">
//   <h2 class="text-center mb-4">Product Listing</h2>

//   <!-- Search Input -->
//   <div class="row justify-content-center mb-4">
//     <div class="col-md-6 col-lg-4">
//       <input
//         type="text"
//         class="form-control"
//         placeholder="Search products..."
//         [(ngModel)]="searchTerm"
//         (ngModelChange)="onSearchChange()"
//       />
//     </div>
//   </div>

//   <!-- Conditional Content -->
//   @if (loading) {
//     <div class="text-center my-3">
//       <div class="spinner-border text-primary" role="status"></div>
//     </div>
//   } @else {
//     <!-- Product Grid -->
//     @if (products.length > 0) {
//       <div class="d-flex flex-wrap justify-content-start gap-3">
//         @for (p of products; track p) {
//           <div class="product-col">
//             <app-product [product]="p" />
//           </div>
//         }
//       </div>
//     } 
//   }
// </div>



// <div class="container py-4 px-3">
//   <h2 class="text-center mb-4">Product Listing</h2>

//   <!-- Search Input -->
//   <div class="row justify-content-center mb-4">
//     <div class="col-md-6 col-lg-4">
//       <input
//         type="text"
//         class="form-control"
//         placeholder="Search products..."
//         [(ngModel)]="searchTerm"
//         (ngModelChange)="onSearchChange()"
//       />
//     </div>
//   </div>

//   <!-- Product Grid (5 per row) -->
//   <div class="d-flex flex-wrap justify-content-start">
//     @for (p of products; track p) {
//       <div class="custom-col mb-4">
//         <app-product [product]="p" />
//       </div>
//     }
//   </div>

//   <!-- Loading Spinner -->
//   <div class="text-center mt-4">
//     @if (loading) {
//       <div class="spinner-border text-primary" role="status"></div>
//     }
//   </div>
// </div>
