import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NewsService } from '../../services/news.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-post-news',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './post-news.html',
  styleUrl: './post-news.css'
})
export class PostNewsComponent {
  newsForm: FormGroup;
  selectedImageFile: File | null = null;
  previewUrl: string | ArrayBuffer | null = null;

  constructor(
    private fb: FormBuilder,
    private newsService: NewsService,
    private router: Router
  ) {
    this.newsForm = this.fb.group({
      title: ['', Validators.required],
      shortDescription: [''],
      content: [''],
      userId: [1, Validators.required], 
      status: [1, Validators.required], 
    });
  }

  onFileChange(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.selectedImageFile = file;

      const reader = new FileReader();
      reader.onload = (e) => (this.previewUrl = reader.result);
      reader.readAsDataURL(file);
    }
  }

  onSubmit(): void {
    if (this.newsForm.invalid) return;

    const formData = new FormData();
    formData.append('title', this.newsForm.value.title);
    formData.append('shortDescription', this.newsForm.value.shortDescription || '');
    formData.append('content', this.newsForm.value.content || '');
    formData.append('userId', this.newsForm.value.userId.toString());
    formData.append('status', this.newsForm.value.status.toString());

    if (this.selectedImageFile) {
      formData.append('image', this.selectedImageFile);
    }

    this.newsService.create(formData).subscribe({
      next: () => {
        alert('News added successfully!');
        this.router.navigate(['/news']);
      },
      error: () => {
        alert('Failed to add news');
      },
    });
  }
}
