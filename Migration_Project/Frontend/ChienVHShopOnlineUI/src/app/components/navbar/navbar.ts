// import { Component, HostListener, OnInit, inject } from '@angular/core';
// import { Router, NavigationEnd } from '@angular/router';
// import { filter } from 'rxjs/operators';
// import { CommonModule } from '@angular/common';
// import { RouterLink } from '@angular/router';
// import { CartService } from '../../services/cart.service';

// @Component({
//   selector: 'app-navbar',
//   imports: [RouterLink, CommonModule],
//   templateUrl: './navbar.html',
//   styleUrl: './navbar.css',
// })
// export class Navbar implements OnInit {
//   private cartService = inject(CartService);
//   cartCount = 0;

//   ngOnInit(): void {
//     this.cartService.cartCount$.subscribe((count) => {
//       this.cartCount = count;
//     });
//   }
// }

import { Component, HostListener, OnInit, inject } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, CommonModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class Navbar implements OnInit {
  private router = inject(Router);
  private authService = inject(AuthService);
  private userService = inject(UserService);
  private cartService = inject(CartService);

  isLoggedIn$ = this.authService.isLoggedIn$;
  userRole$ = this.authService.userRole$;
  cartCount = 0;
  userName: string = '';

  ngOnInit(): void {
    this.userService.user$.subscribe(user => {
      if (user) {
        this.userName = user.username;
      }
    });

    this.cartService.cartCount$.subscribe((count) => {
      this.cartCount = count;
    });
  }

  logout(): void {
    console.log('Logout clicked');
    this.authService.setLoggingOut(true);

    this.authService.logout().subscribe({
      next: () => {
        console.log('Logout subscribed');
        this.authService.clearLocalState();

        this.cartService.clearCart().subscribe({
          next: () => console.log('Cart cleared'),
          error: err => console.error('Failed to clear cart', err),
        });

        this.router.navigate(['/login']);
      },
      error: err => {
        console.error('Logout failed', err);
        this.authService.setLoggingOut(false);
      },
      complete: () => this.authService.setLoggingOut(false)
    });
  }

}

