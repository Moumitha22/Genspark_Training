import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Chart, ChartData, ChartOptions } from 'chart.js';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { NgChartsModule } from 'ng2-charts';
import { DashboardService } from '../../core/services/dashboard.service';
import { AdminDashboardModel } from '../../models/admin-dashboard.model';
import { ChartItemModel } from '../../models/chart-item.model';
import { UsersComponent } from '../users-list/users-list';
import { AdminPropertyListComponent } from '../admin-properties-list/admin-properties-list';

Chart.register(ChartDataLabels);

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, NgChartsModule,  AdminPropertyListComponent],
  templateUrl: './admin-dashboard.html',
  styleUrls: ['./admin-dashboard.css']
})
export class AdminDashboardComponent implements OnInit {
  private dashboardService = inject(DashboardService);

  data: AdminDashboardModel | null = null;
  totalForSale = 0;
  totalForRent = 0;
  loading = true;

  typeChartData: ChartData<'pie', number[], string> = { labels: [], datasets: [] };
  purposeChartData: ChartData<'doughnut', number[], string> = { labels: [], datasets: [] };
  statusChartData: ChartData<'bar', number[], string> = { labels: [], datasets: [] };

  chartOptions: ChartOptions = {
    responsive: true,
    plugins: {
      datalabels: {
        color: '#fff',
        font: { weight: 'bold', size: 11 },
        formatter: (value, context) => {
          const dataset = context.chart.data.datasets[0].data as number[];
          const total = dataset.reduce((sum, val) => sum + val, 0);
          return total ? `${((value / total) * 100).toFixed(1)}%` : '0%';
        }
      },
      legend: { display: true, position: 'bottom' }
    }
  };

  ngOnInit(): void {
    this.dashboardService.getAdminDashboard().subscribe({
      next: res => {
        this.data = res;
        this.setupCharts();
        this.loading = false;
      },
      error: err => {
        console.error('Failed to load admin dashboard', err);
        this.loading = false;
      }
    });
  }

  private setupCharts() {
    const typeColors = this.generateColors(this.data!.propertyTypeChart.length);
    const purposeColors = this.generateColors(this.data!.propertyPurposeChart.length);
    const statusColors = this.generateColors(this.data!.propertyStatusChart.length);
const purposeItems = this.data!.propertyPurposeChart;

this.totalForSale = purposeItems.find(i => i.label.toLowerCase() === 'sale')?.value ?? 0;
this.totalForRent = purposeItems.find(i => i.label.toLowerCase() === 'rent')?.value ?? 0;

this.purposeChartData = this.toChartData<'doughnut'>(purposeItems, purposeColors);

    // this.purposeChartData = this.toChartData<'doughnut'>(this.data!.propertyPurposeChart, purposeColors);
    this.typeChartData = this.toChartData<'pie'>(this.data!.propertyTypeChart, typeColors);
    this.statusChartData = this.toChartData<'bar'>(this.data!.propertyStatusChart, statusColors);
  }

  private toChartData<T extends 'pie' | 'doughnut' | 'bar'>(
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

  private generateColors(count: number): string[] {
    const fixedColors = [
      '#e74a3b', '#4e73df', '#f6c23e', '#1cc88a',
      '#6610f2', '#0dcaf0', '#6f42c1', '#36b9cc',
      '#20c997', '#fd7e14'
    ];
    return Array.from({ length: count }, (_, i) => fixedColors[i % fixedColors.length]);
  }
}
