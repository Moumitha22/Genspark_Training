import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { map, Observable } from 'rxjs';
import { AdminDashboardModel } from '../../models/admin-dashboard.model';
import { ListerDashboardModel } from '../../models/lister-dashboard.model';

@Injectable()
export class DashboardService {
  private baseUrl = `${environment.apiBaseUrl}/api/v1/Dashboard`;

  private http = inject(HttpClient);

  getAdminDashboard(): Observable<AdminDashboardModel> {
    return this.http.get<{data : AdminDashboardModel}>(`${this.baseUrl}/admin`)
    .pipe(map(response => response.data));
  }


  getListerDashboard(): Observable<ListerDashboardModel> {
    return this.http.get<{ data: ListerDashboardModel }>(`${this.baseUrl}/lister`)
        .pipe(map(response => response.data));
    }

}
