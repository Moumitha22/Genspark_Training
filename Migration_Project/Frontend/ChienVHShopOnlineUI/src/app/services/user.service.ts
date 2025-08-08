import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { UserModel } from '../models/user';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl = `${environment.apiBaseUrl}/api/Users`;

  private currentUserSubject = new BehaviorSubject<UserModel | null>(null);
  user$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {}

  getAllUsers(): Observable<UserModel[]> {
    return this.http.get<UserModel[]>(this.baseUrl);
  }

  getUserById(id: number): Observable<UserModel> {
    return this.http.get<UserModel>(`${this.baseUrl}/${id}`);
  }

  getCurrentUser(): Observable<{ message: string, data: UserModel }> {
    return this.http.get<{ message: string, data: UserModel }>(`${this.baseUrl}/me`);
  }

  loadCurrentUser(): void {
    this.getCurrentUser().subscribe({
     next: (res: any) => {
      this.currentUserSubject.next(res.data as UserModel);
    },
      error: () => this.currentUserSubject.next(null)
    });
  }

//   updateUser(id: number, userDto: UserUpdateDto): Observable<UserModel> {
//     return this.http.put<UserModel>(`${this.baseUrl}/${id}`, userDto);
//   }

  deleteUser(id: number): Observable<UserModel> {
    return this.http.delete<UserModel>(`${this.baseUrl}/${id}`);
  }
}
