import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, of, throwError } from 'rxjs';
import { environment } from '../../../env/environment';
import { AddUser } from '../models/AddUser';
import { ChangePasswordModel } from '../models/ChangePassword';
import { User } from '../models/User';
import { ErrorHandlerService } from './errorhandler.service';
import { LoginRequest } from '../models/LoginRequest';
import { ForgetPasswordRequest } from '../models/ForgetPassword';
import { SearchUser } from '../models/SearchUser';

@Injectable()
export class UserService {
  constructor(
    private http: HttpClient,
    private errorHandler: ErrorHandlerService
  ) {}

  private baseUrl = `${environment.apiUrl}/User`;
  getByEmail(email: string): Observable<any> {
    return this.http
      .get<any>(`${this.baseUrl}/GetByEmail/?email=${email}`)
      .pipe(catchError(this.errorHandler.handleError));
  }

  registerNewuser(payload: AddUser) {
    return this.http
      .post<any>(`${this.baseUrl}`, payload)
      .pipe(catchError(this.errorHandler.handleError));
  }

  getAllUsers(): Observable<any> {
    return this.http
      .get<any>(`${this.baseUrl}/GetAll`)
      .pipe(catchError(this.errorHandler.handleError));
  }

  changeUserPassword(payload: ChangePasswordModel): Observable<any> {
    return this.http
      .patch(`${this.baseUrl}/change-password`, payload, {
        headers: { 'Content-Type': 'application/json' },
        responseType: 'text' as 'json',
      })
      .pipe(catchError(this.errorHandler.handleError));
  }

  updateUser(payload: any, userId: string): Observable<any> {
    return this.http
      .put(`${this.baseUrl}/UpdateUser?id=${userId}`, payload)
      .pipe(catchError(this.errorHandler.handleError));
  }

  deleteUser(payload: any): Observable<any> {
    return this.http
      .delete(`${environment.apiUrl}/UserAccountStatus`, {
        headers: { 'Content-Type': 'application/json' },
        body: payload,
      })
      .pipe(catchError(this.errorHandler.handleError));
  }

  forgetPassword(payload: ForgetPasswordRequest): Observable<any> {
    return this.http
      .patch(`${this.baseUrl}/forget-password`, payload)
      .pipe(catchError(this.errorHandler.handleError));
  }

  getByUserId(userId: string): Observable<any> {
    return this.http
      .get<any>(`${this.baseUrl}/${userId}`)
      .pipe(catchError(this.errorHandler.handleError));
  }

  getSearchUsers(searchQuery: any): Observable<any> {
    let params: any = {};
    if (searchQuery.SearchTerm) {
      params.SearchTerm = searchQuery.SearchTerm;
    }
    if (searchQuery.SortBy) {
      params.SortBy = searchQuery.SortBy;
    }
    return this.http.get<any>(`${this.baseUrl}/search`, { params }).pipe(
      catchError((error) => {
        if (error.status === 404) {
          return of({ success: true, message: 'No users found', data: [] });
        }
        return this.errorHandler.handleError(error);
      })
    );
  }

  loginSuperAdmin(payload: any): Observable<any> {
    return this.http
      .post<any>(`${this.baseUrl}/super-admin-login`, payload)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          if (error.status === 401) {
            return throwError(() => new Error('Invalid credentials'));
          }
          return this.errorHandler.handleError(error);
        })
      );
    }

    registerAdmin(payload: any): Observable<any> {
    return this.http
      .post<any>(`${this.baseUrl}/register-admin`, payload)
      .pipe(catchError(this.errorHandler.handleError));
    }
}
