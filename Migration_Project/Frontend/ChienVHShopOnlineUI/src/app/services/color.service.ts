import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ColorModel } from '../models/color';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class ColorService {
  private baseUrl = `${environment.apiBaseUrl}/api/Color`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<ColorModel[]> {
    return this.http.get<ColorModel[]>(this.baseUrl);
  }

  get(id: number): Observable<ColorModel> {
    return this.http.get<ColorModel>(`${this.baseUrl}/${id}`);
  }

  create(name: string): Observable<ColorModel> {
    return this.http.post<ColorModel>(this.baseUrl, { name });
  }

  update(id: number, name: string): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, { name });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
