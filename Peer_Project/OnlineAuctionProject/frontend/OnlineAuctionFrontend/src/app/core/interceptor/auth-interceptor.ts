import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (req.url.includes('/Authentication/refresh')) {
    return next(req);
  }

  const accessToken = authService.getToken('access_token');
  const cloned = req.clone({
    setHeaders: { Authorization: `Bearer ${accessToken}` },
  });
  return next(cloned);
};
