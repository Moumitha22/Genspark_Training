import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import {
  CalendarOffIcon,
  EyeOffIcon,
  FileIcon,
  LucideAngularModule,
  TrashIcon,
  TriangleAlert,
  ArchiveIcon, 
  ArchiveRestore
} from 'lucide-angular';
import { ModelView } from '../model-view/model-view';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { Spinner } from '../spinner/spinner';
import { AuctionService } from '../../../core/services/auction.service';
import { AuctionFilter } from '../auction-filter/auction-filter';
import { Pagination } from '../pagination/pagination';
import { BiddingService } from '../../../core/services/bidding.service';
import { UserService } from '../../../core/services/user.service';
import { User } from '../../../core/models/User';
import { ConfirmModal } from '../confirm-modal/confirm-modal';
import { AuctionDeleteService } from '../../../core/services/auctionDelete.service';
import { AuctionDeleteRequest } from '../../../core/models/AuctionDeleteRequest';
import { BidsWinningStatus } from '../bids-winning-status/bids-winning-status';

@Component({
  selector: 'app-view-auction',
  imports: [
    CommonModule,
    FormsModule,
    LucideAngularModule,
    ModelView,
    Spinner,
    AuctionFilter,
    Pagination,
    ConfirmModal,
    BidsWinningStatus,
  ],
  templateUrl: './view-auction.html',
})
export class ViewAuction {
  readonly fileAttachment = FileIcon;
  readonly nofileAttachement = EyeOffIcon;
  readonly warning = TriangleAlert;
  readonly cancel = CalendarOffIcon;
  readonly trash = TrashIcon;
  readonly archive = ArchiveIcon;
  readonly unarchive = ArchiveRestore; 
  
  constructor(
    private auctionService: AuctionService,
    private authService: AuthService,
    private bidService: BiddingService,
    private router: Router,
    private snackBar: SnackbarService,
    private route: ActivatedRoute,
    private userService: UserService,
    private auctionDeleteService: AuctionDeleteService
  ) {}

  page = 1;
  pageSize = 10;
  totalPages = 0;
  auctions: any[] = [];
  bidsForAuction: any[] = [];
  showCancelModel = false;
  isLoading = false;
  showFilter = false;
  showBidsData = false;
  filtersApplied = false;
  showDeleteBidModal = false;
  showArchiveModal = false;
  showDelete = false;
  showEditModel = false;
  showBidModel = false;
  showRequestReason = false;
  currentFilters: any = {};
  deleteRequest: any;
  auctionToCancel: string | null = '';
  selectedAuctionId: string = '';
  auctionToEdit: any = null;
  role = '';
  sellerId = '';
  selectedBidId: string = '';
  bidsData: any;
  userData: User | null = null;
  winnerId: number | null = null;
  archiveAction: 'Archive' | 'Unarchive' = 'Archive';
  archiveMessage: string = '';
  archiveIcon: any = ArchiveIcon; 
  auctionName: string = '';
  fileName: string = '';
  selectedSort: string = '';
  auctionNameChanged: Subject<string> = new Subject<string>();
  fileNameChanged: Subject<string> = new Subject<string>();

