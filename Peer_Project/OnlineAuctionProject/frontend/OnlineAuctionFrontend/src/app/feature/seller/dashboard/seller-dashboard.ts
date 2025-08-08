import {
  Component,
  inject,
  OnInit,
  AfterViewInit,
  ViewChild,
} from '@angular/core';
import { LucideAngularModule } from 'lucide-angular';
import { SidebarComponent } from '../../../layout/sidebar/sidebar';
import { Header } from '../../../layout/header/header';
import { ChartComponent, ChartType, NgApexchartsModule } from 'ng-apexcharts';
import { CommonModule } from '@angular/common';
import { WelcomeCard } from '../../../shared/components/welcome-card/welcome-card';
import { AuctionService } from '../../../core/services/auction.service';
import {
  countByStatus,
  buildStatusChartOptions,
  buildMonthlyChart,
} from '../../../shared/utils/dashboard-utils';
import { observeThemeChanges } from '../../../shared/utils/theme-utils';
import { SummaryCard } from '../../../shared/components/summary-card/summary-card';
import { BiddingService } from '../../../core/services/bidding.service';
import { Pagination } from '../../../shared/components/pagination/pagination';
import { AuthService } from '../../../core/services/auth.service';

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
  selector: 'app-seller-dashboard',
  standalone: true,
  imports: [
    LucideAngularModule,
    NgApexchartsModule,
    CommonModule,
    WelcomeCard,
    SummaryCard,
    Pagination,
  ],
  templateUrl: './seller-dashboard.html',
})
export class SellerDashboard implements OnInit, AfterViewInit {
  @ViewChild('statusChart') statusChart!: ChartComponent;
  @ViewChild('monthlyChart') monthlyChart!: ChartComponent;

  public statusChartOptions!: Partial<ChartOptions>;
  public monthlyChartOptions!: Partial<ChartOptions>;
  public auctionItems: any[] = [];
  userId: any;
  totalSales: any;


  endingSoonAuctions: any[] = [];
  pagedAuctions: any[] = [];

  pageSize = 5;
  currentPage = 1;
  totalPages = 0;
  filterationValue = 'createdAt';
  chartHeight = 300;

  private auctionService = inject(AuctionService);
  private authService = inject(AuthService);

  filterEndingSoonAuctions() {
    const now = new Date();
    const twoDaysLater = new Date();
    twoDaysLater.setDate(now.getDate() + 2);

    this.endingSoonAuctions = this.auctionItems.filter((a) => {
      const end = new Date(a.endTime);
      return end > now && end <= twoDaysLater;
    });

    this.totalPages = Math.max(
      1,
      Math.ceil(this.endingSoonAuctions.length / this.pageSize)
    );
    this.currentPage = Math.min(this.currentPage, this.totalPages);
    this.updatePagedData();
  }

  updatePagedData() {
    const start = (this.currentPage - 1) * this.pageSize;
    const end = start + this.pageSize;
    this.pagedAuctions = this.endingSoonAuctions.slice(start, end);
  }

  status: string[] = ['Upcoming', 'Live', 'Completed', 'Closed', 'Cancelled'];

  private hasViewInitialized = false;

  ngOnInit(): void {
    this.fetchAuctions();
    this.authme();
  }

  fetchAuctions() {
    this.auctionService.getAuctionBySeller().subscribe({
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
          this.auctionItems,
          'createdAt',
          undefined,
          this.chartHeight
        );
        this.filterEndingSoonAuctions();
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
              this.auctionItems,
              this.filterationValue,
              undefined,
              this.chartHeight
            );
          }
        );
      },
      error: (err) => {
        console.error(err);
        this.auctionItems = [];
        const counts = countByStatus([], () => 'Unknown', this.status);
        this.statusChartOptions = buildStatusChartOptions(
          counts,
          this.status,
          this.chartHeight
        );
        this.monthlyChartOptions = buildMonthlyChart(
          [],
          'Unknown',
          undefined,
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
              this.auctionItems,
              this.filterationValue,
              'bar',
              this.chartHeight
            );
          }
        );
      },
    });
  }

  authme() {
    this.authService.authme().subscribe({
      next: (res) => {
        const user = res.data;
        if (user) {
          this.userId = user.id;
          this.getTotalSales();
        }
      },
      error: (err) => {
        console.error('Error fetching user data:', err);
      },
    });
  }

  ngAfterViewInit(): void {
    this.hasViewInitialized = true;
  }

  goToPage(newPage: number) {
    if (newPage < 1 || newPage > this.totalPages) return;
    this.currentPage = newPage;
    this.updatePagedData();
  }

  getAuctionStatus(auction: any): string {
    if (auction.status === 'Upcoming') return 'Upcoming';
    if (auction.status === 'Cancelled') return 'Cancelled';
    if (auction.status === 'Closed') return 'Closed';
    if (auction.status === 'Completed') return 'Completed';
    if (auction.status === 'Live') return 'Live';
    return 'Unknown';
  }

  getTotalSales() {
    this.auctionService.getTotalSales(this.userId).subscribe({
      next: (res) => {
        const totalSales = res.data;
        console.log('Total Sales:', totalSales);
        this.totalSales = totalSales || 0;
      },
      error: (err) => {
        console.error('Error fetching total sales:', err);
      },
    });
  }
}
