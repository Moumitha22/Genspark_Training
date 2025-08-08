import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpErrorResponse
} from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private router = inject(Router);

  constructor(private authService: AuthService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // const isExternal = req.url.startsWith('https://nominatim.openstreetmap.org');

    // if (isExternal) {
    //   return next.handle(req);
    // }
  
    const token = this.authService.getAccessToken();
    let clonedReq = req;

    if (token) {
      console.log('[AuthInterceptor] Attaching access token');
      clonedReq = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        },
        withCredentials: true
      });
    } else {
      clonedReq = req.clone({ withCredentials: true });
    }

    return next.handle(clonedReq).pipe(
      catchError(err => {
        if (err instanceof HttpErrorResponse && err.status === 401 && !this.isRefreshing &&
            !this.authService.isLoggingOut() && this.authService.getAccessToken()) {
          console.warn('[AuthInterceptor] Access token expired. Attempting refresh...');
          this.isRefreshing = true;

          return this.authService.refreshToken().pipe(
            switchMap(() => {
              const newAccessToken = this.authService.getAccessToken(); 
              console.log('[AuthInterceptor] Refresh successful. Retrying request with new token:');
              
              this.isRefreshing = false;

              const newReq = req.clone({
                  setHeaders: {
                  Authorization: `Bearer ${newAccessToken}`
                  },
                  withCredentials: true
              });
              return next.handle(newReq);
            }),

            catchError(refreshErr => {
              console.error('[AuthInterceptor] Refresh token failed.', refreshErr);
              this.isRefreshing = false;
              this.authService.logout(); 
              this.router.navigate(['/login']);
              return throwError(() => refreshErr);
            })
          );
        }

        return throwError(() => err);
      })
    );
  }

}