  fetchAuctions(filters?: any) {
    if (filters) {
      this.closeModel('filter');
      this.filtersApplied = Object.keys(filters).length > 0;
      this.currentFilters = filters;
    } else {
      if (
        !this.currentFilters ||
        Object.keys(this.currentFilters).length === 0
      ) {
        this.filtersApplied = false;
      }
      filters = this.currentFilters;
    }

    if (this.role === 'Seller' && !filters.sortBy && !filters.sortDirection){
      filters = { ...filters, sellerId: this.sellerId, sortBy: 'createdAt', sortDirection: 'desc'};
    }
    if (this.role === 'Admin' && !filters.sortBy && !filters.sortDirection){
      filters = { ...filters, sortBy: 'deleteRequest', sortDirection: 'desc'};
    }

    const params: any = {
      page: this.page,
      pageSize: this.pageSize,
    };

    if (this.auctionName?.trim()) {
      params.name = this.auctionName.trim();
    }
    if (filters.status) params.status = filters.status;
    if (filters.startTime) params.startTime = filters.startTime;
    if (filters.endTime) params.endTime = filters.endTime;
    if (filters.sortBy) params.sortBy = filters.sortBy;
    if (filters.sortDirection) params.sortDirection = filters.sortDirection;
    if (filters.status) params.status = filters.status;
    if (filters.sellerId) params.sellerId = filters.sellerId;
    if (filters.startingPriceMin != null) params.startingPriceMin = filters.startingPriceMin;
    if (filters.startingPriceMax != null) params.startingPriceMax = filters.startingPriceMax;
    if (filters.reservePriceMin != null) params.reservePriceMin = filters.reservePriceMin;
    if (filters.reservePriceMax != null) params.reservePriceMax = filters.reservePriceMax;
    if (filters.hasFileAttachments) params.hasFileAttachments = filters.hasFileAttachments;
    if (this.auctionName?.trim()) params.name = this.auctionName.trim();
    if (this.fileName?.trim()) params.fileName = this.fileName.trim();

    this.auctionService.getAuctions(params).subscribe({
      next: (res) => {
        const pagination = res.data?.pagination;
        this.auctions = res.data?.data?.$values || [];
        this.totalPages = Math.ceil(pagination.totalRecords / this.pageSize);
      },
      error: (err) => {
        console.error('Error fetching auctions:', err);
      },
    });
  }

  goToPage(newPage: number) {
    this.page = newPage;
    this.fetchAuctions();
  }

  showRejectInput = false;
  rejectRemarks = '';

  onApproveDeleteRequest(request: any) {
    this.auctionDeleteService
      .approveAuctionDeleteRequest(request.auctionItemId)
      .subscribe({
        next: (res) => {
          this.snackBar.showSuccess('Delete request approved successfully');
          this.showRequestReason = false;
          this.showRejectInput = false;
          this.rejectRemarks = '';
          this.fetchAuctions();
        },
        error: (err) => {
          this.snackBar.showError('Failed to approve delete request');
          console.error(err);
        },
      });
  }

  onRejectDeleteRequest(request: any, remarks: string) {
    this.auctionDeleteService
      .rejectAuctionDeleteRequest({
        auctionItemId: request.auctionItemId,
        remarks: remarks,
      })
      .subscribe({
        next: (res) => {
          this.snackBar.showSuccess('Delete request rejected successfully');
          this.showRequestReason = false;
          this.showRejectInput = false;
          this.rejectRemarks = '';
          this.fetchAuctions();
        },
        error: (err) => {
          this.snackBar.showError('Failed to reject delete request');
          console.error(err);
        },
      });
  }

  ngOnInit() {
    this.route.queryParams.subscribe((params) => {
      this.page = +params['page'] || 1;
      this.currentFilters = {
        ...this.currentFilters,
        ...(params['status'] && { status: params['status'] }),
        ...(params['startTime'] && { startTime: params['startTime'] }),
        ...(params['endTime'] && { endTime: params['endTime'] }),
        ...(params['sortBy'] && { sortBy: params['sortBy'] }),
        ...(params['sortDirection'] && {
          sortDirection: params['sortDirection'],
        }),
      };
      this.filtersApplied = Object.keys(this.currentFilters).length > 0;
      this.fetchRoleAndAuctions();
    });
    this.auctionNameChanged
      .pipe(
        debounceTime(500),
        distinctUntilChanged()
      )
      .subscribe((value) => {
        this.auctionName = value;
        this.fetchAuctions(); 
      });
    this.fileNameChanged
      .pipe(
        debounceTime(500),
        distinctUntilChanged()
      )
      .subscribe((value) => {
        this.fileName = value;
        this.fetchAuctions(); 
      });
  }

