import {
  ComponentFixture,
  TestBed,
  waitForAsync,
  fakeAsync,
  tick,
} from '@angular/core/testing';
import { ViewAuction } from './view-auction';
import { AuctionService } from '../../../core/services/auction.service';
import { AuthService } from '../../../core/services/auth.service';
import { BiddingService } from '../../../core/services/bidding.service';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { UserService } from '../../../core/services/user.service';
import { AuctionDeleteService } from '../../../core/services/auctionDelete.service';

describe('ViewAuction', () => {
  let component: ViewAuction;
  let fixture: ComponentFixture<ViewAuction>;
  let auctionServiceSpy: jasmine.SpyObj<AuctionService>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let bidServiceSpy: jasmine.SpyObj<BiddingService>;
  let snackBarSpy: jasmine.SpyObj<SnackbarService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let activatedRouteStub: any;
  let auctionDeleteServiceSpy: jasmine.SpyObj<AuctionDeleteService>;

  beforeEach(waitForAsync(() => {
    auctionServiceSpy = jasmine.createSpyObj('AuctionService', [
      'getAuctions',
      'deleteAuction',
      'UpdateAuction',
      'cancelAuction',
    ]);
    authServiceSpy = jasmine.createSpyObj('AuthService', ['authme']);
    bidServiceSpy = jasmine.createSpyObj('BiddingService', [
      'getBidsByAuctionId',
      'deleteBids',
    ]);
    auctionDeleteServiceSpy = jasmine.createSpyObj('AuctionDeleteService', [
      'requestAuctionDelete',
    ]);
    snackBarSpy = jasmine.createSpyObj('SnackbarService', [
      'showSuccess',
      'showError',
    ]);
    userServiceSpy = jasmine.createSpyObj('UserService', ['getUserById']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    activatedRouteStub = {
      queryParams: of({ page: 1 }),
    };

    TestBed.configureTestingModule({
      imports: [ViewAuction],
      providers: [
        { provide: AuctionService, useValue: auctionServiceSpy },
        { provide: AuthService, useValue: authServiceSpy },
        { provide: BiddingService, useValue: bidServiceSpy },
        { provide: SnackbarService, useValue: snackBarSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ActivatedRoute, useValue: activatedRouteStub },
        { provide: UserService, useValue: userServiceSpy },
        { provide: AuctionDeleteService, useValue: auctionDeleteServiceSpy },
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    auctionServiceSpy.getAuctions.and.returnValue(
      of({ data: { data: { $values: [] }, pagination: { totalRecords: 0 } } })
    );
    authServiceSpy.authme.and.returnValue(
      of({ data: { role: 'Seller', id: 'seller1' } })
    );
    fixture = TestBed.createComponent(ViewAuction);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch auctions on init', () => {
    expect(auctionServiceSpy.getAuctions).toHaveBeenCalled();
    expect(component.auctions).toEqual([]);
  });

  it('should open and close delete model', () => {
    component.showModel('delete',{id:'a1'});
    expect(component.showDelete).toBeTrue();
    expect(component.selectedAuctionId).toBe('a1');
    component.closeModel('delete');
    expect(component.showDelete).toBeFalse();
    expect(component.selectedAuctionId).toBe('');
  });
  it('should delete auction successfully', fakeAsync(() => {
    auctionServiceSpy.deleteAuction.and.returnValue(of({}));
    component.selectedAuctionId = 'a1';
    component.onAuctionDelete();
    tick(2000);
    expect(auctionServiceSpy.deleteAuction).toHaveBeenCalledWith('a1');
    expect(snackBarSpy.showSuccess).toHaveBeenCalledWith(
      'Auction Deleted Successfully'
    );
    expect(component.isLoading).toBeFalse();
    expect(component.showDelete).toBeFalse();
  }));

  it('should handle error when deleting auction', () => {
    auctionServiceSpy.deleteAuction.and.returnValue(
      throwError(() => new Error('fail'))
    );
    component.selectedAuctionId = 'a1';
    component.onAuctionDelete();
    expect(snackBarSpy.showError).toHaveBeenCalledWith('Failed to delete');
    expect(component.isLoading).toBeFalse();
    expect(component.showDelete).toBeFalse();
  });

  it('should delete bids successfully', () => {
    bidServiceSpy.deleteBids.and.returnValue(of({}));
    spyOn(window, 'alert');
    component.onDeleteBids('bid1');
    expect(bidServiceSpy.deleteBids).toHaveBeenCalledWith('bid1');
    expect(window.alert).toHaveBeenCalledWith('Deleted');
  });

  it('should handle error when deleting bids', () => {
    bidServiceSpy.deleteBids.and.returnValue(
      throwError(() => new Error('fail'))
    );
    spyOn(window, 'alert');
    component.onDeleteBids('bid1');
    expect(window.alert).toHaveBeenCalledWith('error');
  });
});
