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
import { ArchiveIcon, ArchiveRestore, TriangleAlert } from 'lucide-angular';
import { ConfirmModal } from '../../../shared/components/confirm-modal/confirm-modal';
import { debounceTime, Subject } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { BidFilter } from '../../../shared/components/bid-filter/bid-filter';

@Component({
  selector: 'app-manage-bids',
  imports: [
    CommonModule,
    ModelView,
    Pagination,
    BidsWinningStatus,
    AuctionDetailsCard,
    ConfirmModal,
    FormsModule,
    BidFilter
  ],
  templateUrl: './manage-bids.html',
  styleUrl: './manage-bids.css'
})
export class ManageBids implements OnInit {
  readonly warning = TriangleAlert;
  readonly archive = ArchiveIcon;
  readonly unarchive = ArchiveRestore; 
  bidItems: any[] = [];
  isLoading = true;
  auctions: any[] = [];
  pagedBidItems: any[] = [];
  showDeleteBidModal = false;
  showArchiveModal = false;
  archiveAction: 'Archive' | 'Unarchive' = 'Archive';
  archiveMessage: string = '';
  archiveIcon: any = ArchiveIcon; 
  selectedAuctionId: string = '';
  selectedBidId: string = '';
  searchString = '';
  selectedSort = 'date-desc';
  statusFilter: string = 'all';
  filter: any = null;
  showBidFilter = false;
  filtersApplied = false;
  showArchivedBidsModal = false;
  archivedBids: any[] = [];
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
    this.getAllBidItems();
    this.getAuctions();
    this.searchSubject.pipe(debounceTime(500)).subscribe((value) => {
      this.searchString = value;
      this.page = 1;
      this.getAllBidItems(this.filter);
    });
  }

  getAllBidItems(filter: any = null) {
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

    this.biddingService.getAllBidItems(filter).subscribe({
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

  openArchiveModal(bidId: string, auctionId: string, action: 'Archive' | 'Unarchive') {
    this.selectedBidId = bidId;
    this.archiveAction = action;
    this.archiveMessage = action === 'Archive'
      ? 'Are you sure you want to archive this bid?'
      : 'Are you sure you want to unarchive this bid?';
    this.archiveIcon = action === 'Archive' ? this.archive : this.unarchive;
    this.showArchiveModal = true;
  }

  closeArchiveModal() {
    this.showArchiveModal = false;
    this.archiveAction = 'Archive';
  }

  onConfirmArchive() {
    if (!this.selectedBidId) {
      this.snackBar.showError('Missing bid ID');
      return;
    }

    const newStatus = this.archiveAction === 'Archive' ? 'Archived' : 'Active';

    this.biddingService.updateBidStatus(this.selectedBidId, { status: newStatus }).subscribe({
      next: () => {
        this.getAllBidItems();
        this.snackBar.showSuccess(`Bid ${this.archiveAction.toLowerCase()}d successfully`);
        this.closeArchiveModal();
      },
      error: () => {
        this.snackBar.showError(`Failed to ${this.archiveAction.toLowerCase()} bid`);
      },
    });
  }

  openDeleteBidModal(bidId: string) {
    this.selectedBidId = bidId;
    this.showDeleteBidModal = true;
  }

  closeDeleteBidModal() {
    this.showDeleteBidModal = false;
    this.selectedBidId = '';
  }

  onConfirmDeleteBid() {
    if (!this.selectedBidId){
      this.snackBar.showError('Missing bid ID');
      return;
    }
    this.biddingService.deleteBids(this.selectedBidId).subscribe({
      next: () => {
        this.getAllBidItems();
        this.snackBar.showSuccess(`Bid deleted successfully`);
        this.closeDeleteBidModal();
      },
      error: () => {
         this.snackBar.showError(`Failed to delete bid`);
      },
    });
  }

  onSearchChange(event: any) {
    this.page = 1;
    this.searchSubject.next(event.target.value);
  }

  onSortChange() {
    this.page = 1;
    this.getAllBidItems(this.filter);
  }

  onFilterApplied(filters: any) {
    this.filtersApplied = Object.keys(filters).length > 0;
    this.filter = {
      ...filters
    };
    this.page = 1;
    this.getAllBidItems(this.filter);
    this.showBidFilter = false;
  }

  closeBidFilterModel() {
    this.showBidFilter = false;
  }

  applyStatusFilter() {
    this.archivedBids = this.bidItems.filter(bid => bid.status === 'Archived');
    const filtered = this.filterBidsByStatus(
      this.bidItems.filter(bid => bid.status === 'Active')
    );
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
