import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { BidderDashboard } from './bidder-dashboard';
import { AuctionService } from '../../../core/services/auction.service';
import { BiddingService } from '../../../core/services/bidding.service';
import { AuthService } from '../../../core/services/auth.service';
import { of, throwError } from 'rxjs';
import { UserService } from '../../../core/services/user.service';
import { WalletService } from '../../../core/services/wallet.service';

describe('BidderDashboard', () => {
  let component: BidderDashboard;
  let fixture: ComponentFixture<BidderDashboard>;

  let auctionServiceSpy: jasmine.SpyObj<AuctionService>;
  let biddingServiceSpy: jasmine.SpyObj<BiddingService>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let walletServiceSpy: jasmine.SpyObj<WalletService>;

  const mockAuctions = {
    data: {
      $values: [
        { id: 1, createdAt: '2025-01-01', status: 'Upcoming' },
        { id: 2, createdAt: '2025-02-01', status: 'Closed' },
      ],
    },
  };

  const mockBiddingItems = {
    data: {
      $values: [
        { id: 'b1', bidTime: new Date().toISOString() },
        { id: 'b2', bidTime: new Date(Date.now() - 1000 * 60).toISOString() },
      ],
    },
  };

  beforeEach(waitForAsync(() => {
    auctionServiceSpy = jasmine.createSpyObj('AuctionService', [
      'getAllAuctions',
    ]);
    biddingServiceSpy = jasmine.createSpyObj('BiddingService', [
      'getBidItemByBidder',
    ]);
    authServiceSpy = jasmine.createSpyObj('AuthService', ['getEmailFromToken']);
    userServiceSpy = jasmine.createSpyObj('UserService', ['getCurrentUser']);

    walletServiceSpy = jasmine.createSpyObj('WalletService',['getWallet']);

    TestBed.configureTestingModule({
      imports: [BidderDashboard],
      providers: [
        { provide: AuctionService, useValue: auctionServiceSpy },
        { provide: BiddingService, useValue: biddingServiceSpy },
        { provide: AuthService, useValue: authServiceSpy },
        { provide: UserService, useValue: userServiceSpy },
        { provide: WalletService, useValue: walletServiceSpy }
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    auctionServiceSpy.getAllAuctions.and.returnValue(of(mockAuctions));
    biddingServiceSpy.getBidItemByBidder.and.returnValue(of(mockBiddingItems));
    walletServiceSpy.getWallet.and.returnValue(of({ balance: 100 }));
    fixture = TestBed.createComponent(BidderDashboard);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch all auctions on init', () => {
    auctionServiceSpy.getAllAuctions.and.returnValue(of(mockAuctions));
    component.ngOnInit();
    expect(auctionServiceSpy.getAllAuctions).toHaveBeenCalled();
    expect(component.auctionItems.length).toBeGreaterThan(0);
    expect(component.auctionItems[0].status).toBe('Upcoming');
  });

  it('should fetch bidding items on init', () => {
    biddingServiceSpy.getBidItemByBidder.and.returnValue(of(mockBiddingItems));
    component.ngOnInit();
    expect(biddingServiceSpy.getBidItemByBidder).toHaveBeenCalled();
    expect(component.biddingItems.length).toBeGreaterThan(0);
    expect(component.latestBiddingItems.length).toBe(2);
    expect(
      new Date(component.latestBiddingItems[0].bidTime) >
        new Date(component.latestBiddingItems[1].bidTime)
    ).toBeTrue();
  });

  it('should handle error on fetchAllAuctions', () => {
    auctionServiceSpy.getAllAuctions.and.returnValue(
      throwError(() => new Error('fail'))
    );
    component.fetchAllAuctions();
    expect(component.auctionItems).toEqual([]);
  });

  it('should handle error on fetchBiddingItems', () => {
    biddingServiceSpy.getBidItemByBidder.and.returnValue(
      throwError(() => new Error('fail'))
    );
    component.fetchBiddingItems();
    expect(component.biddingItems).toEqual([]);
    expect(component.latestBiddingItems).toEqual([]);
  });

  it('should return correct auction status', () => {
    expect(component.getAuctionStatus({ status: 'Upcoming' })).toBe('Upcoming');
    expect(component.getAuctionStatus({ status: 'Live' })).toBe('Live');
    expect(component.getAuctionStatus({ status: 'Other' })).toBe('Unknown');
  });

  it('should show and close auction modal', () => {
    component.auctionItems = [{ id: '1', status: 'Upcoming' }];
    component.showAucrionModel('1');
    expect(component.showModal).toBeTrue();
    expect(component.selectedAuction).toEqual({ id: '1', status: 'Upcoming' });

    component.closeAuctionModel();
    expect(component.showModal).toBeFalse();
    expect(component.selectedAuction).toBeNull();
  });
});
