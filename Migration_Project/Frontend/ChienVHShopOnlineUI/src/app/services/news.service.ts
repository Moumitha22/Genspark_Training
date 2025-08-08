import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { NewsModel } from '../models/news';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class NewsService {
  private baseUrl = `${environment.apiBaseUrl}/api/News`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<NewsModel[]> {
    return this.http.get<NewsModel[]>(this.baseUrl);
  }

  getById(id: number): Observable<NewsModel> {
    return this.http.get<NewsModel>(`${this.baseUrl}/${id}`);
  }

  create(formData: FormData): Observable<NewsModel> {
    return this.http.post<NewsModel>(this.baseUrl, formData);
  }

  update(id: number, formData: FormData): Observable<NewsModel> {
    return this.http.put<NewsModel>(`${this.baseUrl}/${id}`, formData);
  }

  delete(id: number): Observable<NewsModel> {
    return this.http.delete<NewsModel>(`${this.baseUrl}/${id}`);
  }

  downloadCsv(): Observable<Blob> {
      return this.http.get(`${this.baseUrl}/export/csv`, {
      responseType: 'blob'
    });
  }

  downloadExcel(): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/export/excel`, {
      responseType: 'blob'
    });
  }


}
