import { TestBed } from '@angular/core/testing';
import { LoginSuperAdmin } from './login-super-admin';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { UserService } from '../../core/services/user.service';
import { SnackbarService } from '../../core/services/snackbar.service';
import { of } from 'rxjs';

describe('LoginSuperAdmin', () => {
  let component: LoginSuperAdmin;
  let userServiceSpy: jasmine.SpyObj<UserService>;

  beforeEach(() => {
    userServiceSpy = jasmine.createSpyObj('UserService', ['loginSuperAdmin', 'registerAdmin']);
    TestBed.configureTestingModule({
      imports: [ReactiveFormsModule],
      providers: [
        FormBuilder,
        { provide: UserService, useValue: userServiceSpy },
        { provide: SnackbarService, useValue: { showError: () => {} } }
      ]
    });
    const fb = TestBed.inject(FormBuilder);
    component = new LoginSuperAdmin(fb, userServiceSpy, TestBed.inject(SnackbarService));
  });

  it('should create forms', () => {
    expect(component.userForm).toBeTruthy();
    expect(component.adminForm).toBeTruthy();
  });

  it('should show admin register modal after successful login', () => {
    userServiceSpy.loginSuperAdmin.and.returnValue(of({}));
    component.userForm.setValue({ email: 'test@test.com', password: '123456' });
    component.onSubmit();
    expect(component.showAdminRegisterModal).toBeTrue();
  });
});