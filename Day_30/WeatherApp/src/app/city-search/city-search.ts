import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { WeatherService } from '../services/weather';

@Component({
  selector: 'app-city-search',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './city-search.html',
  styleUrls: ['./city-search.css']
})
export class CitySearchComponent {
  city: string = '';

  constructor(private weatherService: WeatherService) {}

  search() {
    if (this.city.trim()) {
      this.weatherService.fetchWeather(this.city);
    }
  }
}
