import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class LoginGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): boolean {
    if (this.authService.isAuthenticated()) {
      const role = this.authService.getUserRole();
      switch (role) {
        case 'Seller':
          this.router.navigate(['/seller/dashboard']);
          break;
        case 'Admin':
          this.router.navigate(['/admin/dashboard']);
          break;
        case 'Bidder':
          this.router.navigate(['/bidder/dashboard']);
          break;
      }
      return false;
    }
    return true;
  }
}
