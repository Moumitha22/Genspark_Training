import {
  ComponentFixture,
  TestBed,
  waitForAsync,
  fakeAsync,
  tick,
} from '@angular/core/testing';
import { AuctionForm } from './auction-form';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { AuctionService } from '../../../core/services/auction.service';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { AuthService } from '../../../core/services/auth.service';
import { of, throwError } from 'rxjs';

describe('AuctionForm', () => {
  let component: AuctionForm;
  let fixture: ComponentFixture<AuctionForm>;
  let auctionServiceSpy: jasmine.SpyObj<AuctionService>;
  let snackBarSpy: jasmine.SpyObj<SnackbarService>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  beforeEach(waitForAsync(() => {
    auctionServiceSpy = jasmine.createSpyObj('AuctionService', [
      'createAuction',
    ]);
    snackBarSpy = jasmine.createSpyObj('SnackbarService', [
      'showSuccess',
      'showError',
    ]);
    authServiceSpy = jasmine.createSpyObj('AuthService', [
      'getUserIdFromToken',
      'getUserRole',
    ]);

    authServiceSpy.getUserIdFromToken.and.returnValue('seller123');
    authServiceSpy.getUserRole.and.returnValue('Seller');

    TestBed.configureTestingModule({
      imports: [AuctionForm, ReactiveFormsModule],
      providers: [
        FormBuilder,
        { provide: AuctionService, useValue: auctionServiceSpy },
        { provide: SnackbarService, useValue: snackBarSpy },
        { provide: AuthService, useValue: authServiceSpy },
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AuctionForm);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component and form', () => {
    expect(component).toBeTruthy();
    expect(component.auctionForm).toBeTruthy();
    expect(component.auctionForm.get('sellerId')?.value).toBe('seller123');
  });

  it('should set selectedFiles on file change', () => {
    const file = new File([''], 'test.txt');
    const event = { target: { files: [file] } };
    component.onFileChange(event);
    expect(component.selectedFiles.length).toBe(1);
    expect(component.selectedFiles[0].name).toBe('test.txt');
  });

  it('should submit auction and show success', fakeAsync(() => {
    auctionServiceSpy.createAuction.and.returnValue(of({}));
    component.auctionForm.setValue({
      name: 'Auction',
      description: 'desc',
      startTime: '2025-01-01',
      endTime: '2025-01-02',
      status: 'Live',
      sellerId: 'seller123',
      startingPrice: 100,
      reservePrice: 50,
    });
    component.selectedFiles = [];
    component.submitAuction();
    expect(component.isloading).toBeTrue();
    tick(2000);
    expect(snackBarSpy.showSuccess).toHaveBeenCalledWith('Auction created!');
    expect(component.isloading).toBeFalse();
  }));

  it('should show error on auction creation failure', () => {
    auctionServiceSpy.createAuction.and.returnValue(
      throwError(() => ({ error: { message: 'fail' } }))
    );
    component.auctionForm.setValue({
      name: 'Auction',
      description: 'desc',
      startTime: '2025-01-01',
      endTime: '2025-01-02',
      status: 'Live',
      sellerId: 'seller123',
      startingPrice: 100,
      reservePrice: 50,
    });
    component.selectedFiles = [];
    component.submitAuction();
    expect(snackBarSpy.showError).toHaveBeenCalledWith(
      'Failed to create: fail'
    );
    expect(component.isloading).toBeFalse();
  });
});
