import { Component, OnInit } from '@angular/core';
import { WalletService } from '../../../core/services/wallet.service';
import { Spinner } from '../../../shared/components/spinner/spinner';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ModelView } from '../../../shared/components/model-view/model-view';
import { SnackbarService } from '../../../core/services/snackbar.service';

@Component({
  selector: 'app-virtual-wallet',
  imports: [Spinner, FormsModule, CommonModule, ModelView],
  templateUrl: './virtual-wallet.html',
})
export class VirtualWallet implements OnInit {
  balance: number | null = null;
  addingAmount = 0;
  virtualWalletHistory: any[] = [];
  message = '';
  isLoading = false;
  showHistoryModal = false;
  newWalletBalance: number = 0;

  constructor(
    private walletService: WalletService,
    private snackbar: SnackbarService
  ) {}

  ngOnInit(): void {
    this.getUserWallet();
  }

  getUserWallet() {
    this.isLoading = true;
    this.walletService.getWallet().subscribe({
      next: (res) => {
        this.balance = res.data?.balance ?? 0;
        this.isLoading = false;
      },
      error: () => {
        this.message = 'Failed to load wallet';
        this.isLoading = false;
      },
    });
  }
  addFundsToWallet() {
    if (this.addingAmount <= 0) return;
    var amount = this.addingAmount + (this.balance ?? 0);
    if (amount > 5_000_000) {
      alert('You can not add more than 5,000,000 to your wallet at once');
      return;
    }

    this.isLoading = true;
    this.walletService.addFunds(this.addingAmount).subscribe({
      next: () => {
        setTimeout(() => {
          this.message = 'Funds added';
          this.getUserWallet();
          this.addingAmount = 0;
        }, 1000);
      },
      error: () => {
        this.message = 'Failed to add Funds';
        this.isLoading = false;
      },
    });
  }

  getVirtualWalletHistory() {
    this.showHistoryModal = true;
    this.walletService.getVirtualWalletHistory().subscribe({
      next: (res) => {
        this.virtualWalletHistory = res.data?.$values;
      },
    });
  }

  AddWallet() {
    if (this.newWalletBalance >= 0) {
      this.isLoading = true;
      this.walletService.AddVirtualWallet(this.newWalletBalance).subscribe({
        next: () => {
          setTimeout(() => {
            this.isLoading = false;
            this.snackbar.showSuccess('Virtual wallet created!!');
            this.getUserWallet();
          }, 2000);
        },
        error: () => {
          this.isLoading = false;
          this.snackbar.showError(`Failed to create`);
        },
      });
    }
  }
}
