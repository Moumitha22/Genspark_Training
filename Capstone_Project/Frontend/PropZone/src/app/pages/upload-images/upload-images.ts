import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { PropertyImageModel } from '../../models/property-image.model';
import { PropertyImageService } from '../../core/services/property-image.service';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-upload-images',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './upload-images.html'
})
export class UploadPropertyImagesComponent implements OnInit {
  propertyId!: string;
  apiBaseUrl = 'http://localhost:5138';

  selectedFiles: File[] = [];
  existingImages: PropertyImageModel[] = [];

  uploadSuccess = false;
  uploading = false;
  errorMsg = '';

  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private propertyImageService = inject(PropertyImageService);
  private notificationService = inject(NotificationService);

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.propertyId = id;
      this.fetchExistingImages();
    }
  }

  fetchExistingImages() {
    this.propertyImageService.getImagesByPropertyId(this.propertyId).subscribe({
      next: (res:any) => (this.existingImages = res.data),
      error: () => (this.errorMsg = 'Failed to load existing images'),
    });
  }

  getImagePreview(file: File): string {
    return URL.createObjectURL(file);
  }


  onFileChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input?.files) {
      const newFiles = Array.from(input.files);
      const existingFileNames = new Set(this.selectedFiles.map(f => f.name));
      newFiles.forEach(file => {
        if (!existingFileNames.has(file.name)) {
          this.selectedFiles.push(file);
        }
      });
      input.value = '';
    }
  }

  removeSelectedFile(index: number) {
    this.selectedFiles.splice(index, 1);
    this.notificationService.success('Image deleted successfully');
  }


  deleteImage(imageId: string) {
    this.propertyImageService.deleteImage(imageId).subscribe({
      next: () => {
        this.notificationService.success('Image deleted successfully');
        this.existingImages = this.existingImages.filter(img => img.id !== imageId);
      },
      error: () => {
        this.notificationService.error('Failed to delete image');
      }
    });
  }


  upload() {
    if (!this.propertyId || this.selectedFiles.length === 0) return;

    this.uploading = true;
    this.propertyImageService
      .uploadImages({ propertyId: this.propertyId, files: this.selectedFiles })
      .subscribe({
        next: () => {
          this.uploadSuccess = true;
          this.errorMsg = '';
          this.uploading = false;
           this.notificationService.success('Images updated successfully');
          this.router.navigate(['/property', this.propertyId]);
        },
        error: err => {
          this.notificationService.error('Upload failed. Try again.');
          this.errorMsg = 'Upload failed. Try again.';
          this.uploading = false;
        },
      });
  }
}
