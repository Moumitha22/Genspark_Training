import { Component, OnInit, ViewChild } from '@angular/core';
import { ChartComponent, ChartType, NgApexchartsModule } from 'ng-apexcharts';
import { AuctionService } from '../../../core/services/auction.service';
import { BiddingService } from '../../../core/services/bidding.service';
import {
  countByStatus,
  buildStatusChartOptions,
  buildMonthlyChart,
} from '../../../shared/utils/dashboard-utils';
import { WelcomeCard } from '../../../shared/components/welcome-card/welcome-card';
import { observeThemeChanges } from '../../../shared/utils/theme-utils';
import { CommonModule } from '@angular/common';
import { SummaryCard } from '../../../shared/components/summary-card/summary-card';
import { BidsWinningStatus } from '../../../shared/components/bids-winning-status/bids-winning-status';
import { ModelView } from '../../../shared/components/model-view/model-view';
import { AuctionDetailsCard } from '../../../shared/components/auction-details-card/auction-details-card';
import { WalletService } from '../../../core/services/wallet.service';
import { LucideAngularModule, WalletIcon } from 'lucide-angular';

export type ChartOptions = {
  series?: ApexAxisChartSeries;
  chart: ApexChart & { type: ChartType };
  dataLabels: ApexDataLabels;
  plotOptions: ApexPlotOptions;
  xaxis: ApexXAxis;
  yaxis?: ApexYAxis;
  colors?: string[];
  theme?: ApexTheme;
  tooltip?: ApexTooltip;
};

@Component({
  selector: 'app-bidder-dashboard',
  imports: [
    NgApexchartsModule,
    WelcomeCard,
    CommonModule,
    SummaryCard,
    BidsWinningStatus,
    ModelView,
    AuctionDetailsCard,
    LucideAngularModule,
  ],
  templateUrl: './bidder-dashboard.html',
})
export class BidderDashboard implements OnInit {
  @ViewChild('statusChart') statusChart!: ChartComponent;
  @ViewChild('monthlyChart') monthlyChart!: ChartComponent;
  type: ('line' | 'bar' | 'area')[] = ['line', 'bar', 'area'];

  public statusChartOptions!: Partial<ChartOptions>;
  public monthlyChartOptions!: Partial<ChartOptions>;

  readonly walletIcon = WalletIcon;

  status: string[] = ['Upcoming', 'Live'];
  constructor(
    private auctionService: AuctionService,
    private biddingService: BiddingService,
    private walletService: WalletService
  ) {}
  auctionItems: any[] = [];
  biddingItems: any[] = [];
  latestBiddingItems: any[] = [];
  private hasViewInitialized = false;
  private filterationValue: string = 'bidTime';
  chartHeight = 250;
  showModal = false;
  selectedAuction: any = null;
  virtualWalletBalance: number = 0;

  ngOnInit(): void {
    this.fetchAllAuctions();
    this.fetchBiddingItems();
    this.fetchVirtualWalletBalance();
  }

  fetchAllAuctions() {
    this.auctionService.getAllAuctions().subscribe({
      next: (res) => {
        this.auctionItems = res?.data?.$values ?? [];
        const counts = countByStatus(
          this.auctionItems,
          this.getAuctionStatus,
          this.status
        );
        this.statusChartOptions = buildStatusChartOptions(
          counts,
          this.status,
          this.chartHeight
        );
        this.monthlyChartOptions = buildMonthlyChart(
          this.biddingItems,
          this.filterationValue,
          this.type[2],
          this.chartHeight
        );
        observeThemeChanges(
          this.auctionItems,
          this.filterationValue,
          this.getAuctionStatus,
          this.status,
          this.hasViewInitialized,
          this.statusChart,
          this.monthlyChart,
          this.statusChartOptions,
          this.monthlyChartOptions,
          this.chartHeight,
          () => {
            const counts = countByStatus(
              this.auctionItems,
              this.getAuctionStatus,
              this.status
            );
            this.statusChartOptions = buildStatusChartOptions(
              counts,
              this.status,
              this.chartHeight
            );
          },
          () => {
            this.monthlyChartOptions = buildMonthlyChart(
              this.biddingItems,
              this.filterationValue,
              this.type[2],
              this.chartHeight
            );
          }
        );
      },
      error: (err) => {
        console.log(err);
        this.auctionItems = [];
        const counts = countByStatus([], () => 'Unknown', this.status);
        this.statusChartOptions = buildStatusChartOptions(
          counts,
          this.status,
          this.chartHeight
        );
        observeThemeChanges(
          this.auctionItems,
          this.filterationValue,
          this.getAuctionStatus,
          this.status,
          this.hasViewInitialized,
          this.statusChart,
          this.monthlyChart,
          this.statusChartOptions,
          this.monthlyChartOptions,
          this.chartHeight,
          () => {
            const counts = countByStatus(
              this.auctionItems,
              this.getAuctionStatus,
              this.status
            );
            this.statusChartOptions = buildStatusChartOptions(
              counts,
              this.status,
              this.chartHeight
            );
          },
          () => {
            this.monthlyChartOptions = buildMonthlyChart(
              this.biddingItems,
              this.filterationValue,
              this.type[2],
              this.chartHeight
            );
          }
        );
      },
    });
  }

  fetchBiddingItems() {
    this.biddingService.getBidItemByBidder().subscribe({
      next: (res) => {
        this.biddingItems = res?.data?.$values ?? [];
        this.latestBiddingItems = this.biddingItems
          .sort(
            (a, b) =>
              new Date(b.bidTime).getTime() - new Date(a.bidTime).getTime()
          )
          .slice(0, 5);
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  fetchVirtualWalletBalance() {
    this.walletService.getWallet().subscribe({
      next: (res) => {
        this.virtualWalletBalance = res?.data?.balance ?? 0;
      },
      error: (err) => {
        console.log(err);
        this.virtualWalletBalance = 0;
      },
    });
  }
  getAuctionStatus(auction: any): string {
    if (auction.status === 'Upcoming') return 'Upcoming';
    if (auction.status === 'Live') return 'Live';
    return 'Unknown';
  }

  showAucrionModel(auctionId: string) {
    this.showModal = true;
    this.selectedAuction = this.auctionItems.find((a) => a.id === auctionId);
  }

  closeAuctionModel() {
    this.showModal = false;
    this.selectedAuction = null;
  }

  maxCapacity = 5_000_000;
  get balanceUsed(): number {
    return Math.min((this.virtualWalletBalance / this.maxCapacity) * 100, 100);
  }
}
