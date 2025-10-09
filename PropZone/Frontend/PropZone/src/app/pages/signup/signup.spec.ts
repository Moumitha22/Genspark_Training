import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { SignupComponent } from './signup';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';
import { of, throwError } from 'rxjs';
import { Router, ActivatedRoute } from '@angular/router';
import { By } from '@angular/platform-browser';
import { RegisterRequest } from '../../models/register-request.model';
import { RouterTestingModule } from '@angular/router/testing';

describe('SignupComponent', () => {
  let component: SignupComponent;
  let fixture: ComponentFixture<SignupComponent>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let notificationSpy: jasmine.SpyObj<NotificationService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
        imports: [
        ReactiveFormsModule,
        SignupComponent,
        RouterTestingModule.withRoutes([]),
        ],
        providers: [
        { provide: AuthService, useValue: jasmine.createSpyObj('AuthService', ['register']) },
        { provide: NotificationService, useValue: jasmine.createSpyObj('NotificationService', ['success', 'error']) },
        {
            provide: ActivatedRoute,
            useValue: {
            snapshot: {
                queryParamMap: {
                get: (key: string) => (key === 'reason' ? 'unauthorized' : null)
                }
            }
            }
        }
        ]
    }).compileComponents();

    fixture = TestBed.createComponent(SignupComponent);
    component = fixture.componentInstance;
    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    notificationSpy = TestBed.inject(NotificationService) as jasmine.SpyObj<NotificationService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;

    spyOn(routerSpy, 'navigate');

    fixture.detectChanges();
    });


  it('should create the signup component', () => {
    expect(component).toBeTruthy();
  });

  it('should show validation errors when form fields are touched and invalid', () => {
    component.registerForm.markAllAsTouched();
    fixture.detectChanges();

    const errors = fixture.debugElement.queryAll(By.css('.text-danger'));
    expect(errors.length).toBeGreaterThan(0);
  });

  it('should not submit if form is invalid', () => {
    component.registerForm.patchValue({ email: '', password: '', name: '' });
    component.onSubmit();

    expect(authServiceSpy.register).not.toHaveBeenCalled();
  });

  it('should register successfully and navigate to login', fakeAsync(() => {
    const payload: RegisterRequest = {
      name: 'Test User',
      email: 'test@example.com',
      password: 'password123',
      phoneNumber: '1234567890',
      role: 'Buyer'
    };

    component.registerForm.setValue(payload);
    authServiceSpy.register.and.returnValue(of({}));

    component.onSubmit();
    tick();

    expect(authServiceSpy.register).toHaveBeenCalledWith(payload);
    expect(notificationSpy.success).toHaveBeenCalledWith('Registration successful! You can now login.');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
    expect(component.errorMessage).toBe('');
  }));

  it('should handle registration failure and show error message', fakeAsync(() => {
    const payload: RegisterRequest = {
      name: 'Test User',
      email: 'test@example.com',
      password: 'password123',
      phoneNumber: '1234567890',
      role: 'Buyer'
    };

    const errorResponse = {
      error: {
        errors: {
          general: ['Email already exists']
        }
      }
    };

    component.registerForm.setValue(payload);
    authServiceSpy.register.and.returnValue(throwError(() => errorResponse));

    component.onSubmit();
    tick();

    expect(component.errorMessage).toBe('Email already exists');
    expect(notificationSpy.success).not.toHaveBeenCalled();
    expect(notificationSpy.error).not.toHaveBeenCalled(); // Optional
  }));
  
});
