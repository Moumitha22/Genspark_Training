import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, CommonModule],
  templateUrl: './signup.html',
  styleUrl: '../login/login.css'
})
export class SignupComponent {
  registerForm: FormGroup;
  errorMessage = '';
  showPassword = false;
  loading = false;
  submitted = false;

  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  constructor() {
    this.registerForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  isInvalid(controlName: string): boolean {
    const control = this.registerForm.get(controlName);
    return !!control && control.invalid && (control.touched || control.dirty || this.submitted);
  }

  onSubmit(): void {
    this.submitted = true;
    if (this.registerForm.invalid) return;

    this.loading = true;
    const payload = this.registerForm.value;

    this.authService.register(payload).subscribe({
      next: () => {
        this.registerForm.reset();
        this.submitted = false;
        this.loading = false;
        this.router.navigate(['/login']);
      },
      error: err => {
        this.errorMessage = err.error?.Message || 'Registration failed';
        this.loading = false;
      }
    });
  }
}
