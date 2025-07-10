import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { PropertyList } from './pages/property-list/property-list';
import { Navbar } from './components/navbar/navbar';
import { UserService } from './core/services/user.service';
import { Footer } from './components/footer/footer';
import { AuthService } from './core/services/auth.service';
import { PushNotificationService } from './core/services/push-notification.service';
import { combineLatest } from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, PropertyList, Navbar, RouterOutlet, Footer],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'PropZone';

  private userService = inject(UserService);
  private authService = inject(AuthService);
  private pushService = inject(PushNotificationService);


  ngOnInit(): void {
    this.userService.loadCurrentUser();

    combineLatest([
      this.authService.isLoggedIn$,
      this.authService.userRole$
    ]).subscribe(([loggedIn, role]) => {
      if (loggedIn && (role === 'Buyer' || role === 'Lister')) {
        this.pushService.startConnection();
      }
    });

  }


  toastVisible = false;
  toastMessage = '';

  showToast(message: string) {
    this.toastMessage = message;
    this.toastVisible = true;

    setTimeout(() => {
      this.toastVisible = false;
    }, 3000);
  }

  hideToast() {
    this.toastVisible = false;
  }

}