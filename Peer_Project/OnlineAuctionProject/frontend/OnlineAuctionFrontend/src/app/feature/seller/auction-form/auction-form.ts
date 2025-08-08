import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { GoalIcon, LucideAngularModule, UploadCloud } from 'lucide-angular';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { AuthService } from '../../../core/services/auth.service';
import { Spinner } from '../../../shared/components/spinner/spinner';
import { AuctionService } from '../../../core/services/auction.service';

@Component({
  selector: 'app-auction-form',
  imports: [LucideAngularModule, ReactiveFormsModule, Spinner],
  templateUrl: './auction-form.html',
})
export class AuctionForm implements OnInit {
  readonly goal = GoalIcon;
  readonly upload = UploadCloud;
  auctionForm!: FormGroup;
  selectedFiles: File[] = [];
  isloading = false;

  constructor(
    private formbuilder: FormBuilder,
    private auctionService: AuctionService,
    private snackBar: SnackbarService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    const userId = this.authService.getUserIdFromToken();
    const role = this.authService.getUserRole();
    this.auctionForm = this.formbuilder.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required],
      status: ['', Validators.required],
      sellerId: [{ value: '', disabled: true }, Validators.required],
      startingPrice: ['', Validators.required],
      reservePrice: ['', Validators.required],
    });
    this.auctionForm.get('sellerId')?.setValue(userId);
    window.addEventListener('beforeunload', this.beforeUnloadHandler);
  }
  onFileChange(event: any) {
    if (event.target.files && event.target.files.length > 0) {
      this.selectedFiles = Array.from(event.target.files);
    }
  }
  submitAuction() {
    const formValue = this.auctionForm.getRawValue();
    const formData = new FormData();

    formData.append('name', formValue.name);
    formData.append('description', formValue.description);
    formData.append('startTime', new Date(formValue.startTime).toISOString());
    formData.append('endTime', new Date(formValue.endTime).toISOString());

    formData.append('status', formValue.status);
    formData.append('sellerId', formValue.sellerId);
    formData.append('startingPrice', formValue.startingPrice.toString());
    if (formValue.reservePrice) {
      formData.append('reservePrice', formValue.reservePrice.toString());
    }

    this.selectedFiles.forEach((file, index) => {
      formData.append('fileAttachments', file, file.name);
    });

    this.auctionService.createAuction(formData).subscribe({
      next: () => {
        this.isloading = true;
        setTimeout(() => {
          this.isloading = false;
          this.snackBar.showSuccess('Auction created!');
          this.auctionForm.reset();
        }, 2000);
      },
      error: (err) => {
        const errorMessage =
          err?.error?.message || err?.message || 'Error occurred';
        this.snackBar.showError(`Failed to create: ${errorMessage}`);
        this.isloading = false;
        console.error('Error creating auction:', err);
      },
    });
  }

  ngOnDestroy(): void {
    window.removeEventListener('beforeunload', this.beforeUnloadHandler);
  }

  beforeUnloadHandler = (event: BeforeUnloadEvent) => {
    if (this.auctionForm.dirty) {
      event.preventDefault();
    }
  };
}
