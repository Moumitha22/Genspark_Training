import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProductModel } from '../models/product';
import { environment } from '../environments/environment';

@Injectable()
export class ProductService {
  private baseUrl = `${environment.apiBaseUrl}/api/Products`;
  private http = inject(HttpClient);

  getProductById(id: number): Observable<ProductModel> {
    return this.http.get<ProductModel>(`${this.baseUrl}/${id}`);
  }

  getAllProducts(): Observable<ProductModel[]> {
    return this.http.get<ProductModel[]>(this.baseUrl);
  }

  getPagedProducts(page: number, pageSize: number): Observable<ProductModel[]> {
    const params = new HttpParams()
      .set('pageNumber', page)
      .set('pageSize', pageSize);

    return this.http.get<ProductModel[]>(`${this.baseUrl}/paged`, { params });
  }

  getPagedProductsByUserId(userId: number, page: number, pageSize: number): Observable<ProductModel[]> {
    const params = new HttpParams()
      .set('pageNumber', page)
      .set('pageSize', pageSize);

    return this.http.get<ProductModel[]>(`${this.baseUrl}/user/${userId}/paged`, { params });
  }

  getPagedProductsByCategory(categoryId: number, page: number, pageSize: number): Observable<ProductModel[]> {
    const params = new HttpParams()
      .set('pageNumber', page)
      .set('pageSize', pageSize);

    return this.http.get<ProductModel[]>(`${this.baseUrl}/category/${categoryId}/paged`, { params });
  }

  createProduct(formData: FormData): Observable<ProductModel> {
    return this.http.post<ProductModel>(this.baseUrl, formData);
  }

  updateProduct(productId: number, formData: FormData): Observable<ProductModel> {
    return this.http.put<ProductModel>(`${this.baseUrl}/${productId}`, formData);
  }

  deleteProduct(id: number): Observable<ProductModel> {
    return this.http.delete<ProductModel>(`${this.baseUrl}/${id}`);
  }
}
