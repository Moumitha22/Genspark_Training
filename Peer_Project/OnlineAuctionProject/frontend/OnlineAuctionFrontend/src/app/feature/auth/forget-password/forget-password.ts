import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { LoginService } from '../../../core/services/login.service';
import { UserService } from '../../../core/services/user.service';
import {
  matchPassValidator,
  passwordValidator,
} from '../../../misc/validators/passwordValitor';
import { LoginRequest } from '../../../core/models/LoginRequest';
import { ForgetPasswordRequest } from '../../../core/models/ForgetPassword';
import { Router } from '@angular/router';
import { TogglePassword } from "../../../shared/components/toggle-password/toggle-password";

@Component({
  selector: 'app-forget-password',
  imports: [ReactiveFormsModule, TogglePassword],
  templateUrl: './forget-password.html',
})
export class ForgetPassword {
  forgetPassword: FormGroup;
  emailVerified = false;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private router: Router
  ) {
    this.forgetPassword = this.fb.group(
      {
        email: ['', [Validators.required, Validators.email]],
        newPassword: ['', [Validators.required, passwordValidator]],
        confirmPassword: ['', [Validators.required]],
      },
      {
        validators: matchPassValidator,
      }
    );
  }

  checkEmail() {
    const email = this.forgetPassword.get('email')?.value;
    this.userService.getByEmail(email).subscribe({
      next: (user) => {
        if (user) {
          console.log(user);
          this.emailVerified = true;

          this.forgetPassword.addControl(
            'newPassword',
            this.fb.control('', [Validators.required, passwordValidator])
          );
          this.forgetPassword.addControl(
            'confirmPassword',
            this.fb.control('', [Validators.required])
          );
        }
      },
      error: () => {
        alert('Email not found!');
      },
    });
  }

  submit() {
    if (this.forgetPassword.valid) {
      const payload: ForgetPasswordRequest = {
        email: this.forgetPassword.get('email')?.value,
        newPassword: this.forgetPassword.get('newPassword')?.value,
      };
      this.userService.forgetPassword(payload).subscribe({
        next: () => {
          alert('Password reset successfully!');
          this.forgetPassword.reset();
          this.emailVerified = false;
          this.router.navigate(['/login']);
        },
        error: (err) => {
          console.error('Error resetting password:', err);
          alert('Failed to reset password. Please try again.');
        },
      });
    }
  }
}
