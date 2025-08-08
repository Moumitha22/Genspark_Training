import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { AuthGuard } from './auth-guard';
import { AuthService } from '../services/auth.service';

describe('AuthGuard', () => {
  let guard: AuthGuard;
  let authServiceSpy: any;
  let routerSpy: any;

  beforeEach(() => {
    authServiceSpy = jasmine.createSpyObj('AuthService', [
      'getToken',
      'isTokenValid',
      'getUserRole',
      'getAccessTokenByRefreshToken',
      'clearToken',
    ]);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        AuthGuard,
        { provide: AuthService, useValue: authServiceSpy },
        { provide: Router, useValue: routerSpy },
      ],
    });

    guard = TestBed.inject(AuthGuard);
  });

  it('should redirect to /login if no token', (done) => {
    authServiceSpy.getToken.and.returnValue(null);

    guard.canActivate({ data: {} } as any).subscribe((result) => {
      expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
      expect(result).toBeFalse();
      done();
    });
  });

  it('should allow if token is valid and no roles required', (done) => {
    authServiceSpy.getToken.and.returnValue('token');
    authServiceSpy.isTokenValid.and.returnValue(true);
    authServiceSpy.getUserRole.and.returnValue('user');

    guard.canActivate({ data: {} } as any).subscribe((result) => {
      expect(result).toBeTrue();
      done();
    });
  });

  it('should redirect to /unauthorized if role does not match', (done) => {
    authServiceSpy.getToken.and.returnValue('token');
    authServiceSpy.isTokenValid.and.returnValue(true);
    authServiceSpy.getUserRole.and.returnValue('user');

    guard
      .canActivate({ data: { roles: ['admin'] } } as any)
      .subscribe((result) => {
        expect(routerSpy.navigate).toHaveBeenCalledWith(['/unauthorized']);
        expect(result).toBeFalse();
        done();
      });
  });

  it('should try refresh if token invalid and succeed', (done) => {
    authServiceSpy.getToken.and.returnValue('token');
    authServiceSpy.isTokenValid.and.returnValue(false);
    authServiceSpy.getAccessTokenByRefreshToken.and.returnValue(of(true));
    authServiceSpy.getUserRole.and.returnValue('admin');

    guard
      .canActivate({ data: { roles: ['admin'] } } as any)
      .subscribe((result) => {
        expect(result).toBeTrue();
        done();
      });
  });

  it('should clear token and redirect to /login if refresh fails', (done) => {
    authServiceSpy.getToken.and.returnValue('token');
    authServiceSpy.isTokenValid.and.returnValue(false);
    authServiceSpy.getAccessTokenByRefreshToken.and.returnValue(
      throwError(() => new Error('fail'))
    );

    guard.canActivate({ data: {} } as any).subscribe((result) => {
      expect(authServiceSpy.clearToken).toHaveBeenCalled();
      expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
      expect(result).toBeFalse();
      done();
    });
  });
});
