import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, CommonModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent {
  loginForm: FormGroup;
  loading = false;
  submitted = false;
  errorMessage = '';
  showPassword = false;

  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  constructor() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }
  isInvalid(controlName: string): boolean {
  const control = this.loginForm.get(controlName);
  return !!control && control.invalid && (control.touched || control.dirty || this.submitted);
}


  onSubmit(): void {
    this.submitted = true;
    if (this.loginForm.invalid) return;

    this.loading = true;
    const payload = this.loginForm.value;

    this.authService.login(payload).subscribe({
      next: () => {
        this.authService.userRole$.subscribe(() => {
          this.router.navigate(['/']);
        });
      },
      error: err => {
        this.errorMessage = err.error?.Message || 'Login failed';
        this.loading = false;
      }
    });
  }
}
