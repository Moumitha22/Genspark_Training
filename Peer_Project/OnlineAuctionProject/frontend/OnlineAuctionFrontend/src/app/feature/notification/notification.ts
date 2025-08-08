import {
  Component,
  ElementRef,
  HostListener,
  OnInit,
  ViewChild,
  viewChild,
} from '@angular/core';
import { NotificationService } from '../../core/services/notify.service';
import {
  BellIcon,
  Calendar,
  Delete,
  DeleteIcon,
  LucideAngularModule,
  XCircleIcon,
} from 'lucide-angular';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-notification',
  templateUrl: './notification.html',
  imports: [LucideAngularModule, CommonModule],
})
export class NotificationComponent implements OnInit {
  readonly bellIcon = BellIcon;
  readonly closeIcon = XCircleIcon;
  readonly dismissIcon = DeleteIcon;
  readonly calendarIcon = Calendar;
  notifications: any[] = [];
  unseenCount = 0;
  panelOpen = false;

  constructor(private notificationService: NotificationService) {}

  ngOnInit() {
    this.notificationService.startSignalRConnection();
    this.bidsPlacedNotification();
    this.auctionStatusUpdateNotification();
    this.auctionCreatedNotification();
    this.winningUpdateNotification();
  }

  bidsPlacedNotification() {
    this.notificationService.bidPlaced$.subscribe((bid) => {
      this.notifications.unshift({
        ...bid,
        seen: false,
        type: 'bid',
      });
      this.unseenCount++;
    });
  }

  auctionStatusUpdateNotification() {
    this.notificationService.auctionStatus$.subscribe((status) => {
      this.notifications.unshift({
        ...status,
        seen: false,
        type: 'status',
      });
      this.unseenCount++;
    });
  }

  auctionCreatedNotification() {
    this.notificationService.auctionCreated$.subscribe((auction) => {
      this.notifications.unshift({
        ...auction,
        seen: false,
        type: 'newAuction',
      });
      this.unseenCount++;
    });
  }

  winningUpdateNotification() {
    this.notificationService.winningBid$.subscribe((winningBid) => {
      this.notifications.unshift({
        ...winningBid,
        seen: false,
        type: 'winningBidUpdate',
      });
      this.unseenCount++;
    });
  }

  togglePanel() {
    this.panelOpen = !this.panelOpen;
    if (this.panelOpen) {
      this.markAllAsSeen();
    }
  }

  markAllAsSeen() {
    this.notifications.forEach((n) => (n.seen = true));
    this.unseenCount = 0;
  }
  dismissNotification(notification: any) {
    this.notifications = this.notifications.filter((n) => n !== notification);
  }

  @HostListener('window:keydown.escape')
  closePanel() {
    if (this.panelOpen) {
      this.panelOpen = false;
    }
  }
}
