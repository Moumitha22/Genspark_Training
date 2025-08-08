import { Injectable } from "@angular/core";
import { environment } from "../../../env/environment";
import { catchError, Observable, throwError } from "rxjs";
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { ErrorHandlerService } from "./errorhandler.service";

@Injectable()
export class EAgreementService{
    private baseUrl = `${environment.apiUrl}/EAgreement`;

    constructor(private http: HttpClient, private erroHandler:ErrorHandlerService){}

    getMyAgreements(): Observable<any>{
        return this.http.get<any>(`${this.baseUrl}/myAgreements`)
        .pipe(
            catchError(this.erroHandler.handleError)
        );
    }
   getMyAgreementsFile(eId: string) :Observable<any> {
    return this.http.get(`${this.baseUrl}/${eId}/download`, {
        responseType: 'blob'
    })
    .pipe(
        catchError(this.erroHandler.handleError)
    )
}
}