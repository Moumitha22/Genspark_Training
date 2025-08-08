import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CategoryModel } from '../models/category';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private baseUrl = `${environment.apiBaseUrl}/api/Category`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<CategoryModel[]> {
    return this.http.get<CategoryModel[]>(this.baseUrl);
  }

  get(id: number): Observable<CategoryModel> {
    return this.http.get<CategoryModel>(`${this.baseUrl}/${id}`);
  }

  create(name: string): Observable<CategoryModel> {
    return this.http.post<CategoryModel>(this.baseUrl, { name });
  }

  update(id: number, name: string): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, { name });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