  fetchRoleAndAuctions() {
    this.authService.authme().subscribe({
      next: (res) => {
        this.role = res.data.role;
        if (this.role === 'Seller') {
          this.selectedSort = 'createdAt_desc';
        } else if (this.role === 'Admin') {
          this.selectedSort = 'deleterequest_desc';
        }
        this.sellerId = res.data.id;
        this.fetchAuctions();
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  onViewAttachements(auctionId: string) {
    if (this.role === 'Admin') {
      this.router.navigate(['/admin/view-auction-attachements/', auctionId], {
        queryParams: {
          page: this.page,
          ...this.currentFilters,
        },
      });
    } else if (this.role === 'Seller') {
      this.router.navigate(['/seller/view-auction-attachements/', auctionId], {
        queryParams: {
          page: this.page,
          ...this.currentFilters,
        },
      });
    }
  }

  openEditModel(auction: any) {
    this.auctionToEdit = { ...auction };
    this.auctionToEdit.startTime = this.formatDateForInput(auction.startTime);
    this.auctionToEdit.endTime = this.formatDateForInput(auction.endTime);
    this.showEditModel = true;
  }

  onEditSave(updatedAuction: any) {
    this.isLoading = true;
    const formData = new FormData();
    formData.append('name', updatedAuction.name);
    formData.append('description', updatedAuction.description);
    formData.append('startTime', updatedAuction.startTime);
    formData.append('endTime', updatedAuction.endTime);
    formData.append('startingPrice', updatedAuction.startingPrice);
    formData.append('reservePrice', updatedAuction.reservePrice);
    if (updatedAuction.file) {
      formData.append(
        'fileAttachments',
        updatedAuction.file,
        updatedAuction.file.name
      );
    }

    this.auctionService.UpdateAuction(updatedAuction.id, formData).subscribe({
      next: () => {
        this.isLoading = false;
        this.closeModel('edit');
        this.snackBar.showSuccess('Auction updated');
        this.fetchAuctions();
      },
      error: (err) => {
        this.isLoading = false;
        this.snackBar.showError('Failed to update auction');
        console.error(err);
      },
    });
  }

  onFileChange(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.auctionToEdit.file = file;
    }
  }

  onAuctionCancel(auction: any) {
    if (!auction || !auction.id) {
      this.snackBar.showError('Invalid auction');
      return;
    }
    this.isLoading = true;
    this.auctionService.cancelAuction(auction.id, 'Cancelled').subscribe({
      next: (res) => {
        setTimeout(() => {
          this.isLoading = false;
          this.closeModel('cancel');
          this.snackBar.showSuccess('Auction cancelled');
          this.fetchAuctions();
        }, 2000);
      },
      error: (err) => {
        this.isLoading = false;
        this.snackBar.showError('Failed to cancel');
        console.log(err);
      },
    });
  }
  
  onSortChange(event: any) {
    const [sortBy, sortDirection] = this.selectedSort.split('_');
    this.currentFilters = {
      ...this.currentFilters,
      sortBy,
      sortDirection,
    };
    this.fetchAuctions();
  }

  onFilterApplied(filters: any) {
    this.page = 1;
    this.filtersApplied = true;
    this.fetchAuctions(filters);
  }

  showModel(type: string, auction: any) {
    if (type === 'filter') {
      this.showFilter = true;
    } else if (type === 'delete') {
      this.showDelete = true;
      this.selectedAuctionId = auction.id;
    } else if (type === 'cancel') {
      this.auctionToCancel = auction;
      this.showCancelModel = true;
    } else if (type === 'bids') {
      this.selectedAuctionId = auction.id;
      this.showBidModel = true;
      this.winnerId = auction?.winnerId ?? null;
      this.fetchBidsForAuction(auction.id);
    } else if (type === 'bidsData') {
      this.selectedBidId = auction.winnerId;
      this.showBidsData = true;
      this.getBidsItem(auction.winnerId);
    } else if (type === 'request-reason') {
      this.deleteRequest = auction;
      this.showRequestReason = true;
      this.showRejectInput = false;
      this.rejectRemarks = '';
    }
  }
  closeModel(type: string) {
    if (type === 'filter') {
      this.showFilter = false;
    } else if (type === 'delete') {
      this.showDelete = false;
      this.selectedAuctionId = '';
    } else if (type === 'cancel') {
      this.showCancelModel = false;
      this.auctionToCancel = null;
    } else if (type === 'edit') {
      this.showEditModel = false;
      this.auctionToEdit = null;
    } else if (type === 'bids') {
      this.showBidModel = false;
      this.selectedAuctionId = '';
    } else if (type === 'bidsData') {
      this.showBidsData = false;
      this.selectedBidId = '';
      this.bidsData = null;
    }
  }

  fetchBidsForAuction(auctionId: string) {
    this.bidService.getBidsByAuctionId(auctionId).subscribe({
      next: (res) => {
        this.bidsForAuction = res.data?.$values || [];
      },
      error: (err) => {
        console.error('Error fetching bids:', err);
      },
    });
  }
  onAuctionDelete() {
    this.isLoading = true;
    this.auctionService.deleteAuction(this.selectedAuctionId).subscribe({
      next: () => {
        setTimeout(() => {
          this.isLoading = false;
          this.snackBar.showSuccess('Auction Deleted Successfully');
          this.closeModel('delete');
          this.fetchAuctions();
        }, 2000);
      },
      error: (err) => {
        this.isLoading = false;
        this.snackBar.showError('Failed to delete');
        this.closeModel('delete');
      },
    });
  }

  openDeleteBidModal(bidId: string, auctionId: string) {
    this.selectedBidId = bidId;
    this.selectedAuctionId = auctionId;
    this.showDeleteBidModal = true;
  }

  closeDeleteBidModal() {
    this.showDeleteBidModal = false;
    this.selectedBidId = '';
    this.selectedAuctionId = '';
  }

  onConfirmDeleteBid() {
    if (!this.selectedBidId || !this.selectedAuctionId) {
      this.snackBar.showError('Missing bid or auction ID');
      return;
    }
    this.bidService.deleteBids(this.selectedBidId).subscribe({
      next: () => {
        this.fetchBidsForAuction(this.selectedAuctionId);
        this.snackBar.showSuccess(`Bid deleted successfully`);
        this.closeDeleteBidModal();
      },
      error: () => {
         this.snackBar.showError(`Failed to delete bid`);
      },
    });
  }

  openArchiveModal(bidId: string, auctionId: string, action: 'Archive' | 'Unarchive') {
    this.selectedBidId = bidId;
    this.selectedAuctionId = auctionId;
    this.archiveAction = action;
    this.archiveMessage = action === 'Archive'
      ? 'Are you sure you want to archive this bid?'
      : 'Are you sure you want to unarchive this bid?';
    this.archiveIcon = action === 'Archive' ? this.archive : this.unarchive;
    this.showArchiveModal = true;
  }

  closeArchiveModal() {
    this.showArchiveModal = false;
    this.selectedBidId = '';
    this.archiveAction = 'Archive';
  }

  onConfirmArchive() {
    if (!this.selectedBidId || !this.selectedAuctionId) {
      this.snackBar.showError('Missing bid or auction ID');
      return;
    }

    const newStatus = this.archiveAction === 'Archive' ? 'Archived' : 'Active';

    this.bidService.updateBidStatus(this.selectedBidId, { status: newStatus }).subscribe({
      next: () => {
        this.fetchBidsForAuction(this.selectedAuctionId);
        this.snackBar.showSuccess(`Bid ${this.archiveAction.toLowerCase()}d successfully`);
        this.closeArchiveModal();
      },
      error: () => {
        this.snackBar.showError(`Failed to ${this.archiveAction.toLowerCase()} bid`);
      },
    });
  }

  getBidsItem(bidId: string) {
    this.bidService.getBidsByBidderId(bidId).subscribe({
      next: (res) => {
        this.bidsData = res.data;
        if (this.bidsData.userId) {
          this.getUserData(this.bidsData.userId);
        }
      },
      error: (err) => {
        console.error('Error fetching bids:', err);
      },
    });
  }

  userDetails: any = '';

  getUserData(userId: string) {
    this.userService.getByUserId(userId).subscribe({
      next: (res) => {
        this.userDetails = res.data;
      },
      error: (err) => {
        console.error('Error fetching user data:', err);
        return null;
      },
    });
  }

  onDeleteRequest(reason: string) {
    const payload: AuctionDeleteRequest = {
      auctionItemId: this.selectedAuctionId,
      userId: this.sellerId,
      reason: reason,
    };
    this.isLoading = true;
    this.auctionDeleteService.requestAuctionDelete(payload).subscribe({
      next: () => {
        this.isLoading = false;
        this.snackBar.showSuccess('Request sent successfully');
        this.closeModel('delete');
        this.fetchRoleAndAuctions();
      },
      error: (err) => {
        this.isLoading = false;
        this.snackBar.showError('Failed to send request');
      },
    });
  }

  formatDateForInput(date: Date | string): string {
    if (!date) return '';
    const d = new Date(date);
    const pad = (n: number) => (n < 10 ? '0' + n : n);
    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(
      d.getDate()
    )}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
  }
}
