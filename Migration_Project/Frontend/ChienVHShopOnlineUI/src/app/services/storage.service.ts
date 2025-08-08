import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { StorageModel } from '../models/storage';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class StorageService {
  private baseUrl = `${environment.apiBaseUrl}/api/Storage`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<StorageModel[]> {
    return this.http.get<StorageModel[]>(this.baseUrl);
  }

  get(id: number): Observable<StorageModel> {
    return this.http.get<StorageModel>(`${this.baseUrl}/${id}`);
  }

  create(name: string): Observable<StorageModel> {
    return this.http.post<StorageModel>(this.baseUrl, { name });
  }

  update(id: number, name: string): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, { name });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
