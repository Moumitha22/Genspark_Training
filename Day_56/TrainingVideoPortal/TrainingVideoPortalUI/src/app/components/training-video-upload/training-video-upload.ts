import { Component, inject } from '@angular/core';
import { TrainingVideoService } from '../../services/training-video.service';
import { TrainingVideoUpload } from '../../models/training-video-upload';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';


@Component({
  selector: 'app-training-video-upload',
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './training-video-upload.html',
  styleUrl: './training-video-upload.css'
})

export class TrainingVideoUploadComponent {
  model: Partial<TrainingVideoUpload> = {
    title: '',
    description: '',
  };

  selectedFile: File | null = null;
  isUploading = false;
  uploadSuccess = false;
  uploadError = false;
  showToast = false;

  private uploadService = inject(TrainingVideoService);
  private router = inject(Router);

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      this.selectedFile = input.files[0];
    }
  }

  onSubmit() {
    if (!this.selectedFile) return;

    const payload: TrainingVideoUpload = {
      title: this.model.title!,
      description: this.model.description!,
      file: this.selectedFile,
    };

    this.isUploading = true;
    this.uploadService.uploadVideo(payload).subscribe({
      next: () => {
        this.uploadSuccess = true;
        this.uploadError = false;
        this.selectedFile = null;
        this.isUploading = false;
        this.showToast = true;
        this.model = { title: '', description: '' };
        setTimeout(() => {
          this.router.navigate(['/videos']);
        }, 2000);
      },
      error: () => {
        this.uploadSuccess = false;
        this.uploadError = true;
        this.isUploading = false;
      },
    });
  }
}
