import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { AuthService } from '../services/auth.service';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

@Injectable()
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot): Observable<boolean> {
    const token = this.authService.getToken('access_token');
    if (!token) {
      this.router.navigate(['/login']);
      return of(false);
    }
    try {
      if (this.authService.isTokenValid(token)) {
        const userRole = this.authService.getUserRole();
        const requiredRoles = route.data['roles'];
        if (requiredRoles && !requiredRoles.includes(userRole)) {
          this.router.navigate(['/unauthorized']);
          return of(false);
        }

        return of(true);
      }
      return this.authService.getAccessTokenByRefreshToken().pipe(
        map(() => {
          const userRole = this.authService.getUserRole();
          const requiredRoles = route.data['roles'] as string[];

          if (requiredRoles && !requiredRoles.includes(userRole!)) {
            this.router.navigate(['/unauthorized']);
            return false;
          }

          return true;
        }),
        catchError(() => {
          this.authService.clearToken();
          this.router.navigate(['/login']);
          return of(false);
        })
      );
    } catch {
      this.authService.clearToken();
      this.router.navigate(['/login']);
      return of(false);
    }
  }
}
