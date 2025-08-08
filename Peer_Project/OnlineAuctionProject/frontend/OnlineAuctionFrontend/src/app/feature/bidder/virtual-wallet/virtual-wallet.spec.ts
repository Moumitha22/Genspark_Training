import {
  ComponentFixture,
  fakeAsync,
  TestBed,
  tick,
  waitForAsync,
} from '@angular/core/testing';
import { VirtualWallet } from './virtual-wallet';
import { WalletService } from '../../../core/services/wallet.service';
import { of, throwError } from 'rxjs';

describe('VirtualWallet', () => {
  let component: VirtualWallet;
  let fixture: ComponentFixture<VirtualWallet>;
  let walletServiceSpy: jasmine.SpyObj<WalletService>;

  const mockWallet = { data: { balance: 500 } };
  const mockHistory = {
    data: {
      $values: [
        { id: 1, amount: 100 },
        { id: 2, amount: 200 },
      ],
    },
  };

  beforeEach(waitForAsync(() => {
    walletServiceSpy = jasmine.createSpyObj('WalletService', [
      'getWallet',
      'addFunds',
      'getVirtualWalletHistory',
    ]);

    TestBed.configureTestingModule({
      imports: [VirtualWallet],
      providers: [{ provide: WalletService, useValue: walletServiceSpy }],
    }).compileComponents();
  }));

  beforeEach(() => {
    walletServiceSpy.getWallet.and.returnValue(of(mockWallet));
    walletServiceSpy.addFunds.and.returnValue(of({}));
    walletServiceSpy.getVirtualWalletHistory.and.returnValue(of(mockHistory));
    fixture = TestBed.createComponent(VirtualWallet);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch wallet balance on init', () => {
    expect(walletServiceSpy.getWallet).toHaveBeenCalled();
    expect(component.balance).toBe(500);
    expect(component.isLoading).toBeFalse();
  });

  it('should handle error when fetching wallet', () => {
    walletServiceSpy.getWallet.and.returnValue(
      throwError(() => new Error('fail'))
    );
    component.getUserWallet();
    expect(component.message).toBe('Failed to load wallet');
    expect(component.isLoading).toBeFalse();
  });

  it('should add funds to wallet and refresh balance', fakeAsync(() => {
    spyOn(component, 'getUserWallet');
    component.addingAmount = 100;
    component.addFundsToWallet();
    tick(1100);
    expect(walletServiceSpy.addFunds).toHaveBeenCalledWith(100);
    expect(component.message).toBe('Funds added');
    expect(component.getUserWallet).toHaveBeenCalled();
    expect(component.addingAmount).toBe(0);
  }));

  it('should not add funds if amount is zero or less', () => {
    component.addingAmount = 0;
    component.addFundsToWallet();
    expect(walletServiceSpy.addFunds).not.toHaveBeenCalled();
  });

  it('should handle error when adding funds', () => {
    walletServiceSpy.addFunds.and.returnValue(
      throwError(() => new Error('fail'))
    );
    component.addingAmount = 100;
    component.addFundsToWallet();
    expect(component.message).toBe('Failed to add Funds');
    expect(component.isLoading).toBeFalse();
  });

  it('should fetch virtual wallet history and open modal', () => {
    component.getVirtualWalletHistory();
    expect(walletServiceSpy.getVirtualWalletHistory).toHaveBeenCalled();
    expect(component.virtualWalletHistory.length).toBe(2);
    expect(component.showHistoryModal).toBeTrue();
  });
});
