import { Component, HostListener, OnInit, inject } from '@angular/core';
import { Router, NavigationEnd, RouterModule } from '@angular/router';
import { filter } from 'rxjs/operators';
import { AuthService } from '../../core/services/auth.service';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { UserService } from '../../core/services/user.service';
import { PushNotificationBellComponent } from '../push-notification-bell/push-notification-bell';
import { PushNotificationService } from '../../core/services/push-notification.service';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, CommonModule, PushNotificationBellComponent, RouterModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class Navbar implements OnInit {
  private router = inject(Router);
  private authService = inject(AuthService);
  private userService = inject(UserService);
  private pushNotificationService = inject(PushNotificationService);

  isLoggedIn$ = this.authService.isLoggedIn$;
  userRole$ = this.authService.userRole$;

  isLandingPage = true;
  isScrolled = false;
  showNavbar = true;
  userName: string = '';

  ngOnInit(): void {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      const url = this.router.url;

      this.isLandingPage = url === '/' || url.startsWith('/landing') || url.startsWith('/login') || url.startsWith('/signup');
      this.checkNavbarState();
    });

    this.checkNavbarState();

    this.userService.user$.subscribe(user => {
      if (user) {
        this.userName = user.name;
      }
    });
  }


  @HostListener('window:scroll', [])
  onWindowScroll() {
    this.checkNavbarState();
  }

  private checkNavbarState() {
    const scrollTop = window.scrollY || document.documentElement.scrollTop;
    this.isScrolled = scrollTop > 200;
  }

  get navbarClasses() {
    return {
      'fixed-top': this.isLandingPage,
      'sticky-top': !this.isLandingPage,
      'scrolled': !this.isLandingPage || this.isScrolled,
      'navbar': true,
      'navbar-expand-lg': true,
      'shadow': true
    };
  }

 logout(): void {
    this.authService.setLoggingOut(true);

    this.authService.logout().subscribe({
      next: () => {
        this.pushNotificationService.stopConnection(); 
        this.authService.clearLocalState();
        this.router.navigate(['/login']);
      },
      error: err => console.error('Logout failed', err),
      complete: () => this.authService.setLoggingOut(false)
    });
  }
}

