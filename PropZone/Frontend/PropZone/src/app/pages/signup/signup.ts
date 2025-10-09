import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { RegisterRequest } from '../../models/register-request.model';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-signup',
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  templateUrl: './signup.html',
  styleUrl: '../login/login.css'
})
export class SignupComponent {
  registerForm: FormGroup;
  errorMessage = '';
  showPassword = false;

  private notificationService = inject(NotificationService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private fb = inject(FormBuilder);

  constructor() {
    this.registerForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      phoneNumber: [''],
      role: ['Buyer', Validators.required]
    });
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  onSubmit(): void {
    if (this.registerForm.invalid) 
      return;

    const payload: RegisterRequest = this.registerForm.value;

    this.authService.register(payload).subscribe({
      next: () => {
        this.registerForm.reset({ role: 'Buyer' });
        this.notificationService.success('Registration successful! You can now login.');
        this.router.navigate(['/login']);
      },
      error: err => {
        const backendMessage = err.error?.errors?.general?.[0] || err.error?.Message || 'Registration failed';   
        this.errorMessage = backendMessage;
      }
    });
  }
}
