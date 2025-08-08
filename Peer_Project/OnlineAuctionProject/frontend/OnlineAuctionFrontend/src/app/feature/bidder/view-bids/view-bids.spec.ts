import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { ViewBids } from './view-bids';
import { BiddingService } from '../../../core/services/bidding.service';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { AuctionService } from '../../../core/services/auction.service';
import { of, throwError } from 'rxjs';

describe('ViewBids', () => {
  let component: ViewBids;
  let fixture: ComponentFixture<ViewBids>;
  let biddingServiceSpy: jasmine.SpyObj<BiddingService>;
  let snackBarSpy: jasmine.SpyObj<SnackbarService>;
  let auctionServiceSpy: jasmine.SpyObj<AuctionService>;

  const mockBidItems = {
    data: {
      $values: [
        { id: 'b1', auctionId: 'a1', amount: 100 },
        { id: 'b2', auctionId: 'a2', amount: 200 },
      ],
    },
  };

  const mockAuctions = {
    data: {
      $values: [
        { id: 'a1', status: 'Live' },
        { id: 'a2', status: 'Closed' },
      ],
    },
  };

  beforeEach(waitForAsync(() => {
    biddingServiceSpy = jasmine.createSpyObj('BiddingService', [
      'getBidItemByBidder',
    ]);
    snackBarSpy = jasmine.createSpyObj('SnackbarService', ['showError']);
    auctionServiceSpy = jasmine.createSpyObj('AuctionService', [
      'getAllAuctions',
    ]);

    TestBed.configureTestingModule({
      imports: [ViewBids],
      providers: [
        { provide: BiddingService, useValue: biddingServiceSpy },
        { provide: SnackbarService, useValue: snackBarSpy },
        { provide: AuctionService, useValue: auctionServiceSpy },
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    biddingServiceSpy.getBidItemByBidder.and.returnValue(of(mockBidItems));
    auctionServiceSpy.getAllAuctions.and.returnValue(of(mockAuctions));
    fixture = TestBed.createComponent(ViewBids);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch bid items and update pagination on init', () => {
    expect(biddingServiceSpy.getBidItemByBidder).toHaveBeenCalled();
    expect(component.bidItems.length).toBe(2);
    expect(component.pagedBidItems.length).toBe(2);
    expect(component.isLoading).toBeFalse();
  });

  it('should fetch auctions on init', () => {
    expect(auctionServiceSpy.getAllAuctions).toHaveBeenCalled();
    expect(component.auctions.length).toBe(2);
  });

  it('should handle error when fetching bid items', () => {
    biddingServiceSpy.getBidItemByBidder.and.returnValue(
      throwError(() => ({ error: { message: 'fail' } }))
    );
    component.getBidItemByBidderId();
    expect(snackBarSpy.showError).toHaveBeenCalledWith('fail');
    expect(component.isLoading).toBeFalse();
  });

  it('should update pagination data when goToPage is called', () => {
    component.bidItems = Array.from({ length: 25 }, (_, i) => ({
      id: `b${i + 1}`,
    }));
    component.pageSize = 10;
    component.goToPage(2);
    expect(component.page).toBe(2);
    expect(component.pagedBidItems.length).toBe(10);
    component.goToPage(3);
    expect(component.pagedBidItems.length).toBe(5);
  });

  it('should open and close auction modal', () => {
    component.auctions = [{ id: 'a1', status: 'Live' }];
    component.openAuctionModal('a1');
    expect(component.showAuctionModal).toBeTrue();
    expect(component.selectedAuction).toEqual({ id: 'a1', status: 'Live' });

    component.closeAuctionModal();
    expect(component.showAuctionModal).toBeFalse();
    expect(component.selectedAuction).toBeNull();
  });
});
