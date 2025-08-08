import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { SellerDashboard } from './seller-dashboard';
import { AuctionService } from '../../../core/services/auction.service';
import { of, throwError } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { UserService } from '../../../core/services/user.service';

describe('SellerDashboard', () => {
  let component: SellerDashboard;
  let fixture: ComponentFixture<SellerDashboard>;
  let auctionServiceSpy: jasmine.SpyObj<AuctionService>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let userServiceSpy: jasmine.SpyObj<UserService>;

  const mockAuctions = {
    data: {
      $values: [
        {
          id: 1,
          status: 'Upcoming',
          endTime: new Date(Date.now() + 86400000).toISOString(),
          createdAt: '2025-01-01',
        },
        {
          id: 2,
          status: 'Live',
          endTime: new Date(Date.now() + 172800000).toISOString(),
          createdAt: '2025-01-02',
        },
        {
          id: 3,
          status: 'Closed',
          endTime: new Date(Date.now() - 86400000).toISOString(),
          createdAt: '2025-01-03',
        },
      ],
    },
  };

  beforeEach(waitForAsync(() => {
    auctionServiceSpy = jasmine.createSpyObj('AuctionService', [
      'getAuctionBySeller',
    ]);
    authServiceSpy = jasmine.createSpyObj('AuthService', ['getEmailFromToken']);
    userServiceSpy = jasmine.createSpyObj('UserService', [
      'getCurrentUser',
      'getByEmail',
    ]);
    TestBed.configureTestingModule({
      imports: [SellerDashboard],
      providers: [
        { provide: AuctionService, useValue: auctionServiceSpy },
        { provide: AuthService, useValue: authServiceSpy },
        { provide: UserService, useValue: userServiceSpy },
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    auctionServiceSpy.getAuctionBySeller.and.returnValue(of(mockAuctions));
    authServiceSpy.getEmailFromToken.and.returnValue('test@gmail.com');
    userServiceSpy.getByEmail.and.returnValue(
      of({ data: { email: 'test@gmail.com' } })
    );
    fixture = TestBed.createComponent(SellerDashboard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch auctions and set chart options on init', () => {
    expect(auctionServiceSpy.getAuctionBySeller).toHaveBeenCalled();
    expect(component.auctionItems.length).toBe(3);
    expect(component.statusChartOptions).toBeTruthy();
    expect(component.monthlyChartOptions).toBeTruthy();
    expect(component.endingSoonAuctions.length).toBe(2);
    expect(component.pagedAuctions.length).toBeLessThanOrEqual(
      component.pageSize
    );
  });

  it('should handle error and set empty auctions', () => {
    auctionServiceSpy.getAuctionBySeller.and.returnValue(
      throwError(() => new Error('fail'))
    );
    component.ngOnInit();
    expect(component.auctionItems.length).toBe(0);
    expect(component.statusChartOptions).toBeTruthy();
    expect(component.monthlyChartOptions).toBeTruthy();
  });

  it('should paginate ending soon auctions', () => {
    component.endingSoonAuctions = Array.from({ length: 12 }, (_, i) => ({
      id: i + 1,
      endTime: new Date(Date.now() + 86400000).toISOString(),
    }));
    component.pageSize = 5;
    component.currentPage = 1;
    component.updatePagedData();
    expect(component.pagedAuctions.length).toBe(5);
    component.goToNextPage();
    expect(component.currentPage).toBe(2);
    expect(component.pagedAuctions.length).toBe(5);
    component.goToNextPage();
    expect(component.currentPage).toBe(3);
    expect(component.pagedAuctions.length).toBe(2);
    component.goToPreviousPage();
    expect(component.currentPage).toBe(2);
  });

  it('should return correct auction status', () => {
    expect(component.getAuctionStatus({ status: 'Upcoming' })).toBe('Upcoming');
    expect(component.getAuctionStatus({ status: 'Live' })).toBe('Live');
    expect(component.getAuctionStatus({ status: 'Closed' })).toBe('Closed');
    expect(component.getAuctionStatus({ status: 'Completed' })).toBe(
      'Completed'
    );
    expect(component.getAuctionStatus({ status: 'Cancelled' })).toBe(
      'Cancelled'
    );
    expect(component.getAuctionStatus({ status: 'Other' })).toBe('Unknown');
  });
});
