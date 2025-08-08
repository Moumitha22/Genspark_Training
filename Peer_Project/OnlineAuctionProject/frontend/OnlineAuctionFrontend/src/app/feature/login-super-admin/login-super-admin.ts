import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { UserService } from '../../core/services/user.service';
import { ModelView } from '../../shared/components/model-view/model-view';
import { SnackbarService } from '../../core/services/snackbar.service';
import { Spinner } from '../../shared/components/spinner/spinner';
import { TogglePassword } from '../../shared/components/toggle-password/toggle-password';
import { AddUser } from '../../core/models/AddUser';
import { setupLowercaseEmail } from '../../shared/utils/function-utils';

@Component({
  selector: 'app-login-super-admin',
  imports: [ReactiveFormsModule, ModelView, Spinner, TogglePassword],
  templateUrl: './login-super-admin.html',
})
export class LoginSuperAdmin {
  userForm: FormGroup;
  adminForm: FormGroup;
  showAdminRegisterModal = false;
  isLoading = false;
  defaultPassword = '';
  constructor(
    private formBuilder: FormBuilder,
    private userService: UserService,
    private snackBar: SnackbarService
  ) {
    this.adminForm = this.formBuilder.group({
      username: [''],
      email: [''],
      role: [{ value: 'Admin', disabled: true }],
      password: [{ value: this.defaultPassword, disabled: true }],
    });

    setupLowercaseEmail(this.adminForm, 'email');

    this.userForm = this.formBuilder.group({
      email: [''],
      password: [''],
    });
    setupLowercaseEmail(this.userForm, 'email');
    this.adminForm
      .get('username')
      ?.valueChanges.subscribe((username: string) => {
        if (username && username.length >= 2) {
          this.defaultPassword = username.substring(0, 2) + '@123';
          this.adminForm.get('password')?.setValue(this.defaultPassword);
        } else {
          this.defaultPassword = '';
          this.adminForm.get('password')?.setValue('');
        }
      });
  }

  ngOnInit() {
    window.addEventListener('beforeunload', this.beforeUnloadHandler);
  }

  ngOnDestroy(): void {
    window.removeEventListener('beforeunload', this.beforeUnloadHandler);
  }
  beforeUnloadHandler = (event: BeforeUnloadEvent) => {
    if (this.showAdminRegisterModal) {
      event.preventDefault();
    }
  };

  onSubmit() {
    this.isLoading = true;
    const userData = this.userForm.value;
    this.userService.loginSuperAdmin(userData).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.showAdminRegisterModal = true;
        window.onbeforeunload = () => {};
      },
      error: (error) => {
        this.isLoading = false;
        this.snackBar.showError('Login failed. Please check your credentials.');
        console.error('Error registering user', error);
      },
    });
  }
  onAdminRegister() {
    const raw = this.adminForm.getRawValue();
    const adminData = {
      userName: raw.username,
      email: raw.email,
      password: raw.password,
      role: 'Admin',
    };
    if (!adminData.userName || adminData.userName.length < 2) {
      this.snackBar.showError('Username must be at least 2 characters.');
      this.isLoading = false;
      return;
    }
    if (!adminData.password) {
      this.snackBar.showError('Password cannot be empty.');
      this.isLoading = false;
      return;
    }
    this.isLoading = true;
    this.userService.registerAdmin(adminData).subscribe({
      next: (response) => {
        this.showAdminRegisterModal = true;
        this.snackBar.showSuccess('Admin registered successfully.');
        this.adminForm.reset();
        this.defaultPassword = '';
        this.isLoading = false;
      },
      error: (error) => {
        this.snackBar.showError('Admin registration failed. Please try again.');
        console.error('Error registering admin', error.message);
        this.isLoading = false;
      },
    });
  }
}
