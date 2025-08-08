import { Injectable } from '@angular/core';
import { environment } from '../../../env/environment';
import { ErrorHandlerService } from './errorhandler.service';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable } from 'rxjs';
import { AuctionDeleteRequest } from '../models/AuctionDeleteRequest';

@Injectable()
export class AuctionDeleteService {
  private baseUrl = `${environment.apiUrl}/AuctionDeleteRequest`;

  constructor(
    private http: HttpClient,
    private errorHandler: ErrorHandlerService
  ) {}

  requestAuctionDelete(payload: AuctionDeleteRequest): Observable<any> {
    return this.http
      .post(`${this.baseUrl}`, payload)
      .pipe(catchError(this.errorHandler.handleError));
  }

  rejectAuctionDeleteRequest(payload: any): Observable<any> {
    return this.http
      .patch(`${this.baseUrl}/reject`, payload)
      .pipe(catchError(this.errorHandler.handleError));
  }

  approveAuctionDeleteRequest(auctionId: any): Observable<any> {
    return this.http
      .patch(`${this.baseUrl}/${auctionId}/approve`, null)
      .pipe(catchError(this.errorHandler.handleError));
  }
}
