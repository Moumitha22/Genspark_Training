
import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router, UrlTree } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({ providedIn: 'root' })
export class RoleGuard implements CanActivate {
  constructor(private auth: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree {
    const expectedRoles: string[] = route.data['expectedRoles'] ?? [];
    const user = this.auth.currentUser;

    if (!user) {
      return this.router.createUrlTree(['/login'], {
        queryParams: { returnUrl: state.url, reason: 'notAuthenticated' }
      });
    }

    if (expectedRoles.length && !expectedRoles.includes(user.role)) {
      return this.router.createUrlTree(['/login'], {
        queryParams: {
          returnUrl: state.url,
          reason: 'invalidRole',
          required: expectedRoles.join(', '),
          actual: user.role
        }
      });
    }

    return true;
  }
}
