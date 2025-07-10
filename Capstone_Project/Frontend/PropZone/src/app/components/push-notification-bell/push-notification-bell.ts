import { Component } from '@angular/core';
import { PushNotificationService } from '../../core/services/push-notification.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-push-notification-bell',
  imports: [CommonModule],
  templateUrl: './push-notification-bell.html',
  styleUrl: './push-notification-bell.css'
})
export class PushNotificationBellComponent {
  dropdownOpen = false;

  constructor(public pushService: PushNotificationService) {}

  toggleDropdown(): void {
    this.dropdownOpen = !this.dropdownOpen;
  }

  removeNotification(index: number): void {
    this.pushService.removeNotification(index);
    if (this.pushService.notifications.length === 0) {
      this.dropdownOpen = false;
    }
  }


}
