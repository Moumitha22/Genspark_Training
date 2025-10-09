import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ContactListerRequest } from '../../models/contact-lister-request.model';
import { InquiryService } from '../../core/services/inquiry.service';

@Component({
  selector: 'app-contact-lister-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './contact-lister-form.html',
  styleUrl: './contact-lister-form.css'

})
export class ContactListerFormComponent implements OnInit {
  @Input() property!: { id: string; title: string, location: string };
  @Output() closed = new EventEmitter<void>();

  form!: FormGroup;
  listerInfo: any = null;
  submitting = false;
  fetchingLister = false;

  private fb = inject(FormBuilder);
  private InquiryService = inject(InquiryService);

  ngOnInit(): void {
    this.form = this.fb.group({
      buyerPhoneNumber: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
      buyerEmail: ['', [Validators.required, Validators.email]],
      message: ['', [Validators.required, Validators.maxLength(500)]]
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    
    this.submitting = true;

    const request: ContactListerRequest = {
      propertyId: this.property.id,
      buyerPhoneNumber: this.form.value.buyerPhoneNumber,
      buyerEmail: this.form.value.buyerEmail,
      message: this.form.value.message
    };

    this.InquiryService.contactLister(request).subscribe({
        next: (result) => {
          this.submitting = false;
          this.fetchingLister = true; 

          setTimeout(() => {
            this.listerInfo = result;
            this.fetchingLister = false;
          }, 1500); 
        },
        error: (err) => {
          this.submitting = false;
          console.error('❌ Error: ' + (err.error?.message || err.message));
        }
      });
    }

    close(): void {
      this.closed.emit();
    }
}
