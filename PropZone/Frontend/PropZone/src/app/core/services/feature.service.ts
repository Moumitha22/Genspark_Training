import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DynamicFeatureModel } from '../../models/dynamic-feature.model';
import { environment } from '../../environments/environment';
import { FeatureAdminModel} from '../../models/feature-admin.model';

@Injectable()
export class FeatureService {
  private baseUrl = `${environment.apiBaseUrl}/api/v1/FeatureMaster`;
  
  private http = inject(HttpClient);

  createFeature(dto: any): Observable<DynamicFeatureModel> {
    return this.http.post<DynamicFeatureModel>(`${this.baseUrl}/feature`, dto);
  }

  getAllFeaturesForAdmin(): Observable<FeatureAdminModel []> {
    return this.http.get<FeatureAdminModel[]>(`${this.baseUrl}`);
  }


  getApplicableFeatures(purpose: string, propertyType?: string): Observable<{ data: DynamicFeatureModel[] }> {
    let params = new HttpParams().set('listingPurpose', purpose);
    if (propertyType) {
      params = params.set('propertyType', propertyType);
    }

    return this.http.get<{ data: DynamicFeatureModel[] }>(
      `${this.baseUrl}/applicable`, { params }
    );

  }

  deleteFeature(id: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }

  updateFeature(id: string, dto: any): Observable<DynamicFeatureModel> {
    return this.http.put<DynamicFeatureModel>(`${this.baseUrl}/feature/${id}`, dto);
  }
  
}
