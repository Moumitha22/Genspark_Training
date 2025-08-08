import { Component, inject, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CircleArrowRight, LucideAngularModule } from 'lucide-angular';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { LoginService } from '../../../core/services/login.service';
import { Spinner } from '../../../shared/components/spinner/spinner';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { loadTheme } from '../../../shared/utils/theme-utils';
import { LoginRequest } from '../../../core/models/LoginRequest';
import { TogglePassword } from "../../../shared/components/toggle-password/toggle-password";
import { setupLowercaseEmail } from '../../../shared/utils/function-utils';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, LucideAngularModule, Spinner, RouterLink, TogglePassword],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login implements OnInit {
  loginForm!: FormGroup;
  readonly ArrowRightwards = CircleArrowRight;
  isLoading = false;

  constructor(
    private snackBar: SnackbarService,
    private formBuilder: FormBuilder,
    private loginService: LoginService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    });
    setupLowercaseEmail(this.loginForm, 'email');
    loadTheme();
  }

  get email() {
    return this.loginForm.get('email');
  }
  get password() {
    return this.loginForm.get('password');
  }

  onLogin(): void {
    this.isLoading = true;

    const payload: LoginRequest = {
      email: this.loginForm.get('email')?.value,
      password: this.loginForm.get('password')?.value,
    };
    this.loginService.login(payload).subscribe({
      next: () => {
        setTimeout(() => {
          const role = this.authService.getUserRole();
          this.isLoading = false;

          switch (role) {
            case 'Seller':
              this.router.navigate(['/seller/dashboard']);
              break;
            case 'Admin':
              this.router.navigate(['/admin/dashboard']);
              break;
            case 'Bidder':
              this.router.navigate(['/bidder/dashboard']);
              break;
            default:
              this.router.navigate(['/login']);
              break;
          }
        }, 2000);
      },
      error: (err) => {
        this.isLoading = false;
        console.error('Login failed:', err);
        // this.snackBar.showError('Login failed. Please check your credentials.');
        let message = 'Please check your credentials.';
  
        if (err.error?.message === 'Validation failed' && err.error?.errors?.email) {
          message = err.error.errors.email[0]; 
        } else if (err.error?.message) {
          message = err.error.message;
        }
        this.snackBar.showError(`Login failed. ${message}`);
      },
    });
  }
}
