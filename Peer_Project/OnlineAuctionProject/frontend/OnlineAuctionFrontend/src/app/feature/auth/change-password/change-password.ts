import { Component, inject, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import {
  matchPassValidator,
  passwordValidator,
} from '../../../misc/validators/passwordValitor';
import { UserService } from '../../../core/services/user.service';
import { ChangePasswordModel } from '../../../core/models/ChangePassword';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { loadTheme } from '../../../shared/utils/theme-utils';
import { Router } from '@angular/router';
import { TogglePassword } from "../../../shared/components/toggle-password/toggle-password";

@Component({
  selector: 'app-change-password',
  imports: [ReactiveFormsModule, TogglePassword],
  templateUrl: './change-password.html',
})
export class ChangePassword implements OnInit {
  changePasssword!: FormGroup;
  userId: string | null = null;
  role: string | null = null;
passwordInput: any;

  constructor(
    private router: Router,
    private userService: UserService,
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private snackBar: SnackbarService
  ) {}

  ngOnInit(): void {
    this.changePasssword = this.formBuilder.group(
      {
        currentPassword: ['', Validators.required],
        newPassword: ['', [Validators.required, passwordValidator]],
        confirmPassword: ['', Validators.required],
      },
      {
        validators: matchPassValidator,
      }
    );
    this.getUserIdFromAuth();
    loadTheme();
  }
  onSubmit() {
    const payloads: ChangePasswordModel = {
      userId: this.userId ?? '',
      currentPassword: this.changePasssword.get('currentPassword')?.value || '',
      newPassword: this.changePasssword.get('newPassword')?.value || '',
    };
    this.userService.changeUserPassword(payloads).subscribe({
      next: () => {
        this.snackBar.showSuccess('Password changed successfully');
        this.changePasssword.reset();
        this.router.navigate([`/${this.role}/dashboard`]);
      },
      error: (err) => {
        this.snackBar.showError('Failed to change the password');
      },
    });
  }

  getUserIdFromAuth() {
    this.authService.authme().subscribe({
      next: (res) => {
        this.userId = res.data.id;
        this.role = res.data.role.toLowerCase();
      },
      error: (err) => {
        console.log(err);
      },
    });
  }
}
