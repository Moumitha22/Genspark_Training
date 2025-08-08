import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../env/environment';

@Injectable({
  providedIn: 'root',
})
export class UserAccountService {
  private baseUrl = `${environment.apiUrl}/UserAccountStatus`;

  constructor(private http: HttpClient) {}

  getDeleteReasonByEmail(email: string): Observable<any> {
    return this.http.get<any>(
      `${this.baseUrl}/get-delete-reason?email=${encodeURIComponent(email)}`
    );
  }

  getAllDeletedUsers(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/all-deleted-users`);
  }

  revokeDletedUserAccount(email: string): Observable<any> {
    return this.http.patch<any>(
      `${this.baseUrl}/restore?email=${encodeURIComponent(email)}`,
      null
    );
  }
}
