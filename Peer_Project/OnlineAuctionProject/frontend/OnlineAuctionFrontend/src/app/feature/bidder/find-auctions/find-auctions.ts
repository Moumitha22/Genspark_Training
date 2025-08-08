import { Component, OnInit } from '@angular/core';
import { AuctionService } from '../../../core/services/auction.service';
import { BiddingService } from '../../../core/services/bidding.service';
import { ModelView } from '../../../shared/components/model-view/model-view';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { PlaceBid } from '../../../core/models/PlaceBid';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { Spinner } from '../../../shared/components/spinner/spinner';
import {
  BanIcon,
  ClockIcon,
  DiamondPercent,
  Gavel,
  GavelIcon,
  HandCoins,
  HashIcon,
  LucideAngularModule,
  Package,
  PackageIcon,
  Paperclip,
  ShieldIcon,
  TagIcon,
  TrophyIcon,
} from 'lucide-angular';
import { NotificationService } from '../../../core/services/notify.service';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { AuctionFilter } from '../../../shared/components/auction-filter/auction-filter';

@Component({
  selector: 'app-find-auctions',
  imports: [ModelView, CommonModule, FormsModule, Spinner, LucideAngularModule, ReactiveFormsModule, AuctionFilter],
  templateUrl: './find-auctions.html',
})
export class FindAuctions implements OnInit {
  readonly gavel = GavelIcon;
  readonly ban = BanIcon;
  readonly package = PackageIcon;
  readonly hash = HashIcon;
  readonly clock = ClockIcon;
  readonly tag = TagIcon;
  readonly shield = ShieldIcon;
  readonly handcoins = HandCoins;
  readonly trophy = TrophyIcon;
  readonly attachment = Paperclip;
  readonly startprice = DiamondPercent;

  liveAuctions: any[] = [];
  isLoading = false;
  showModel = false;
  searchString: string = '';
  SearchSubject = new Subject<string>();
  searchFileName: string = '';
  SearchFileSubject = new Subject<string>();
  selectedAuctionId: string | null = null;
  highestBid: number | null = null;
  bidAmount: number = 0;
  startPriceRange = {min:0, max:10000};
  minStartPrice: number = 0;
  selectedSort: string = 'createdAt-desc';
  showFiltersModal = false;
  filtersForm!: FormGroup;
  filtersApplied = false;

  constructor(
    private auctionService: AuctionService,
    private biddingService: BiddingService,
    private authService: AuthService,
    private snackBar: SnackbarService,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.filtersForm = this.fb.group({
      startTime: [''],
      endTime: [''],
      startingPriceMin: [''],
      startingPriceMax: [''],
      reservePriceMin: [''],
      reservePriceMax: [''],
      hasFileAttachments: [false],
    });
    this.fetchLiveAuctions();
    this.SearchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe((searchTerm) => {
      this.searchString = searchTerm;
      this.fetchLiveAuctions();
    });

    this.SearchFileSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe((searchTerm) => {
      this.searchFileName = searchTerm;
      this.fetchLiveAuctions();
    });
  }

  onSearchProducts() {
    this.SearchSubject.next(this.searchString);
  }

  onSearchFileName() {
    this.SearchFileSubject.next(this.searchFileName);
  }

