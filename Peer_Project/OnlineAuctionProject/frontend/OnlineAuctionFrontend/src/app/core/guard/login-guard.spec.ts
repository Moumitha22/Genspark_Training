import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { LoginGuard } from './login-guard';
import { AuthService } from '../services/auth.service';

describe('LoginGuard', () => {
  let guard: LoginGuard;
  let authServiceSpy: any;
  let routerSpy: any;

  beforeEach(() => {
    authServiceSpy = jasmine.createSpyObj('AuthService', [
      'isAuthenticated',
      'getUserRole',
    ]);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        LoginGuard,
        { provide: AuthService, useValue: authServiceSpy },
        { provide: Router, useValue: routerSpy },
      ],
    });

    guard = TestBed.inject(LoginGuard);
  });

  it('should allow activation if not authenticated', () => {
    authServiceSpy.isAuthenticated.and.returnValue(false);
    expect(guard.canActivate()).toBeTrue();
  });

  it('should redirect Seller to seller dashboard', () => {
    authServiceSpy.isAuthenticated.and.returnValue(true);
    authServiceSpy.getUserRole.and.returnValue('Seller');
    expect(guard.canActivate()).toBeFalse();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/seller/dashboard']);
  });

  it('should redirect Admin to admin dashboard', () => {
    authServiceSpy.isAuthenticated.and.returnValue(true);
    authServiceSpy.getUserRole.and.returnValue('Admin');
    expect(guard.canActivate()).toBeFalse();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/admin/dashboard']);
  });

  it('should redirect Bidder to bidder dashboard', () => {
    authServiceSpy.isAuthenticated.and.returnValue(true);
    authServiceSpy.getUserRole.and.returnValue('Bidder');
    expect(guard.canActivate()).toBeFalse();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/bidder/dashboard']);
  });
});
