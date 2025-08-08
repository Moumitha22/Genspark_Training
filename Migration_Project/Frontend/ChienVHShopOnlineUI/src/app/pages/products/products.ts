import { Component, inject, OnInit } from '@angular/core';
import { ProductModel } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { ActivatedRoute } from '@angular/router';
import { ProductCard } from '../../components/product-card/product-card';
import { CommonModule } from '@angular/common';
import { CategoryService } from '../../services/category.service';
import { CategoryModel } from '../../models/category';

@Component({
  selector: 'app-products',
  imports: [ProductCard, CommonModule],
  templateUrl: './products.html',
  styleUrl: './products.css'
})

export class ProductsComponent implements OnInit {
  products: ProductModel[] = [];
  categories: CategoryModel[] = [];
  isLoading = true;
  errorMessage = '';
  categoryId: number | null = null;

  currentPage = 1;
  pageSize = 6;

  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);

  ngOnInit(): void {
    this.loadCategories();
    this.loadProducts();
  }

  loadCategories(): void {
    this.categoryService.getAll().subscribe({
      next: (data) => (this.categories = data),
      error: (err) => console.error('Failed to load categories', err)
    });
  }

  loadProducts(): void {
    this.isLoading = true;

    const request$ = this.categoryId
      ? this.productService.getPagedProductsByCategory(this.categoryId, this.currentPage, this.pageSize)
      : this.productService.getPagedProducts(this.currentPage, this.pageSize);

    request$.subscribe({
      next: (res) => {
        this.products = res;
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = 'Failed to load products.';
        this.isLoading = false;
        console.error(err);
      }
    });
  }

  deleteProduct(productId: number): void {
    this.productService.deleteProduct(productId).subscribe({
      next: () => {
        this.products = this.products.filter(p => p.id !== productId);
      },
      error: () => alert('Failed to delete product.')
    });
  }

  onCategorySelected(categoryId: number | null): void {
    this.categoryId = categoryId;
    this.currentPage = 1; 
    this.loadProducts();
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
}
