import { Component, Renderer2 } from '@angular/core';
import { LucideAngularModule, Moon, Sun } from 'lucide-angular';
import {
  ApexAxisChartSeries,
  ApexChart,
  ChartComponent,
  ApexDataLabels,
  ApexXAxis,
  ApexPlotOptions,
} from 'ng-apexcharts';
import { NotificationComponent } from '../../feature/notification/notification';
import { AuthService } from '../../core/services/auth.service';

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  dataLabels: ApexDataLabels;
  plotOptions: ApexPlotOptions;
  xaxis: ApexXAxis;
};
@Component({
  selector: 'app-header',
  imports: [LucideAngularModule, NotificationComponent],
  templateUrl: './header.html',
})
export class Header {
  readonly darkIcon = Moon;
  readonly lightIcon = Sun;
  role = '';
  constructor(private renderer: Renderer2, private authService: AuthService) {}

  ngOnInit() {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'dark') {
      this.enableDarkTheme();
    } else {
      this.enableLightTheme();
    }

    this.getUserRole();
  }

  getUserRole() {
    this.authService.authme().subscribe({
      next: (res) => {
        this.role = res.data.role;
      },
    });
  }
  enableDarkTheme() {
    this.renderer.addClass(document.documentElement, 'dark');
    localStorage.setItem('theme', 'dark');
  }

  enableLightTheme() {
    this.renderer.removeClass(document.documentElement, 'dark');
    localStorage.setItem('theme', 'light');
  }

  setDarkTheme() {
    this.enableDarkTheme();
  }

  setLightTheme() {
    this.enableLightTheme();
  }
}
