import { Component, viewChild, ViewChild } from '@angular/core';

import { ChartComponent, ChartType, NgApexchartsModule } from 'ng-apexcharts';
import { WelcomeCard } from '../../../shared/components/welcome-card/welcome-card';
import { UserService } from '../../../core/services/user.service';
import { AuctionService } from '../../../core/services/auction.service';
import { buildMonthlyChart } from '../../../shared/utils/dashboard-utils';
import { observeThemeChanges } from '../../../shared/utils/theme-utils';
import { User } from '../../../core/models/User';
import { NgOptimizedImage } from '@angular/common';
import { SummaryCard } from '../../../shared/components/summary-card/summary-card';

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
  selector: 'app-admin-dashboard',
  imports: [WelcomeCard, NgApexchartsModule, SummaryCard],
  templateUrl: './admin-dashboard.html',
})
export class AdminDashboard {
  @ViewChild('auctionChart') auctionChart!: ChartComponent;
  @ViewChild('usersCreation') usersCreationChart!: ChartComponent;

  totalUsers = 0;
  totalSellers = 0;
  totalBidders = 0;
  totalAdmins = 0;

  auctionData: any[] = [];
  usersData: User[] = [];
  auctionchartOptions: Partial<ChartOptions> | null = null;
  userCreationChartOptions: Partial<ChartOptions> | null = null;

  filterationValue = 'createdAt';
  type: ('line' | 'bar' | 'area')[] = ['line', 'bar', 'area'];
  hasViewInitialized = false;
  chartHeight = 400;
  constructor(
    private userService: UserService,
    private auctionService: AuctionService
  ) {}

  ngAfterViewInit(): void {
    this.hasViewInitialized = true;
  }

  ngOnInit() {
    this.loadUsers();
    this.loadAuctions();
    this.loadUserCreationStats();
  }

  loadUsers() {
    this.userService.getAllUsers().subscribe({
      next: (response) => {
        const users: any[] = response?.$values || [];
        this.totalUsers = users.length;
        this.totalSellers = users.filter(
          (u: any) => u.role === 'Seller'
        ).length;
        this.totalBidders = users.filter(
          (u: any) => u.role === 'Bidder'
        ).length;
        this.totalAdmins = users.filter((u: any) => u.role === 'Admin').length;
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  getAuctionStatus(auction: any): string {
    if (auction.status === 'Upcoming') return 'Upcoming';
    if (auction.status === 'Cancelled') return 'Cancelled';
    if (auction.status === 'Closed') return 'Closed';
    if (auction.status === 'Completed') return 'Completed';
    if (auction.status === 'Live') return 'Live';
    return 'Unknown';
  }

  loadAuctions() {
    this.auctionService.getAllAuctions().subscribe({
      next: (res) => {
        const auctions = res?.data.$values || [];
        this.auctionData = auctions;
        this.auctionchartOptions = buildMonthlyChart(
          auctions,
          'createdAt',
          this.type[2],
          this.chartHeight
        );
        observeThemeChanges(
          this.auctionData,
          this.filterationValue,
          () => {},
          '',
          this.hasViewInitialized,
          this.auctionChart,
          '',
          this.auctionchartOptions,
          '',
          0,
          () => {},
          () => {
            this.auctionchartOptions = buildMonthlyChart(
              this.auctionData,
              this.filterationValue,
              this.type[2],
              this.chartHeight
            );
          }
        );
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  loadUserCreationStats() {
    this.userService.getAllUsers().subscribe({
      next: (res) => {
        const users = res?.$values || [];
        this.usersData = users;
        this.userCreationChartOptions = buildMonthlyChart(
          users,
          'createdAt',
          this.type[1],
          this.chartHeight
        );
        observeThemeChanges(
          this.usersData,
          'createdAt',
          () => {},
          '',
          this.hasViewInitialized,
          this.usersCreationChart,
          '',
          this.userCreationChartOptions,
          '',
          0,
          () => {},
          () => {
            this.userCreationChartOptions = buildMonthlyChart(
              this.usersData,
              this.filterationValue,
              this.type[1],
              this.chartHeight
            );
          }
        );
      },
      error: (err) => {
        console.log(err);
      },
    });
  }
}
