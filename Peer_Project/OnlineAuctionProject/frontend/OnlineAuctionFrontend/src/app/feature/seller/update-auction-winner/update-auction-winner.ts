import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { ChevronDown, ChevronUp, LucideAngularModule } from 'lucide-angular';
import { Spinner } from '../../../shared/components/spinner/spinner';
import { UpdateWinningIdRequest } from '../../../core/models/WinningBidUpdate';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { BiddingService } from '../../../core/services/bidding.service';
import { AuctionService } from '../../../core/services/auction.service';
@Component({
  selector: 'app-update-auction-winner',
  imports: [FormsModule, CommonModule, LucideAngularModule, Spinner],
  templateUrl: './update-auction-winner.html',
})
export class UpdateAuctionWinner implements OnInit {
  readonly dropDown = ChevronDown;
  readonly dropUp = ChevronUp;
  auctionData: any[] = [];
  filteredAuctions: any[] = [];
  selectedAuctionId: string = '';
  selectedAuctionName: string = '';
  highestBid: any = null;
  selectedWinningId: string = '';
  loadingBid = false;
  errorMsg = '';
  dropdownOpen = false;
  showHighestBidDetails = false;

  constructor(
    private auctionService: AuctionService,
    private snackBar: SnackbarService,
    private biddingService: BiddingService
  ) {}

  ngOnInit(): void {
    this.auctionService.getAuctionBySeller().subscribe({
      next: (res) => {
        this.auctionData = Array.isArray(res.data?.$values)
          ? res.data?.$values
          : [];
        this.filteredAuctions = this.auctionData.filter(
          (a: any) =>
            a.status === 'Live' && (!a.winnerId || a.winnerId === null)
        );
      },
      error: () => {
        this.filteredAuctions = [];
      },
    });
  }

  onAuctionSelect(auction: any) {
    this.selectedAuctionId = auction.id;
    this.selectedAuctionName = auction.name;
    this.dropdownOpen = false;
  }
  fetchHighestBid() {
    this.showHighestBidDetails = true;
    if (!this.selectedAuctionId) return;
    this.loadingBid = true;
    this.errorMsg = '';
    this.biddingService.fetchHighestBid(this.selectedAuctionId).subscribe({
      next: (res: any) => {
        this.highestBid = res.data;
        this.loadingBid = false;
      },
      error: () => {
        this.errorMsg = 'Failed to fetch highest bid.';
        this.loadingBid = false;
      },
    });
  }

  toggleDropdown() {
    this.dropdownOpen = !this.dropdownOpen;
  }

  selectAuction(auction: any) {
    this.selectedAuctionId = auction.id;
    this.selectedAuctionName = auction.name;
    this.dropdownOpen = false;
  }

  onWinneringBidUpdate() {
    if (this.selectedAuctionId && this.selectedWinningId) {
      this.loadingBid = true;
      const payload: UpdateWinningIdRequest = {
        winningId: this.selectedWinningId,
        auctionItemId: this.selectedAuctionId,
      };

      this.auctionService.updateWinningId(payload).subscribe({
        next: () => {
          this.loadingBid = false;
          this.snackBar.showSuccess(
            'Winning Bid Id updated and E-agreement generated to the bidder'
          );
        },
        error: (error) => {
          this.loadingBid = false;
          console.error('Error updating winning ID:', error);
          this.snackBar.showError('Failed to update');
        },
      });
    } else {
      this.snackBar.showInfo('Fields are required');
    }
  }
}
