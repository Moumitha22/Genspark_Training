import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable, tap } from 'rxjs';
import { DiscountCode } from '../../models/discount-code.model';
import { ActiveDiscountCodeFilter } from '../../models/active-discount-code-filter';
import { SortModel } from '../../models/sort.model';
import { PaginationModel } from '../../models/pagination.model';
import { DiscountSimulationRequest } from '../../models/simulate-discount-request.model';
import { DiscountSimulationResponse } from '../../models/simulate-discount-reponse.modal';

@Injectable()
export class DiscountCodeService {
  private baseUrl = `${environment.apiBaseUrl}/api/v1/DiscountCode`;
  constructor(private http: HttpClient) {}

  createDiscount(payload: DiscountCode): Observable<DiscountCode> {
    return this.http.post<DiscountCode>(`${this.baseUrl}`, payload);
  }

  getActiveDiscount(
    filter: ActiveDiscountCodeFilter
  ): Observable<DiscountCode[]> {
    let params = new HttpParams();
    if (filter.typeOfProperty) {
      params = params.set('TypeOfProperty', filter.typeOfProperty);
    }
    if (filter.purposeOfListing) {
      params = params.set('PurposeOfListing', filter.purposeOfListing);
    }
    if (filter.price !== undefined && filter.price !== null) {
      params = params.set('Price', filter.price.toString());
    }
    return this.http
      .get<{ data: DiscountCode[] }>(`${this.baseUrl}/active`, { params })
      .pipe(map((response) => response.data));
  }

  updateDiscount(id: string, payload: DiscountCode): Observable<DiscountCode> {
    return this.http.put<DiscountCode>(`${this.baseUrl}/${id}`, payload);
  }

  updateDeletionStatus(id: string, disable: boolean): Observable<void> {
    return this.http.patch<void>(
      `${this.baseUrl}/${id}?disable=${disable}`,
      {}
    );
  }

  searchDiscounts(
    filterModel: BasicDiscountFilter,
    sort: SortModel,
    pagination: PaginationModel
  ): Observable<{ items: DiscountCode[]; totalItems: number }> {
    let params = new HttpParams();
    Object.entries(filterModel).forEach(([key, value]) => {
      if (value !== undefined && value !== null) {
        const formattedKey = key.charAt(0).toUpperCase() + key.slice(1);
        params = params.set(formattedKey, value.toString());
      }
    });
    params = params.set('SortBy', sort.sortBy || 'CreatedAt');
    params = params.set('Ascending', sort.ascending.toString());
    params = params.set('Page', pagination.page.toString());
    params = params.set('PageSize', pagination.pageSize.toString());

    return this.http
      .get<{
        data: { items: DiscountCode[]; pagination: { totalItems: number } };
      }>(`${this.baseUrl}/search`, { params })
      .pipe(
        map((response) => ({
          items: response.data.items || [],
          totalItems: response.data.pagination?.totalItems || 0,
        }))
      );
  }

  simulateDiscounts(
    payload: DiscountSimulationRequest
  ): Observable<DiscountSimulationResponse> {
    return this.http
      .post<{ data: DiscountSimulationResponse }>(
        `${this.baseUrl}/simulateDiscount`,
        payload
      )
      .pipe(
        map((response) => ({
          originalPrice: response.data.originalPrice,
          discountedPrice: response.data.discountedPrice,
        }))
      );
  }
}
