import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../services/category.service';
import { ColorService } from '../../services/color.service';
import { ModelService } from '../../services/model.service';
import { StorageService } from '../../services/storage.service';
import { Router } from '@angular/router';
import { UserService } from '../../services/user.service';
import { UserModel } from '../../models/user';

@Component({
  selector: 'app-post-product',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './post-product.html',
  styleUrls: ['./post-product.css']
})
export class PostProductComponent implements OnInit {
  productForm!: FormGroup;
  categories: { id: number; name: string }[] = [];
  models: { id: number; name: string }[] = [];
  colors: { id: number; name: string }[] = [];
  storages: { id: number; name: string }[] = [];

  isSubmitting = false;
  errorMessage = '';
  selectedFile?: File;
  currentUser: UserModel | null = null;

  private fb = inject(FormBuilder);
  private router = inject(Router);
  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);
  private modelService = inject(ModelService);
  private colorService = inject(ColorService);
  private storageService = inject(StorageService);
  private userService = inject(UserService);

  
  ngOnInit(): void {
    this.buildForm();
    this.loadCategories();
    this.loadModels();
    this.loadColors();
    this.loadStorages();
    this.userService.user$.subscribe(user => {
      if (user) {
        this.currentUser = user;
        console.log('User is loaded:', user);
        console.log(this.currentUser.username);
        this.productForm.patchValue({
          userId: user.id
        });
      }
    });
  }


  buildForm(): void {
    this.productForm = this.fb.group({
      productName: ['', Validators.required],
      price: [0, [Validators.required, Validators.min(0)]],
      userId: [null],
      categoryId: ['', Validators.required],
      colorId: [null],
      modelId: [null],
      storageId: [null],
      sellStartDate: [null],
      sellEndDate: [null],
      isNew: [1]
    });
  }

  loadCategories(): void {
    this.categoryService.getAll().subscribe({
      next: (res) => (this.categories = res),
      error: () => (this.errorMessage = 'Failed to load categories')
    });
  }

  loadModels(): void {
    this.modelService.getAll().subscribe({
      next: (res) => (this.models = res),
      error: () => (this.errorMessage = 'Failed to load models')
    });
  }

  loadColors(): void {
    this.colorService.getAll().subscribe({
      next: (res) => (this.colors = res),
      error: () => (this.errorMessage = 'Failed to load colors')
    });
  }

  loadStorages(): void {
    this.storageService.getAll().subscribe({
      next: (res) => (this.storages = res),
      error: () => (this.errorMessage = 'Failed to load storages')
    });
  }


  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      this.selectedFile = input.files[0];
    }
  }

  onSubmit(): void {
    this.errorMessage = '';
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formData = new FormData();
    Object.entries(this.productForm.value).forEach(([key, value]) => {
      if (value !== null && value !== undefined) {
        formData.append(key, value.toString());
      }
    });

    if (this.selectedFile) {
      formData.append('Image', this.selectedFile);
    }

    this.productService.createProduct(formData).subscribe({
      next: () => {
        this.productForm.reset();
        this.selectedFile = undefined;
        this.router.navigate(['/products']);
      },
      error: (err) => {
        if (err.error?.Message) {
          this.errorMessage = err.error.Message;
        } else {
          this.errorMessage = 'Failed to post product.';
        }
      },
      complete: () => (this.isSubmitting = false)
    });
  }
}
