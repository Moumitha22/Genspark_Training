import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { NotificationComponent } from './notification';
import { NotificationService } from '../../core/services/notify.service';
import { AuthService } from '../../core/services/auth.service';
import { of, Subject } from 'rxjs';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';

describe('NotificationComponent', () => {
  let component: NotificationComponent;
  let fixture: ComponentFixture<NotificationComponent>;
  let notificationServiceSpy: jasmine.SpyObj<NotificationService>;

  let bidPlacedSubject: Subject<any>;
  let auctionStatusSubject: Subject<any>;
  let auctionCreatedSubject: Subject<any>;
  let winningBidSubject: Subject<any>;

  beforeEach(waitForAsync(() => {
    bidPlacedSubject = new Subject<any>();
    auctionStatusSubject = new Subject<any>();
    auctionCreatedSubject = new Subject<any>();
    winningBidSubject = new Subject<any>();

    notificationServiceSpy = jasmine.createSpyObj(
      'NotificationService',
      ['startSignalRConnection'],
      {
        bidPlaced$: bidPlacedSubject.asObservable(),
        auctionStatus$: auctionStatusSubject.asObservable(),
        auctionCreated$: auctionCreatedSubject.asObservable(),
        winningBid$: winningBidSubject.asObservable(),
      }
    );

    TestBed.configureTestingModule({
      imports: [NotificationComponent, CommonModule, LucideAngularModule],
      providers: [
        { provide: NotificationService, useValue: notificationServiceSpy },
        AuthService,
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NotificationComponent);
    component = fixture.componentInstance;
    component.notifications = [];
    fixture.detectChanges();
  });

  it('should create the NotificationComponent', () => {
    expect(component).toBeTruthy();
  });

  it('should add a new bid notification when bidPlaced$ emits', () => {
    bidPlacedSubject.next({ amount: 1000 });
    expect(component.notifications.length).toBe(1);
    expect(component.notifications[0].type).toBe('bid');
  });

  it('should add a new status notification when auctionStatus$ emits', () => {
    auctionStatusSubject.next({ status: 'Completed' });
    expect(component.notifications.length).toBe(1);
    expect(component.notifications[0].type).toBe('status');
  });

  it('should add a new auction notification when auctionCreated$ emits', () => {
    auctionCreatedSubject.next({ name: 'New Auction' });
    expect(component.notifications.length).toBe(1);
    expect(component.notifications[0].type).toBe('newAuction');
  });

  it('should add a winning bid notification when winningBid$ emits', () => {
    winningBidSubject.next({ winnerId: 'abc123' });
    expect(component.notifications.length).toBe(1);
    expect(component.notifications[0].type).toBe('winningBidUpdate');
  });

  it('should toggle panel visibility', () => {
    expect(component.panelOpen).toBe(false);
    component.togglePanel();
    expect(component.panelOpen).toBe(true);
  });

  it('should mark all notifications as seen', () => {
    component.notifications = [
      { seen: false, type: 'bid' },
      { seen: false, type: 'status' },
    ];
    component.markAllAsSeen();
    expect(component.notifications.every((n) => n.seen)).toBe(true);
    expect(component.unseenCount).toBe(0);
  });

  it('should dismiss a notification', () => {
    const notif = { type: 'bid', seen: false };
    component.notifications = [notif];
    component.dismissNotification(notif);
    expect(component.notifications.length).toBe(0);
  });
});
