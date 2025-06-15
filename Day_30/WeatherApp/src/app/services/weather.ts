import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, catchError, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WeatherService {
  private baseUrl = 'https://api.openweathermap.org/data/2.5/weather';
  private API_KEY = '6b4b413bb3f01444530b44763be4b2f7'
  private weatherSubject = new BehaviorSubject<any>(null);
  weather$ = this.weatherSubject.asObservable();

  constructor(private http: HttpClient) {}

  fetchWeather(city: string): void {
    const url = `${this.baseUrl}?q=${city}&appid=${this.API_KEY}&units=metric`;
    
    this.http.get(url).pipe(
      catchError(err => {
        this.weatherSubject.next({ error: 'City not found or API error.' });
        return throwError(() => err);
      })
    ).subscribe(data => {
      console.log('Weather data:', data);
      this.weatherSubject.next(data)
    });
  }
}
