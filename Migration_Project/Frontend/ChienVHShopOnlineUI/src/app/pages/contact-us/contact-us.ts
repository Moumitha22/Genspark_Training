import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ContactUsService } from '../../services/contactus.service';
import { RecaptchaWrapperComponent } from '../../components/recaptcha-wrapper/recaptcha-wrapper';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-contact-us',
  imports: [ReactiveFormsModule, RecaptchaWrapperComponent, CommonModule],
  templateUrl: './contact-us.html',
  styleUrls: ['./contact-us.css'],
})
export class ContactUsComponent {
  message = '';
  error = '';
  isSubmitting = false;
  captchaToken: string | null = null;
  readonly siteKey = '6LfwjpUrAAAAAGtE_SMRhfycFJpgEgk_TOIhY84a';

  private fb = inject(FormBuilder);
  private contactService = inject(ContactUsService);
  private router = inject(Router);

  contactForm = this.fb.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', Validators.required],
    content: ['', Validators.required],
  });

  onCaptchaResolved(token: string): void {
    this.captchaToken = token;
  }

  onSubmit(): void {
    this.message = '';
    this.error = '';
    this.isSubmitting = true;

    if (this.contactForm.invalid || !this.captchaToken) {
      this.error = 'Please fill out the form correctly and complete the reCAPTCHA.';
      this.isSubmitting = false;
      return;
    }

    const formValue = this.contactForm.value as any;
    const payload = {
      ...formValue,
      captchaToken: this.captchaToken,
    };

    this.contactService.submitContact(payload).subscribe({
      next: (res) => {
        this.message = res.message;
        alert(this.message);
        this.contactForm.reset();
        this.captchaToken = null;
        this.isSubmitting = false;
        this.router.navigate(['/products']);
      },
      error: (err) => {
        this.error = err.error?.message || 'Submission failed.';
        this.isSubmitting = false;
      },
    });
  }
}