  fetchLiveAuctions() {
    this.isLoading = true;
    this.auctionService.getLiveAuctions().subscribe({
      next: (res) => {
        let auctions = res?.data?.$values || [];
        if (this.searchString && this.searchString.trim() !== '') {
          auctions = auctions.filter(
            (a: any) =>
              a.name?.toLowerCase().includes(this.searchString.toLowerCase()) ||
              a.description
                ?.toLowerCase()
                .includes(this.searchString.toLowerCase())
          );
        }

        if (this.searchFileName && this.searchFileName.trim() !== '') {
          const fileKeyword = this.searchFileName.toLowerCase();
          auctions = auctions.filter((a: any) =>
            a.files?.$values?.some((f: any) =>
              f.name?.toLowerCase().includes(fileKeyword)
            )
          );
        }

        auctions = auctions.filter(
          (a: any) => (a.startingPrice ?? 0) >= this.minStartPrice
        );

        const currentFilters = this.filtersForm?.value;
        console.log(currentFilters);
        if (currentFilters) {
          if (currentFilters.startTime) {
            const startDate = new Date(currentFilters.startTime);
            auctions = auctions.filter((a: any) => new Date(a.startTime) >= startDate);
          }

          if (currentFilters.endTime) {
            const endDate = new Date(currentFilters.endTime);
            endDate.setHours(23, 59, 59, 999);
            auctions = auctions.filter((a: any) => new Date(a.endTime) <= endDate);
          }

          if (currentFilters.startingPriceMin !== '') {
            auctions = auctions.filter((a: any) => a.startingPrice >= +currentFilters.startingPriceMin);
          }

          if (currentFilters.startingPriceMax !== '') {
            auctions = auctions.filter((a: any) => a.startingPrice <= +currentFilters.startingPriceMax);
          }

          if (currentFilters.reservePriceMin !== '') {
            auctions = auctions.filter((a: any) => a.reservePrice >= +currentFilters.reservePriceMin);
          }

          if (currentFilters.reservePriceMax !== '') {
            auctions = auctions.filter((a: any) => a.reservePrice <= +currentFilters.reservePriceMax);
          }

          if (currentFilters.hasFileAttachments) {
            auctions = auctions.filter((a: any) => a.files?.$values && a.files.$values.length > 0);
          }

        }

        if (this.selectedSort) {
          const [field, direction] = this.selectedSort.split('-');
          auctions = auctions.sort((a: any, b: any) => {
            const aVal = new Date(a[field]) || a[field];
            const bVal = new Date(b[field]) || b[field];
            if (aVal < bVal) return direction === 'asc' ? -1 : 1;
            if (aVal > bVal) return direction === 'asc' ? 1 : -1;
            return 0;
          });
        }
        
        this.liveAuctions = auctions;
        this.isLoading = false;
      },
      error: (err) => {
        this.isLoading = false;
        console.log(err);
      },
    });
  }

  onSliderChange() {
    this.fetchLiveAuctions();
  }
  openBidModel(auctionid: string) {
    this.selectedAuctionId = auctionid;
    this.bidAmount = 0;
    this.highestBid = null;
    this.showModel = true;

    this.biddingService.fetchHighestBid(auctionid).subscribe({
      next: (res) => {
        this.highestBid = res?.data?.amount ?? null;
      },
      error: (err) => {
        this.highestBid = null;
        console.log(err);
      },
    });
  }

  closeModel() {
    this.showModel = false;
  }

  viewFile(auctionId: string, fileName: string) {
    this.auctionService.getfile(auctionId, fileName).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        window.open(url);
      },
      error: (err) => {
        this.snackBar.showError(
          "Can't fetch this file. Please try again later."
        );
      },
    });
  }

  submitBid() {
    const bidderId = this.authService.getUserIdFromToken();
    if (!this.selectedAuctionId || !bidderId || !this.bidAmount) return;
    const payloads: PlaceBid = {
      bidderId: bidderId,
      auctionItemId: this.selectedAuctionId,
      amount: this.bidAmount,
    };
    this.biddingService.placeBidding(payloads).subscribe({
      next: () => {
        this.snackBar.showSuccess('Bids placed successfully');
        this.showModel = false;
      },
      error: (err) => {
        console.log(err);
        this.snackBar.showError(`Failed ${err.error.message}`);
      },
    });
  }

  applyFilters(filters?: any) {
    if (filters) {
      this.filtersApplied = Object.keys(filters).length > 0;
      if(this.filtersApplied)
        this.filtersForm.patchValue(filters);
      else 
        this.resetFilters();
    }
    this.fetchLiveAuctions()
    this.showFiltersModal = false;
  }

  resetFilters() {
    this.filtersForm.reset({
      startingPriceMin: 0,
      startingPriceMax: 10000,
      reservePriceMin: 0,
      reservePriceMax: 10000,
      startTime: '',
      endTime: '',
      hasFileAttachments: false
    });
    this.minStartPrice = 0;
    this.selectedSort = 'createdAt-desc';
    this.fetchLiveAuctions();
  }
}
