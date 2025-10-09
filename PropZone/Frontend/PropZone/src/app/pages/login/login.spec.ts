import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { LoginComponent } from './login';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';
import { By } from '@angular/platform-browser';
import { LoginRequest} from '../../models/login-request.model';
import { LoginResponse } from '../../models/login-response.model';
import { RouterTestingModule } from '@angular/router/testing';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let notificationSpy: jasmine.SpyObj<NotificationService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
        imports: [
          ReactiveFormsModule,
          LoginComponent,
          RouterTestingModule.withRoutes([])  
        ],
        providers: [
        { provide: AuthService, useValue: jasmine.createSpyObj('AuthService', ['login']) },
        { provide: NotificationService, useValue: jasmine.createSpyObj('NotificationService', ['success', 'error', 'warning']) },
        {
            provide: ActivatedRoute,
            useValue: {
            snapshot: {
                queryParamMap: {
                get: (key: string) => {
                    if (key === 'returnUrl') return '/dashboard';
                    if (key === 'reason') return null;
                    return null;
                }
                }
            }
            }
        }
        ]
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;

    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    notificationSpy = TestBed.inject(NotificationService) as jasmine.SpyObj<NotificationService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    spyOn(routerSpy, 'navigate'); 

    fixture.detectChanges();
    });

  it('should show validation error when form is touched and invalid', () => {
    component.loginForm.markAllAsTouched();
    fixture.detectChanges();

    const errors = fixture.debugElement.queryAll(By.css('.text-danger'));
    expect(errors.length).toBeGreaterThan(0);
  });

  it('should not submit if no role selected for Buyer/Lister login', () => {
    component.isAdminLogin = false;
    component.selectedRole = null;
    component.loginForm.setValue({ email: 'test@example.com', password: '123456' });

    component.onSubmit();
    expect(component.errorMessage).toBe('Please select a role to continue.');
  });

  it('should call login and navigate to home page on successful Buyer login', fakeAsync(() => {
    const payload: LoginRequest = {
      email: 'buyer@example.com',
      password: 'pass123',
      role: 'Buyer'
    };

   
    const response: LoginResponse = {
        username: 'Buyer User',
        accessToken: 'token123'
    };

    component.selectedRole = 'Buyer';
    component.loginForm.setValue({ email: payload.email, password: payload.password });


    authServiceSpy.login.and.returnValue(of(response)); 


    component.onSubmit();
    tick();

    expect(authServiceSpy.login).toHaveBeenCalledWith(payload);
    expect(notificationSpy.success).toHaveBeenCalledWith('Logged in successfully');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/']);
  }));

  it('should call login and navigate to lister dashboard on successful Lister login', fakeAsync(() => {
    const payload: LoginRequest = {
        email: 'lister@example.com',
        password: 'lister123',
        role: 'Lister'
    };

    const response: LoginResponse = {
        username: 'Lister User',
        accessToken: 'token456'
    };

    component.selectedRole = 'Lister';
    component.loginForm.setValue({ email: payload.email, password: payload.password });

    authServiceSpy.login.and.returnValue(of(response));

    component.onSubmit();
    tick();

    expect(authServiceSpy.login).toHaveBeenCalledWith(payload);
    expect(notificationSpy.success).toHaveBeenCalledWith('Logged in successfully');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/lister/dashboard']);
    }));


  it('should call login and navigate to admin dashboard on Admin login', fakeAsync(() => {
    const payload: LoginRequest = {
      email: 'admin@example.com',
      password: 'admin123',
      role: 'Admin'
    };

     const response: LoginResponse = {
        username: 'Buyer User',
        accessToken: 'token123'
    };

    component.isAdminLogin = true;
    component.selectedRole = 'Admin';
    component.loginForm.setValue({ email: payload.email, password: payload.password });

    authServiceSpy.login.and.returnValue(of(response));

    component.onSubmit();
    tick();

    expect(authServiceSpy.login).toHaveBeenCalledWith(payload);
    expect(notificationSpy.success).toHaveBeenCalledWith('Logged in successfully');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/admin/dashboard']);
  }));

  it('should handle login failure', fakeAsync(() => {
    const payload: LoginRequest = {
      email: 'wrong@example.com',
      password: 'wrongpass',
      role: 'Buyer'
    };

    const errorResponse = {
      error: {
        errors: {
          general: ['Invalid credentials']
        }
      }
    };

    component.selectedRole = 'Buyer';
    component.loginForm.setValue({ email: payload.email, password: payload.password });

    authServiceSpy.login.and.returnValue(throwError(() => errorResponse));

    component.onSubmit();
    tick();

    expect(component.errorMessage).toBe('Invalid credentials');
    expect(notificationSpy.success).not.toHaveBeenCalled();
  }));
});
