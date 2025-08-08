import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { Observable } from 'rxjs/internal/Observable';
import { environment } from '../../../env/environment';
import { PlaceBid } from '../models/PlaceBid';
import { ErrorHandlerService } from './errorhandler.service';

@Injectable()
export class BiddingService {
  private baseUrl = `${environment.apiUrl}/BidItem`;

  constructor(
    private http: HttpClient,
    private errorHandler: ErrorHandlerService
  ) {}

  fetchHighestBid(auctionId: string): Observable<any> {
    return this.http
      .get(`${this.baseUrl}/HighestBid/${auctionId}`)
      .pipe(catchError(this.errorHandler.handleError));
  }

  getAllBidItems(filter?: any): Observable<any> {
    let params = new HttpParams();

    if (filter) {
      if (filter.amountMin) params = params.set('amountMin', filter.amountMin);
      if (filter.amountMax) params = params.set('amountMax', filter.amountMax);
      if (filter.dateMin) params = params.set('dateMin', filter.dateMin);
      if (filter.dateMax) params = params.set('dateMax', filter.dateMax);
      if (filter.sortBy) params = params.set('sortBy', filter.sortBy);
      if (filter.sortDirection) params = params.set('sortDirection', filter.sortDirection);
      if (filter.name) params = params.set('name', filter.name);
    }
    return this.http
      .get(this.baseUrl,{ params })
      .pipe(catchError(this.errorHandler.handleError));
  }

  getBidItemByBidder(filter?: any): Observable<any> {
    let params = new HttpParams();

    if (filter) {
      if (filter.amountMin) params = params.set('amountMin', filter.amountMin);
      if (filter.amountMax) params = params.set('amountMax', filter.amountMax);
      if (filter.dateMin) params = params.set('dateMin', filter.dateMin);
      if (filter.dateMax) params = params.set('dateMax', filter.dateMax);
      if (filter.sortBy) params = params.set('sortBy', filter.sortBy);
      if (filter.sortDirection) params = params.set('sortDirection', filter.sortDirection);
      if (filter.name) params = params.set('name', filter.name);
    }
    return this.http
      .get(`${this.baseUrl}/ByBidder`,{ params })
      .pipe(catchError(this.errorHandler.handleError));
  }

  placeBidding(payload: PlaceBid): Observable<any> {
    return this.http
      .post(`${this.baseUrl}`, payload)
      .pipe(catchError(this.errorHandler.handleError));
  }

  getBidsByAuctionId(auctionId: string): Observable<any> {
    return this.http
      .get(`${this.baseUrl}/${auctionId}`)
      .pipe(catchError(this.errorHandler.handleError));
  }

  deleteBids(bidId: string): Observable<any> {
    return this.http
      .delete(`${this.baseUrl}/${bidId}`)
      .pipe(catchError(this.errorHandler.handleError));
  }

  updateBidStatus(bidId: string, bidItem: any): Observable<any> {
    return this.http
      .patch(`${this.baseUrl}/${bidId}`, bidItem)
      .pipe(catchError(this.errorHandler.handleError));
  }

  getBidsByBidderId(bidderId: string): Observable<any> {
    return this.http
      .get(`${this.baseUrl}/ByBiddingId/${bidderId}`)
      .pipe(catchError(this.errorHandler.handleError));
  }
}
