import { Component } from '@angular/core';
import { WeatherDashboardComponent } from './weather-dashboard/weather-dashboard';
import { RouterOutlet } from '@angular/router';
import { Menu } from './menu/menu';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [WeatherDashboardComponent, RouterOutlet, Menu],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {}
