// import { Component, inject, OnInit } from '@angular/core';
// import { ProductModel } from '../../models/product';
// import { ProductService } from '../../services/product.service';
// import { ActivatedRoute } from '@angular/router';
// import { ProductCard } from '../../components/product-card/product-card';
// import { CommonModule } from '@angular/common';
// import { CategoryService } from '../../services/category.service';
// import { CategoryModel } from '../../models/category';

// @Component({
//   selector: 'app-products',
//   imports: [ProductCard, CommonModule],
//   templateUrl: './my-products.html',
//   styleUrl: './products/products.css'
// })

// export class MyProductsComponent implements OnInit {
//   products: ProductModel[] = [];
//   categories: CategoryModel[] = [];
//   isLoading = true;
//   errorMessage = '';
//   categoryId: number | null = null;

//   currentPage = 1;
//   pageSize = 6;

//   private productService = inject(ProductService);
//   private categoryService = inject(CategoryService);

//   ngOnInit(): void {
//     this.loadCategories();
//     this.loadProducts();
//   }

//   loadCategories(): void {
//     this.categoryService.getAll().subscribe({
//       next: (data) => (this.categories = data),
//       error: (err) => console.error('Failed to load categories', err)
//     });
//   }

//   loadProducts(): void {
//     this.isLoading = true;

//     const request$ = this.categoryId
//       ? this.productService.getPagedProductsByCategory(this.categoryId, this.currentPage, this.pageSize)
//       : this.productService.getPagedProducts(this.currentPage, this.pageSize);

//     request$.subscribe({
//       next: (res) => {
//         this.products = res;
//         this.isLoading = false;
//       },
//       error: (err) => {
//         this.errorMessage = 'Failed to load products.';
//         this.isLoading = false;
//         console.error(err);
//       }
//     });
//   }

//   deleteProduct(productId: number): void {
//     this.productService.deleteProduct(productId).subscribe({
//       next: () => {
//         this.products = this.products.filter(p => p.id !== productId);
//       },
//       error: () => alert('Failed to delete product.')
//     });
//   }

//   onCategorySelected(categoryId: number | null): void {
//     this.categoryId = categoryId;
//     this.currentPage = 1; 
//     this.loadProducts();
//   }

//   onNextPage(): void {
//     this.currentPage++;
//     this.loadProducts();
//   }

//   onPrevPage(): void {
//     if (this.currentPage > 1) {
//       this.currentPage--;
//       this.loadProducts();
//     }
//   }
// }

import { Component, inject, OnInit } from '@angular/core';
import { ProductModel } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';
import { ProductCard } from '../../components/product-card/product-card';

@Component({
  selector: 'app-my-products',
  standalone: true,
  imports: [ProductCard, CommonModule],
  templateUrl: './my-products.html',
  styleUrl: '../products/products.css'
})
export class MyProductsComponent implements OnInit {
  products: ProductModel[] = [];
  isLoading = true;
  errorMessage = '';

  currentPage = 1;
  pageSize = 6;

  private productService = inject(ProductService);
  private authService = inject(AuthService);

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.isLoading = true;
    const userId = this.authService.currentUser?.id;

    if (!userId) {
      this.errorMessage = 'User not authenticated.';
      this.isLoading = false;
      return;
    }

    this.productService.getPagedProductsByUserId(+userId, this.currentPage, this.pageSize).subscribe({
      next: (res) => {
        this.products = res;
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = 'Failed to load your products.';
        this.isLoading = false;
        console.error(err);
      }
    });
  }

  onNextPage(): void {
    this.currentPage++;
    this.loadProducts();
  }

  onPrevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadProducts();
    }
  }

  deleteProduct(productId: number): void {
    this.productService.deleteProduct(productId).subscribe({
      next: () => {
        this.products = this.products.filter(p => p.id !== productId);
      },
      error: () => alert('Failed to delete product.')
    });
  }
}
