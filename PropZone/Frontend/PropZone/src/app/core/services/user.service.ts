import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { BehaviorSubject, Observable } from 'rxjs';
import { User } from '../../models/user.model';

@Injectable()
export class UserService {
  private baseUrl = `${environment.apiBaseUrl}/api/v1/User`;

  private currentUserSubject = new BehaviorSubject<User | null>(null);
  user$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {}

  getCurrentUser(): Observable<User> {
    return this.http.get<User>(`${this.baseUrl}/me`);
  }

  loadCurrentUser(): void {
    this.getCurrentUser().subscribe({
      next: (res: any) => this.currentUserSubject.next(res.data as User),
      error: () => this.currentUserSubject.next(null)
    });
  }

  getAllUsers(): Observable<{data: User[]}> {
    return this.http.get<{data :User[]}>(`${this.baseUrl}`);
  }

  getUserById(userId: string): Observable<User> {
    return this.http.get<User>(`${this.baseUrl}/${userId}`);
  }

  getUserByEmail(email: string): Observable<User> {
    return this.http.get<User>(`${this.baseUrl}/email/${email}`);
  }

  updateUser(userId: string, dto: any): Observable<{data: User}> {
    return this.http.put<{data: User}>(`${this.baseUrl}/${userId}`, dto);
  }

  updateUserStatus(userId: string, disable: boolean): Observable<any> {
    return this.http.put(`${this.baseUrl}/${userId}/status?disable=${disable}`, {});
  }

}
