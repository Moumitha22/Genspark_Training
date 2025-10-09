import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable()
export class NominatimService {
  private http = inject(HttpClient);

  geocode(locality: string, city: string, state: string) {
    const query = encodeURIComponent(`${locality}, ${city}, ${state}`);
    const url = `https://nominatim.openstreetmap.org/search?q=${query}&format=json`;

    return this.http.get<any[]>(url);
  }

  reverseGeocode(lat: number, lon: number) {
    const url = `https://nominatim.openstreetmap.org/reverse?lat=${lat}&lon=${lon}&format=json`;
    return this.http.get<any>(url);
  }

}
