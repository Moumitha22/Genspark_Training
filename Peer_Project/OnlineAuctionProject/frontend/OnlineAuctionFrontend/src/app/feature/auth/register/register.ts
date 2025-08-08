import { Component, inject, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { matchPassValidator } from '../../../misc/validators/passwordValitor';
import { UserService } from '../../../core/services/user.service';
import { AddUser } from '../../../core/models/AddUser';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { Spinner } from '../../../shared/components/spinner/spinner';
import { Router, RouterLink } from '@angular/router';
import { TogglePassword } from '../../../shared/components/toggle-password/toggle-password';
import { UserAccountService } from '../../../core/services/userAccount.service';
import { ModelView } from '../../../shared/components/model-view/model-view';
import { setupLowercaseEmail } from '../../../shared/utils/function-utils';

@Component({
  selector: 'app-register',
  imports: [
    ReactiveFormsModule,
    Spinner,
    RouterLink,
    TogglePassword,
    ModelView,
  ],
  templateUrl: './register.html',
})
export class Register implements OnInit {
  userForm!: FormGroup;
  constructor(
    private formBuilder: FormBuilder,
    private userService: UserService,
    private snackBar: SnackbarService,
    private router: Router,
    private userAccountService: UserAccountService
  ) {}
  isLoading = false;
  showAdminInfoModel = false;

  ngOnInit(): void {
    this.userForm = this.formBuilder.group(
      {
        username: ['', [Validators.required]],
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required]],
        confirmPassword: ['', Validators.required],
        role: ['', Validators.required],
      },
      {
        validators: matchPassValidator,
      }
    );
    setupLowercaseEmail(this.userForm, 'email');
  }

  onFormSubmit() {
    if (this.userForm.valid) {
      const { confirmPassword, ...payload } = this.userForm.value;
      this.isLoading = true;

      this.userAccountService.getDeleteReasonByEmail(payload.email).subscribe({
        next: (res) => {
          if (res?.data) {
            this.isLoading = false;
            this.snackBar.showError(
              'This user was previously deleted. Please contact admin to revoke the deletion.'
            );
            this.showAdminInfoModel = true;
          } else {
            if (payload.role === 'Admin') {
              this.snackBar.showError('Admin registration is not allowed.');
              this.isLoading = false;
              return;
            }
            this.userService.registerNewuser(payload).subscribe({
              next: () => {
                setTimeout(() => {
                  this.snackBar.showSuccess('New User Created');
                  this.isLoading = false;
                  this.userForm.reset();
                  this.router.navigate(['/login']);
                }, 2000);
              },
              error: (err) => {
                this.isLoading = false;
                this.snackBar.showError(`Failed to create - ${err.message}`);
              },
            });
          }
        },
        error: (err) => {
          if (err.status === 404) {
            if (payload.role === 'Admin') {
              this.snackBar.showError('Admin registration is not allowed.');
              this.isLoading = false;
              return;
            }
            this.userService.registerNewuser(payload).subscribe({
              next: () => {
                setTimeout(() => {
                  this.snackBar.showSuccess('New User Created');
                  this.isLoading = false;
                  this.userForm.reset();
                  this.router.navigate(['/login']);
                }, 2000);
              },
              error: (err) => {
                this.isLoading = false;
                this.snackBar.showError(`Failed to create - ${err.message}`);
              },
            });
          } else {
            this.isLoading = false;
            this.snackBar.showError(
              `Error checking user status. - ${err.message}`
            );
          }
        },
      });
    } else {
      this.userForm.markAllAsTouched();
    }
  }
}
