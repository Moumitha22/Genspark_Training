import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { LoginRequest } from '../../models/login-request.model';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, CommonModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  returnUrl: string = '/';
  errorMessage = '';
  loading = false;
  showPassword = false;
  selectedRole: 'Buyer' | 'Lister' | 'Admin' | null = null;
  isAdminLogin = false;

  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private fb = inject(FormBuilder);
  private notificationService = inject(NotificationService);

  constructor() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    const queryParams = this.route.snapshot.queryParamMap;
    this.returnUrl = queryParams.get('returnUrl') || '/';

    const reason = queryParams.get('reason');
    if (reason === 'notAuthenticated') {
      this.notificationService.warning('Please login to continue.');
    }

    if (reason === 'invalidRole') {
      const required = queryParams.get('required');
      const actual = queryParams.get('actual');
      this.notificationService.warning(`Access denied for role '${actual}'. Please login as '${required}' to continue.`);
    }
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  selectRole(role: 'Buyer' | 'Lister' | 'Admin'): void {
    this.selectedRole = role;
  }

  loginAsAdmin(): void {
    this.isAdminLogin = true;
    this.selectedRole = 'Admin';
  }

  backToUserLogin(): void {
    this.isAdminLogin = false;
    this.selectedRole = null;
  }

  onSubmit(): void {
    if (this.loginForm.invalid) return;

    if (!this.isAdminLogin && !this.selectedRole) {
      this.errorMessage = 'Please select a role to continue.';
      return;
    }

    this.loading = true;

    const payload: LoginRequest = {
      ...this.loginForm.value,
      role: this.isAdminLogin ? 'Admin' : this.selectedRole!
    };

    this.authService.login(payload).subscribe({
      next: () => {
        this.notificationService.success('Logged in successfully');
        const redirect = this.isAdminLogin ? '/admin/dashboard' : this.selectedRole == 'Lister' ? '/lister/dashboard' : '/';
        this.router.navigate([redirect]);
      },
      error: err => {
        const backendMessage = err.error?.errors?.general?.[0] || err.error?.Message || 'Login failed';
        this.errorMessage = backendMessage;
        this.loading = false;
      }
    });
  }
}
