import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PropertyModel } from '../../models/property.model';
import { BasicPropertySearchModel } from '../../models/basic-property-search.model';
import { SortModel } from '../../models/sort.model';
import { PaginationInfo, PaginationModel } from '../../models/pagination.model';
import { PropertyAddRequest } from '../../models/property-add-request.model';
import { environment } from '../../environments/environment';

@Injectable()
export class PropertyService {
  private baseUrl = `${environment.apiBaseUrl}/api/v1/Property`;

  constructor(private http: HttpClient) {}

  getPropertyById(propertyId:string):Observable<{data :PropertyModel}> {
    return this.http.get<{ data: PropertyModel}>(`${this.baseUrl}/${propertyId}`);
  }

  basicSearch(
    model: BasicPropertySearchModel, sort: SortModel,pagination: PaginationModel): Observable<PropertyModel[]> {
    let params = new HttpParams();

    if (model.locality) params = params.set('Locality', model.locality);
    if (model.city) params = params.set('City', model.city);
    if (model.listingPurpose) params = params.set('Purpose', model.listingPurpose);
    if (model.propertyTypes?.length) {
      model.propertyTypes.forEach(type => {
        params = params.append('PropertyTypes', type);
      });
    }
    if (model.minPrice !== undefined) params = params.set('MinPrice', model.minPrice.toString());
    if (model.maxPrice !== undefined) params = params.set('MaxPrice', model.maxPrice.toString());
    if (model.keyword) params = params.set('Keyword', model.keyword);
    if (model.minArea !== undefined) params = params.set('MinArea', model.minArea.toString());
    if (model.maxArea !== undefined) params = params.set('MaxArea', model.maxArea.toString());
    if (model.hasImages !== undefined) params = params.set('HasImages', model.hasImages.toString());
    if (model.isDiscountAvailable !== undefined) params = params.set('IsDiscountAvailable', model.isDiscountAvailable.toString());

    if (sort.sortBy) params = params.set('SortBy', sort.sortBy);
    params = params.set('Ascending', sort.ascending.toString());

    params = params
      .set('Page', pagination.page.toString())
      .set('PageSize', pagination.pageSize.toString());
    return this.http.get<PropertyModel[]>(`${this.baseUrl}/search`, { params });
  }

  advancedSearch(model: any, sort: SortModel, pagination: PaginationModel): Observable<PropertyModel[]> {
    const params = new HttpParams()
      .set('SortBy', sort.sortBy)
      .set('Ascending', sort.ascending)
      .set('Page', pagination.page.toString())
      .set('PageSize', pagination.pageSize.toString());

    return this.http.post<any>(
      `${this.baseUrl}/search`,
      model,
      { params }
    );
  }

  createProperty(request: PropertyAddRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}`, request);
  }
  
  updateProperty(propertyId: string, request: PropertyAddRequest): Observable<any> {
    return this.http.put(`${this.baseUrl}/${propertyId}`, request);
  }
  

  getPropertiesByLister(listerId: string, pagination: PaginationModel): Observable<{ data: { items: PropertyModel[], pagination: PaginationInfo } }> {
    const params = new HttpParams()
      .set('Page', pagination.page.toString())
      .set('PageSize', pagination.pageSize.toString());

    return this.http.get<{ data: { items: PropertyModel[], pagination: PaginationInfo } }>(
      `${this.baseUrl}/by-lister/${listerId}`,
      { params }
    );
  }

  getAllProperties(): Observable<{ data: PropertyModel[] }> {
    return this.http.get<{ data: PropertyModel[] }>(`${this.baseUrl}`);
  }

  deleteProperty(id: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }

}
