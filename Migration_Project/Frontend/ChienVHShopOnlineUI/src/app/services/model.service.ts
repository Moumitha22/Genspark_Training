import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ProdModel } from '../models/model';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class ModelService {
  private baseUrl = `${environment.apiBaseUrl}/api/Model`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<ProdModel[]> {
    return this.http.get<ProdModel[]>(this.baseUrl);
  }

  get(id: number): Observable<ProdModel> {
    return this.http.get<ProdModel>(`${this.baseUrl}/${id}`);
  }

  create(name: string): Observable<ProdModel> {
    return this.http.post<ProdModel>(this.baseUrl, { name });
  }

  update(id: number, name: string): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, { name });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
