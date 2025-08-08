import { Component, OnInit } from '@angular/core';
import { BiddingService } from '../../../core/services/bidding.service';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { CommonModule } from '@angular/common';
import { AuctionService } from '../../../core/services/auction.service';
import { ModelView } from '../../../shared/components/model-view/model-view';
import { paginate } from '../../../shared/utils/pagination-utils';
import { Pagination } from '../../../shared/components/pagination/pagination';
import { BidsWinningStatus } from '../../../shared/components/bids-winning-status/bids-winning-status';
import { AuctionDetailsCard } from '../../../shared/components/auction-details-card/auction-details-card';
import { BidFilter } from '../../../shared/components/bid-filter/bid-filter';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime } from 'rxjs';
import { LucideAngularModule, TicketSlash } from 'lucide-angular';

@Component({
  selector: 'app-view-bids',
  imports: [
    LucideAngularModule,
    CommonModule,
    ModelView,
    Pagination,
    BidsWinningStatus,
    AuctionDetailsCard,
    BidFilter,
    FormsModule,
  ],
  templateUrl: './view-bids.html',
})
export class ViewBids implements OnInit {
  readonly bidsIcon = TicketSlash;
  bidItems: any[] = [];
  isLoading = true;
  auctions: any[] = [];
  pagedBidItems: any[] = [];
  searchString = '';
  selectedSort = 'date-desc';
  statusFilter: string = 'all';
  filter: any = null;
  showBidFilter = false;
  filtersApplied = false;
  private searchSubject = new Subject<string>();

  constructor(
    private biddingService: BiddingService,
    private snackBar: SnackbarService,
    private auctionService: AuctionService
  ) {}

  page = 1;
  pageSize = 10;
  totalPages = 0;

  updatePaginationData(filteredBids: any[]) {
    const result = paginate(filteredBids, this.page, this.pageSize);
    this.pagedBidItems = result.data;
    this.totalPages = result.totalPages;
  }

  goToPage(page: number) {
    this.page = page;
    this.applyStatusFilter();
  }

  showAuctionModal = false;
  selectedAuction: any = null;

  openAuctionModal(auctionId: string) {
    this.selectedAuction = this.auctions.find((a) => a.id === auctionId);
    this.showAuctionModal = true;
  }

  closeAuctionModal() {
    this.showAuctionModal = false;
    this.selectedAuction = null;
  }

  ngOnInit(): void {
    this.getBidItemByBidderId(this.filter);
    this.getAuctions();

    this.searchSubject.pipe(debounceTime(500)).subscribe((value) => {
      this.searchString = value;
      this.page = 1;
      this.getBidItemByBidderId(this.filter);
    });
  }

  getBidItemByBidderId(filter: any = null) {
    this.isLoading = true;
    if (!filter) {
      filter = {}; 
    }
    if (this.searchString) {
      filter.name = this.searchString;
    }

    if (this.selectedSort) {
      const [sortBy, sortDirection] = this.selectedSort.split('-');
      filter.sortBy = sortBy;
      filter.sortDirection = sortDirection;
    }
    
    this.biddingService.getBidItemByBidder(filter).subscribe({
      next: (res) => {
        this.bidItems = res?.data?.$values ?? [];
        this.isLoading = false;
        this.applyStatusFilter();
      },
      error: (err) => {
        this.snackBar.showError(err.error.message);
        this.isLoading = false;
      },
    });
  }

  getAuctions() {
    this.auctionService.getAllAuctions().subscribe({
      next: (res) => {
        this.auctions = res?.data?.$values ?? [];
      },
    });
  }

  onSearchChange(event: any) {
    this.page = 1;
    this.searchSubject.next(event.target.value);
  }

  onSortChange() {
    this.page = 1;
    this.getBidItemByBidderId(this.filter);
  }

  onFilterApplied(filters: any) {
    this.filtersApplied = Object.keys(filters).length > 0;
    this.filter = {
      ...filters
    };
    this.page = 1;
    this.getBidItemByBidderId(this.filter);
    this.showBidFilter = false;
  }

  closeBidFilterModel() {
    this.showBidFilter = false;
  }

  applyStatusFilter() {
    const filtered = this.filterBidsByStatus(this.bidItems);
    this.updatePaginationData(filtered);
  }

  filterBidsByStatus(bids: any[]): any[] {
    if (this.statusFilter === 'all') return bids;

    return bids.filter(bid => {
      const status = this.getBidStatus(bid);
      return status === this.statusFilter;
    });
  }

  getBidStatus(bid: any): 'win' | 'loss' | 'pending' {
    const auction = this.auctions.find((a) => a.id === bid.auctionItemId);
    if (!auction) return 'pending';

    const status = (auction.status || '').toLowerCase();
    if (['closed', 'completed', 'cancelled'].includes(status)) {
      if (auction.winnerId === bid.id) return 'win';
      return 'loss';
    }
    return 'pending';
  }

}
