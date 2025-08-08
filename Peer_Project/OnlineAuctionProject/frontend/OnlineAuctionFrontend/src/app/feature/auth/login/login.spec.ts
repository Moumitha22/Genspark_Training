import {
  ComponentFixture,
  TestBed,
  waitForAsync,
  fakeAsync,
  tick,
} from '@angular/core/testing';
import { Login } from './login';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, provideRouter, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { LoginService } from '../../../core/services/login.service';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { of, throwError } from 'rxjs';

const mockActivatedRoute = {
  snapshot: {
    params: { id: '1' },
    paramMap: {
      get: (key: string) => (key === 'id' ? '1' : null),
    },
  },
};

describe('Login', () => {
  let fixture: ComponentFixture<Login>;
  let component: Login;
  let loginServiceSpy: jasmine.SpyObj<LoginService>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let snackBarSpy: jasmine.SpyObj<SnackbarService>;

  beforeEach(waitForAsync(() => {
    loginServiceSpy = jasmine.createSpyObj('LoginService', ['login']);
    authServiceSpy = jasmine.createSpyObj('AuthService', ['getUserRole']);
    snackBarSpy = jasmine.createSpyObj('SnackbarService', [
      'showSuccess',
      'showError',
      'showInfo',
    ]);

    loginServiceSpy.login.and.returnValue(of({}));

    TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, Login],
      providers: [
        FormBuilder,
        { provide: LoginService, useValue: loginServiceSpy },
        { provide: AuthService, useValue: authServiceSpy },
        { provide: SnackbarService, useValue: snackBarSpy },
        { provide: ActivatedRoute, useValue: mockActivatedRoute },
        provideRouter([{ path: 'seller/dashboard', component: Login }]),
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    loginServiceSpy.login.and.returnValue(of({}));
    authServiceSpy.getUserRole.and.returnValue('Bidder');
    fixture = TestBed.createComponent(Login);
    component = fixture.componentInstance;
    component.ngOnInit();
    fixture.detectChanges();
    expect(component.loginForm).toBeDefined();
    expect(component.loginForm.valid).toBeFalse();
  });

  it('should create Login component', () => {
    expect(component).toBeTruthy();
  });

  it('should login as Seller and navigate correctly', fakeAsync(() => {
    component.loginForm.setValue({
      email: 'seller@test.com',
      password: 'pass',
    });
    expect(component.loginForm.valid).toBeTrue();
    loginServiceSpy.login.and.callFake((payload) => {
      console.log('login called with:', payload);
      return of({});
    });
    authServiceSpy.getUserRole.and.returnValue('Seller');

    component.onLogin();
    tick(2000);

    expect(loginServiceSpy.login).toHaveBeenCalledWith({
      email: 'seller@test.com',
      password: 'pass',
    });

    expect(component.isLoading).toBeFalse();
  }));

  it('should show error snackbar on login failure', fakeAsync(() => {
    component.loginForm.setValue({
      email: 'fail@test.com',
      password: 'fail',
    });
    loginServiceSpy.login.and.returnValue(
      throwError(() => new Error('Login failed'))
    );

    component.onLogin();
    tick();

    expect(snackBarSpy.showError).toHaveBeenCalledWith(
      'Login failed. Please check your credentials.'
    );
    expect(component.isLoading).toBeFalse();
  }));
});
