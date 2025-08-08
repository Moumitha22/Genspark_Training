import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../services/category.service';
import { ColorService } from '../../services/color.service';
import { ModelService } from '../../services/model.service';
import { StorageService } from '../../services/storage.service';

@Component({
  selector: 'app-edit-product',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './edit-product.html',
  styleUrls: ['../post-product/post-product.css']
})
export class EditProductComponent implements OnInit {
  productForm!: FormGroup;
  productId!: number;
  isSubmitting = false;
  selectedFile?: File;
  errorMessage = '';

  categories: any[] = [];
  models: any[] = [];
  colors: any[] = [];
  storages: any[] = [];

  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);
  private modelService = inject(ModelService);
  private colorService = inject(ColorService);
  private storageService = inject(StorageService);

  ngOnInit(): void {
    this.productId = Number(this.route.snapshot.paramMap.get('id'));
    this.buildForm();
    this.loadProduct();
    this.loadDropdowns();
  }

  buildForm(): void {
    this.productForm = this.fb.group({
      productId: this.productId,
      productName: ['', Validators.required],
      price: [0, [Validators.required, Validators.min(0)]],
      userId: [null],
      categoryId: [null, Validators.required],
      modelId: [null],
      colorId: [null],
      storageId: [null],
      sellStartDate: [null],
      sellEndDate: [null],
      isNew: [1]
    });
  }

  loadDropdowns(): void {
    this.categoryService.getAll().subscribe({ next: res => (this.categories = res) });
    this.modelService.getAll().subscribe({ next: res => (this.models = res) });
    this.colorService.getAll().subscribe({ next: res => (this.colors = res) });
    this.storageService.getAll().subscribe({ next: res => (this.storages = res) });
  }

  loadProduct(): void {
    this.productService.getProductById(this.productId).subscribe({
      next: res => {
        const product = res;
        this.productForm.patchValue({
          productId: this.productId,
          productName: product.productName,
          price: product.price,
          userId: product.userId,
          categoryId: product.categoryId,
          modelId: product.modelId,
          colorId: product.colorId,
          storageId: product.storageId,
          sellStartDate: this.formatDate(product.sellStartDate),
          sellEndDate: this.formatDate(product.sellEndDate),
          isNew: Number(product.isNew)
        });
      },
      error: () => (this.errorMessage = 'Failed to load product.')
    });
  }

  formatDate(dateInput: string | Date | null | undefined): string | null {
    if (!dateInput) return null;
    const date = new Date(dateInput);
    return date.toISOString().split('T')[0];
  }



  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      this.selectedFile = input.files[0];
    }
  }

  onSubmit(): void {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formData = new FormData();
    Object.entries(this.productForm.value).forEach(([key, value]) => {
      if (value !== null && value !== undefined) {
        if (key === 'isNew') {
        formData.append(key, String(Number(value))); 
      } else {
        formData.append(key, value.toString());
      }
      }
    });
    
    if (this.selectedFile) {
      formData.append('Image', this.selectedFile);
    }
    for (const [key, value] of formData.entries()) {
      console.log(`${key}:`, value);
    }


    this.productService.updateProduct(this.productId, formData).subscribe({
      next: () => this.router.navigate(['/my-products']),
      error: () => {
        this.errorMessage = 'Failed to update product.';
        this.isSubmitting = false;
      },
      complete: () => (this.isSubmitting = false)
    });
  }
}
