import { Injectable } from '@angular/core';
import { environment } from '../../../env/environment';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { ErrorHandlerService } from './errorhandler.service';

@Injectable()
export class WalletService {
  private baseUrl = `${environment.apiUrl}/User`;

  constructor(
    private http: HttpClient,
    private errorHandler: ErrorHandlerService
  ) {}

  getWallet(): Observable<any> {
    return this.http
      .get<any>(`${this.baseUrl}/GetWalletByUserId`)
      .pipe(catchError(this.errorHandler.handleError));
  }

  addFunds(amount: number): Observable<any> {
    return this.http
      .patch<any>(
        `${this.baseUrl}/AddFundsToWallet?amount=${amount}`,
        {},
        { responseType: 'text' as 'json' }
      )
      .pipe(catchError(this.errorHandler.handleError));
  }

  getVirtualWalletHistory(): Observable<any> {
    return this.http
      .get<any>(`${this.baseUrl}/GetWalletHistoryByUserId`)
      .pipe(catchError(this.errorHandler.handleError));
  }

  AddVirtualWallet(newBalance: number): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/AddVirtualWalletToUser`, {
        balance: newBalance,
    })
    .pipe(catchError(this.errorHandler.handleError))
  }
}
