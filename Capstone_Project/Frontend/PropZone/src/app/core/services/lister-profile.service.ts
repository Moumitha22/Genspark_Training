import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ListerProfileRequest } from '../../models/lister-profile.model';

@Injectable({
  providedIn: 'root',
})
export class ListerProfileService {
  private baseUrl = `${environment.apiBaseUrl}/api/v1/ListerProfile`;

  constructor(private http: HttpClient) {}

  checkProfileCompletion(): Observable<{ isComplete: boolean }> {
    return this.http.get<{ isComplete: boolean }>(`${this.baseUrl}/is-complete`);
  }

  createProfile(dto: ListerProfileRequest): Observable<any> {
    return this.http.post(this.baseUrl, dto);
  }

  updateProfile(profileId: string, dto: ListerProfileRequest): Observable<any> {
    return this.http.put(`${this.baseUrl}/${profileId}`, dto);
  }

  getByListerId(listerId: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/by-lister/${listerId}`);
  }
}
