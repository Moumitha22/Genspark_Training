import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { FindAuctions } from './find-auctions';
import { AuctionService } from '../../../core/services/auction.service';
import { BiddingService } from '../../../core/services/bidding.service';
import { AuthService } from '../../../core/services/auth.service';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { of, throwError, Subject } from 'rxjs';

describe('FindAuctions', () => {
  let component: FindAuctions;
  let fixture: ComponentFixture<FindAuctions>;
  let auctionServiceSpy: jasmine.SpyObj<AuctionService>;
  let biddingServiceSpy: jasmine.SpyObj<BiddingService>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let snackBarSpy: jasmine.SpyObj<SnackbarService>;

  const mockAuctions = {
    data: {
      $values: [
        { id: '1', name: 'Auction 1', files:{
          $values:[
            {id:'3', name:'demo.png', downloadUrl:'http://localost:4200/demo', contentType:'image/png'}
          ]
        } },
        { id: '2', name: 'Auction 2',
          files:{
          $values:[
            {id:'4', name:'demo1.png', downloadUrl:'http://localost:4200/demo1', contentType:'image/png'}
          ]
        }

         },
      ],
    },
  };

  beforeEach(waitForAsync(() => {
    auctionServiceSpy = jasmine.createSpyObj('AuctionService', [
      'getLiveAuctions',
    ]);
    biddingServiceSpy = jasmine.createSpyObj('BiddingService', [
      'fetchHighestBid',
      'placeBidding',
    ]);
    authServiceSpy = jasmine.createSpyObj('AuthService', [
      'getUserIdFromToken',
    ]);
    snackBarSpy = jasmine.createSpyObj('SnackbarService', [
      'showSuccess',
      'showError',
    ]);

    TestBed.configureTestingModule({
      imports: [FindAuctions],
      providers: [
        { provide: AuctionService, useValue: auctionServiceSpy },
        { provide: BiddingService, useValue: biddingServiceSpy },
        { provide: AuthService, useValue: authServiceSpy },
        { provide: SnackbarService, useValue: snackBarSpy },
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    auctionServiceSpy.getLiveAuctions.and.returnValue(of(mockAuctions));
    fixture = TestBed.createComponent(FindAuctions);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch live auctions on init', () => {
    expect(auctionServiceSpy.getLiveAuctions).toHaveBeenCalled();
    expect(component.liveAuctions.length).toBe(2);
    expect(component.isLoading).toBeFalse();
  });

  it('should filter auctions by search string', () => {
    auctionServiceSpy.getLiveAuctions.and.returnValue(
      of({
        data: { $values: [{ id: '1', name: 'Test Auction' }] },
      })
    );
    component.searchString = 'test';
    component.fetchLiveAuctions();
    expect(component.liveAuctions.length).toBe(1);
    expect(component.liveAuctions[0].name).toBe('Test Auction');
  });

  it('should handle error on fetchLiveAuctions', () => {
    auctionServiceSpy.getLiveAuctions.and.returnValue(
      throwError(() => new Error('fail'))
    );
    spyOn(console, 'log');
    component.fetchLiveAuctions();
    expect(component.isLoading).toBeFalse();
    expect(console.log).toHaveBeenCalled();
  });

  it('should open bid model and fetch highest bid', () => {
    biddingServiceSpy.fetchHighestBid.and.returnValue(
      of({ data: { amount: 500 } })
    );
    component.openBidModel('1');
    expect(component.selectedAuctionId).toBe('1');
    expect(component.showModel).toBeTrue();
    expect(biddingServiceSpy.fetchHighestBid).toHaveBeenCalledWith('1');
    expect(component.highestBid).toBe(500);
  });

  it('should handle error when fetching highest bid', () => {
    biddingServiceSpy.fetchHighestBid.and.returnValue(
      throwError(() => new Error('fail'))
    );
    spyOn(console, 'log');
    component.openBidModel('1');
    expect(component.highestBid).toBeNull();
    expect(console.log).toHaveBeenCalled();
  });

  it('should submit bid successfully', () => {
    component.selectedAuctionId = '1';
    component.bidAmount = 100;
    authServiceSpy.getUserIdFromToken.and.returnValue('bidder1');
    biddingServiceSpy.placeBidding.and.returnValue(of({}));
    component.submitBid();
    expect(biddingServiceSpy.placeBidding).toHaveBeenCalled();
    expect(snackBarSpy.showSuccess).toHaveBeenCalledWith(
      'Bids placed successfully'
    );
    expect(component.showModel).toBeFalse();
  });

  it('should show error if submitBid fails', () => {
    component.selectedAuctionId = '1';
    component.bidAmount = 100;
    authServiceSpy.getUserIdFromToken.and.returnValue('bidder1');
    biddingServiceSpy.placeBidding.and.returnValue(
      throwError(() => ({ error: { message: 'fail' } }))
    );
    spyOn(console, 'log');
    component.submitBid();
    expect(snackBarSpy.showError).toHaveBeenCalledWith('Failed fail');
    expect(console.log).toHaveBeenCalled();
  });

  it('should not submit bid if required fields are missing', () => {
    component.selectedAuctionId = null;
    component.bidAmount = 0;
    authServiceSpy.getUserIdFromToken.and.returnValue(null);
    component.submitBid();
    expect(biddingServiceSpy.placeBidding).not.toHaveBeenCalled();
  });
});
