import {
  HttpClient,
  HttpErrorResponse,
  HttpParams,
} from '@angular/common/http';
import { ErrorHandler, Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { UpdateWinningIdRequest } from '../models/WinningBidUpdate';
import { environment } from '../../../env/environment';
import { ErrorHandlerService } from './errorhandler.service';
import { PaginatedAuction } from '../models/PaginatedAuction';

@Injectable()
export class AuctionService {
  private baseUrl = `${environment.apiUrl}/AuctionItem`;

  constructor(
    private http: HttpClient,
    private errorHandler: ErrorHandlerService
  ) {}

  getAuctionBySeller(): Observable<any> {
    return this.http
      .get<any>(`${this.baseUrl}/BySeller`)
      .pipe(catchError(this.errorHandler.handleError));
  }

  getAllAuctions(): Observable<any> {
    return this.http
      .get<any>(`${this.baseUrl}`)
      .pipe(catchError(this.errorHandler.handleError));
  }

  createAuction(data: FormData): Observable<any> {
    return this.http
      .post(this.baseUrl, data)
      .pipe(catchError(this.errorHandler.handleError));
  }

  getAuctions(paginatedParams: PaginatedAuction): Observable<any> {
    console.log("🔍 Received in service:", paginatedParams); 
    let queryParams = new HttpParams()
      .set('Page', paginatedParams.page)
      .set('PageSize', paginatedParams.pageSize)
      .set('SortBy', paginatedParams.sortBy || 'name')
      .set('SortDirection', paginatedParams.sortDirection || 'asc');

    if (paginatedParams.name)
      queryParams = queryParams.set('Name', paginatedParams.name);
    if (paginatedParams.status)
      queryParams = queryParams.set('Status', paginatedParams.status);
    if (paginatedParams.startTime)
      queryParams = queryParams.set('StartTime', paginatedParams.startTime);
    if (paginatedParams.endTime)
      queryParams = queryParams.set('EndTime', paginatedParams.endTime);
    if (paginatedParams.sellerId)
      queryParams = queryParams.set('SellerId', paginatedParams.sellerId);
    if (paginatedParams.startingPriceMin != null)
      queryParams = queryParams.set('StartingPriceMin', paginatedParams.startingPriceMin.toString());
    if (paginatedParams.startingPriceMax != null)
      queryParams = queryParams.set('StartingPriceMax', paginatedParams.startingPriceMax.toString());
    if (paginatedParams.reservePriceMin != null)
      queryParams = queryParams.set('ReservePriceMin', paginatedParams.reservePriceMin.toString());
    if (paginatedParams.reservePriceMax != null)
      queryParams = queryParams.set('ReservePriceMax', paginatedParams.reservePriceMax.toString());
    if (paginatedParams.hasFileAttachments)
      queryParams = queryParams.set('HasFileAttachments', paginatedParams.hasFileAttachments);
    if (paginatedParams.fileName)
      queryParams = queryParams.set('FileName', paginatedParams.fileName);
    
    return this.http
      .get(`${this.baseUrl}/PagedData`, {
        params: queryParams,
      })
      .pipe(catchError(this.errorHandler.handleError));
  }

  getAuctionByAuctionId(auctionId: string): Observable<any> {
    return this.http
      .get(`${this.baseUrl}/${auctionId}`)
      .pipe(catchError(this.errorHandler.handleError));
  }

  getfile(auctionId: string, fileName: string): Observable<any> {
    return this.http
      .get(`${this.baseUrl}/download/${auctionId}/${fileName}`, {
        responseType: 'blob',
      })
      .pipe(catchError(this.errorHandler.handleError));
  }

  updateWinningId(payload: UpdateWinningIdRequest): Observable<any> {
    return this.http
      .patch(`${this.baseUrl}/UpdateWinningId`, payload)
      .pipe(catchError(this.errorHandler.handleError));
  }

  getLiveAuctions(): Observable<any> {
    return this.http
      .get(`${this.baseUrl}/GetActiveAuctions`)
      .pipe(catchError(this.errorHandler.handleError));
  }

  UpdateAuction(auctionId: string, updatedAuction: any): Observable<any> {
    return this.http.put(
      `${this.baseUrl}/UpdateAuctionItem?id=${auctionId}`,
      updatedAuction
    );
  }

  cancelAuction(auctionId: string, newStatus: string): Observable<any> {
    const params = { newStatus };
    return this.http
      .patch(`${this.baseUrl}/${auctionId}/status`, null, { params })
      .pipe(catchError(this.errorHandler.handleError));
  }

  deleteAuction(auctionId: string): Observable<any> {
    return this.http
      .delete(`${this.baseUrl}/${auctionId}`)
      .pipe(catchError(this.errorHandler.handleError));
  }

  getTotalSales(sellerId: string): Observable<any> {
    return this.http
      .get(`${this.baseUrl}/GetTotalSales/${sellerId}`)
      .pipe(catchError(this.errorHandler.handleError));
  }
}
