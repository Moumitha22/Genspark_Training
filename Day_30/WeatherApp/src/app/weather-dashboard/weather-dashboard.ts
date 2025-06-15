import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WeatherService } from '../services/weather';
import { CitySearchComponent } from '../city-search/city-search';
import { WeatherCardComponent } from '../weather-card/weather-card';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-weather-dashboard',
  standalone: true,
  imports: [CommonModule, CitySearchComponent, WeatherCardComponent],
  templateUrl: './weather-dashboard.html',
  styleUrls: ['./weather-dashboard.css']
})
export class WeatherDashboardComponent {
  weather$: Observable<any>;

  constructor(private weatherService: WeatherService) {
    this.weather$ = this.weatherService.weather$;
  }
}
