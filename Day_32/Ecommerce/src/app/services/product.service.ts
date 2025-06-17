import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProductModel } from '../models/product.model';

@Injectable()
export class ProductService {
  private http = inject(HttpClient);

  getProduct(id:number){
        return this.http.get(`https://dummyjson.com/products/${id}`);
    }

  getProductsBySearch(searchString: string, limit: number, skip: number): Observable<any> {
    const url = `https://dummyjson.com/products/search?q=${searchString}&limit=${limit}&skip=${skip}`;
    return this.http.get<any>(url);
  }

}