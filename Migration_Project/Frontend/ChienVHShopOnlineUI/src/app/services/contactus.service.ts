import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';

export interface ContactUsRequest {
  name: string;
  email: string;
  phone: string;
  content: string;
  captchaToken: string;
}

@Injectable({
  providedIn: 'root',
})
export class ContactUsService {  
    private baseUrl = `${environment.apiBaseUrl}/api/ContactUs`;

    constructor(private http: HttpClient) {}

    submitContact(contact: ContactUsRequest): Observable<any> {
        return this.http.post(`${this.baseUrl}`, contact);
    }
}
