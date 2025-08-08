import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { catchError, Observable, tap } from "rxjs";
import { AuthService } from "./auth.service";
import { environment } from "../../../env/environment";
import { ErrorHandlerService } from "./errorhandler.service";
import { LoginRequest } from "../models/LoginRequest";



@Injectable()
export class LoginService{

    private loginUrl = `${environment.apiUrl}/Authentication`;

    constructor(private http:HttpClient, private authService: AuthService, private erroHandler:ErrorHandlerService){}

    login(payload: LoginRequest): Observable<any> {
    return this.http.post<any>(`${this.loginUrl}/login`, payload).pipe(
        tap(res => {
        const token = res?.data?.token;
        const refreshToken = res?.data?.refreshToken;
        if (token) {
            this.authService.setToken('access_token', token);
            this.authService.setToken('refresh_token', refreshToken)
        }
    }),
    catchError(this.erroHandler.handleError)
        
    );
    }
}