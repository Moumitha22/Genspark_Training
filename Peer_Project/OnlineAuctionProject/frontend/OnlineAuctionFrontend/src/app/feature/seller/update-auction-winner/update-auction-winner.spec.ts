import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { UpdateAuctionWinner } from './update-auction-winner';
import { AuctionService } from '../../../core/services/auction.service';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { BiddingService } from '../../../core/services/bidding.service';
import { of, throwError } from 'rxjs';

describe('UpdateAuctionWinner', () => {
  let component: UpdateAuctionWinner;
  let fixture: ComponentFixture<UpdateAuctionWinner>;
  let auctionServiceSpy: jasmine.SpyObj<AuctionService>;
  let snackBarSpy: jasmine.SpyObj<SnackbarService>;
  let biddingServiceSpy: jasmine.SpyObj<BiddingService>;

  const mockAuctions = {
    data: {
      $values: [
        { id: '1', name: 'Auction 1', status: 'Live', winnerId: null },
        { id: '2', name: 'Auction 2', status: 'Closed', winnerId: 'w2' },
      ],
    },
  };

  beforeEach(waitForAsync(() => {
    auctionServiceSpy = jasmine.createSpyObj('AuctionService', [
      'getAuctionBySeller',
      'updateWinningId',
    ]);
    snackBarSpy = jasmine.createSpyObj('SnackbarService', [
      'showSuccess',
      'showError',
      'showInfo',
    ]);
    biddingServiceSpy = jasmine.createSpyObj('BiddingService', [
      'fetchHighestBid',
    ]);

    TestBed.configureTestingModule({
      imports: [UpdateAuctionWinner],
      providers: [
        { provide: AuctionService, useValue: auctionServiceSpy },
        { provide: SnackbarService, useValue: snackBarSpy },
        { provide: BiddingService, useValue: biddingServiceSpy },
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    auctionServiceSpy.getAuctionBySeller.and.returnValue(of(mockAuctions));
    fixture = TestBed.createComponent(UpdateAuctionWinner);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch and filter auctions on init', () => {
    expect(auctionServiceSpy.getAuctionBySeller).toHaveBeenCalled();
    expect(component.auctionData.length).toBe(2);
    expect(component.filteredAuctions.length).toBe(1);
    expect(component.filteredAuctions[0].status).toBe('Live');
  });

  it('should handle error when fetching auctions', () => {
    auctionServiceSpy.getAuctionBySeller.and.returnValue(
      throwError(() => new Error('fail'))
    );
    component.ngOnInit();
    expect(component.filteredAuctions).toEqual([]);
  });

  it('should select auction', () => {
    const auction = { id: '1', name: 'Auction 1' };
    component.onAuctionSelect(auction);
    expect(component.selectedAuctionId).toBe('1');
    expect(component.selectedAuctionName).toBe('Auction 1');
    expect(component.dropdownOpen).toBeFalse();
  });

  it('should fetch highest bid and set highestBid', () => {
    biddingServiceSpy.fetchHighestBid.and.returnValue(
      of({ data: { amount: 500 } })
    );
    component.selectedAuctionId = '1';
    component.fetchHighestBid();
    expect(biddingServiceSpy.fetchHighestBid).toHaveBeenCalledWith('1');
    expect(component.highestBid.amount).toBe(500);
    expect(component.loadingBid).toBeFalse();
    expect(component.showHighestBidDetails).toBeTrue();
  });

  it('should handle error when fetching highest bid', () => {
    biddingServiceSpy.fetchHighestBid.and.returnValue(
      throwError(() => new Error('fail'))
    );
    component.selectedAuctionId = '1';
    component.fetchHighestBid();
    expect(component.errorMsg).toBe('Failed to fetch highest bid.');
    expect(component.loadingBid).toBeFalse();
  });

  it('should toggle dropdown', () => {
    component.dropdownOpen = false;
    component.toggleDropdown();
    expect(component.dropdownOpen).toBeTrue();
    component.toggleDropdown();
    expect(component.dropdownOpen).toBeFalse();
  });

  it('should select auction using selectAuction', () => {
    const auction = { id: '2', name: 'Auction 2' };
    component.selectAuction(auction);
    expect(component.selectedAuctionId).toBe('2');
    expect(component.selectedAuctionName).toBe('Auction 2');
    expect(component.dropdownOpen).toBeFalse();
  });

  it('should update winning bid and show success', () => {
    component.selectedAuctionId = '1';
    component.selectedWinningId = 'w1';
    auctionServiceSpy.updateWinningId.and.returnValue(of({}));
    component.onWinneringBidUpdate();
    expect(auctionServiceSpy.updateWinningId).toHaveBeenCalledWith({
      winningId: 'w1',
      auctionItemId: '1',
    });
    expect(snackBarSpy.showSuccess).toHaveBeenCalled();
  });

  it('should show error if update winning bid fails', () => {
    component.selectedAuctionId = '1';
    component.selectedWinningId = 'w1';
    auctionServiceSpy.updateWinningId.and.returnValue(
      throwError(() => new Error('fail'))
    );
    component.onWinneringBidUpdate();
    expect(snackBarSpy.showError).toHaveBeenCalledWith('Failed to update');
  });

  it('should show info if fields are missing on update', () => {
    component.selectedAuctionId = '';
    component.selectedWinningId = '';
    component.onWinneringBidUpdate();
    expect(snackBarSpy.showInfo).toHaveBeenCalledWith('Fields are required');
  });
});
