import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { ContactListerRequest } from '../../models/contact-lister-request.model';


@Injectable({
  providedIn: 'root',
})
export class InquiryService {
  private baseUrl = `${environment.apiBaseUrl}/api/v1/Contact`;

  constructor(private http: HttpClient) {}

  contactLister(request: ContactListerRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/lister`, request);
  }

  getAllContactLogs(): Observable<any> {
    return this.http.get(this.baseUrl);
  }

  getPropertyInquiries(propertyId: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/logs/property/${propertyId}`);
  }

  getListerInquiries(listerId: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/logs/lister/${listerId}`);
  }

  getBuyerInquiries(buyerId: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/logs/buyer/${buyerId}`);
  }
}
