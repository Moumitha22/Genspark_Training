import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { jwtDecode } from "jwt-decode";
import { catchError, map, Observable, of, tap } from "rxjs";
import { environment } from "../../../env/environment";
import { ErrorHandlerService } from "./errorhandler.service";


@Injectable()
export class AuthService{

    private http = inject(HttpClient);
    private errorHandler = inject(ErrorHandlerService)
    private router = inject(Router)
    private apiUrl = `${environment.apiUrl}/Authentication`

    getAccessTokenByRefreshToken(): Observable<any> {
        const refreshToken = this.getToken('refresh_token');
        return this.http.post<any>(
        `${this.apiUrl}/refresh`,
        JSON.stringify(refreshToken), 
        { headers: { 'Content-Type': 'application/json' } }
        ).pipe(
        tap(res => {
            localStorage.setItem('access_token', res.data.token);
            localStorage.setItem('refresh_token', res.data.refreshToken);
        }),
        map(res => res.data.token),
        catchError(this.errorHandler.handleError)
        )
    }


    setToken(type: string, token: string): void {
        localStorage.setItem(type, token);
    }
    getToken(type:string): string | null {
        return localStorage.getItem(type);
    }
    getUserRole(): string | null {
        const token = this.getToken('access_token');
        if (!token) return null;
        return jwtDecode<any>(token)?.role || null;
    }

    authme(): Observable<any>{
        return this.http.get(`${this.apiUrl}/me`)
        .pipe(catchError(this.errorHandler.handleError))
    }

    getEmailFromToken(): string | null {
        const token = this.getToken('access_token');
        if (!token) return null;

        try {
            const decoded: any = jwtDecode(token);
            return decoded.nameid || null;
        } catch {
            return null;
        }
    }
    
    
    getUserIdFromToken(): string | null {
        const token = this.getToken('access_token');
        if (!token) return null;

        try {
            const decoded: any = jwtDecode(token);
            return decoded.UserId || null;
        } catch {
            return null;
        }
    }

    isAuthenticated(): boolean {
        const token = this.getToken('access_token');
        if (!token) return false;
        const decoded: any = jwtDecode(token);
        const now = Math.floor(Date.now() / 1000);
        if(decoded.exp > now) return true;
        return false;
    }

    isTokenValid(token: string): boolean {
    try {
      const decoded: any = jwtDecode(token);
      const exp = decoded.exp;
      const now = Math.floor(Date.now() / 1000);
      return exp > now;
    } catch {
      return false;
    }
    }

    clearToken() {
        localStorage.removeItem('access_token');
        localStorage.removeItem('refresh_token');
    }

    

    logout(): Observable<any> {
    const refreshToken = this.getToken('refresh_token');
    return this.http.post(`${this.apiUrl}/logout`, JSON.stringify(refreshToken), { headers: { 'Content-Type': 'application/json' } })
      .pipe(
        tap(() => {
          this.clearToken();
          this.router.navigate(['/login']);
        }),
        catchError(error => {
          console.error('Logout error (could be network or server issue, still proceeding with local cleanup):', error);
          this.clearToken();
          this.router.navigate(['/login']);
          return of(null);
        })
      );
  }


}