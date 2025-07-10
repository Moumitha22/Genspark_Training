import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Chart, ChartData, ChartOptions } from 'chart.js';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { NgChartsModule } from 'ng2-charts';
import { DashboardService } from '../../core/services/dashboard.service';
import { ListerDashboardModel } from '../../models/lister-dashboard.model';
import { ChartItemModel } from '../../models/chart-item.model';
import { ChartDataset } from 'chart.js';
import { ListerInquiriesComponent } from '../lister-inquiries/lister-inquiries';
import { UserService } from '../../core/services/user.service';

Chart.register(ChartDataLabels);

@Component({
  selector: 'app-lister-dashboard',
  standalone: true,
  imports: [CommonModule, NgChartsModule, ListerInquiriesComponent],
  templateUrl: './lister-dashboard.html',
  styleUrls: ['./lister-dashboard.css']
})
export class ListerDashboardComponent implements OnInit {
  private dashboardService = inject(DashboardService);
  private userService = inject(UserService);

  userName: string = '';
  data: ListerDashboardModel | null = null;
  loading = true;

  typeChartData: ChartData<'pie', number[], string> = { labels: [], datasets: [] };
  purposeChartData: ChartData<'doughnut', number[], string> = { labels: [], datasets: [] };
  statusChartData: ChartData<'bar', number[], string> = { labels: [], datasets: [] };
  typeBarChartData: ChartData<'bar', number[], string> = { labels: [], datasets: [] };

  chartOptions: ChartOptions = {
    responsive: true,
    plugins: {
      datalabels: {
        color: '#fff',
        font: { weight: 'bold', size: 11 },
        formatter: (value, context) => {
          const dataset = context.chart.data.datasets[0].data as number[];
          const total = dataset.reduce((sum, val) => sum + val, 0);
          const percentage = total > 0 ? (value / total * 100).toFixed(1) : '0.0';
          return `${percentage}%`;
        }
      },
      legend: { display: true, position: 'bottom' }
    }
  };



  ngOnInit(): void {
    this.userService.user$.subscribe(user => {
      if (user) {
        this.userName = user.name;
      }
    });
    this.dashboardService.getListerDashboard().subscribe({
      next: res => {
        this.data = res;
        this.setupCharts();
        this.loading = false;
      },
      error: err => {
        console.error('Failed to load dashboard', err);
        this.loading = false;
      }
    });
  }


  private setupCharts() {
    const typeItems = this.data?.propertyTypeChart ?? [];
    const purposeItems = this.data?.propertyPurposeChart ?? [];
    const statusItems = this.data?.propertyStatusChart ?? [];

    const typeColors = this.generateColors(typeItems.length);
    const purposeColors = this.generateColors(purposeItems.length);
    const statusColors = this.generateColors(statusItems.length);

    this.typeChartData = this.toChartData<'pie'>(typeItems, typeColors);
    this.typeBarChartData = this.toBarChartData(typeItems, typeColors);
    this.purposeChartData = this.toChartData<'doughnut'>(purposeItems, purposeColors);
    this.statusChartData = this.toBarChartData(statusItems, statusColors);

    console.log(this.typeChartData);
  }


 private toChartData<T extends 'pie' | 'doughnut'>(
  items: ChartItemModel[],
  colors: string[]
): ChartData<T, number[], string> {
  return {
    labels: items.map(i => i.label),
    datasets: [
      {
        label: 'Properties', 
        data: items.map(i => i.value),
        backgroundColor: colors,
        borderWidth: 1
      } as any
    ]
  };
}


  private toBarChartData(
    items: ChartItemModel[],
    colors: string[]
  ): ChartData<'bar', number[], string> {
    return {
      labels: items.map(i => i.label),
      datasets: [
        {
          label: 'Properties',
          data: items.map(i => i.value),
          backgroundColor: colors
        }
      ]
    };
  }

  private generateColors(count: number): string[] {

    const fixedColors = [
      '#e74a3b', // Bright Red
      '#4e73df', // Blue (Matches .card-properties)
      '#f6c23e',  // Yellow (Matches .card-inquiries)
      '#1cc88a', // Green (Matches .card-sale)
      '#6610f2', // Indigo
      '#0dcaf0', // Sky Blue
      '#6f42c1', // Purple
      '#36b9cc', // Teal (Matches .card-rent)
      '#20c997', // Aqua Green
      '#fd7e14', // Orange
    ];

    // const fixedColors = [
    //   '#4e73df', // Blue (Matches .card-properties)
    //   '#1cc88a', // Green (Matches .card-sale)
    //   '#36b9cc', // Teal (Matches .card-rent)
    //   '#f6c23e'  // Yellow (Matches .card-inquiries)
    // ];

    return Array.from({ length: count }, (_, i) => fixedColors[i % fixedColors.length]);
  }

}

