import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { PropertyImageModel } from '../../models/property-image.model';
import { PropertyImageService } from '../../core/services/property-image.service';
import { NotificationService } from '../../core/services/notification.service';
import { environment } from '../../environments/environment';


@Component({
  selector: 'app-upload-images',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './upload-images.html'
})
export class UploadPropertyImagesComponent implements OnInit {
  propertyId!: string;
  apiBaseUrl = environment.apiBaseUrl;

  existingImages: PropertyImageModel[] = [];
  selectedFilePreviews: { file: File; previewUrl: string }[] = [];

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
      const existingFileNames = new Set(this.selectedFilePreviews.map(f => f.file.name));

      newFiles.forEach(file => {
        if (!existingFileNames.has(file.name)) {
          const previewUrl = URL.createObjectURL(file);
          this.selectedFilePreviews.push({ file, previewUrl });
        }
      });

      input.value = '';
    }
  }


  removeSelectedFile(index: number) {
    const fileToRemove = this.selectedFilePreviews[index];
    URL.revokeObjectURL(fileToRemove.previewUrl); // Cleanup blob memory
    this.selectedFilePreviews.splice(index, 1);
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
    const files = this.selectedFilePreviews.map(f => f.file);
    if (!this.propertyId || files.length === 0) return;

    this.uploading = true;
    this.propertyImageService
      .uploadImages({ propertyId: this.propertyId, files })
      .subscribe({
        next: () => {
          this.uploadSuccess = true;
          this.notificationService.success('Images uploaded successfully');
          this.router.navigate(['/property', this.propertyId]);
        },
        error: err => {
          const backendMessage = err.error?.errors?.Files?.[0] || err.error?.Message || 'Upload failed';
          this.notificationService.error('Upload failed. ' + backendMessage);
          this.uploading = false;
        }
      });
  }

}
