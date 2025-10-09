import { inject, Injectable } from "@angular/core";
import { CanActivate, Router, UrlTree } from "@angular/router";
import { catchError, map, Observable, of } from "rxjs";
import { ListerProfileService } from "../services/lister-profile.service";
import { NotificationService } from "../services/notification.service";

@Injectable({ providedIn: 'root' })
export class ProfileCompletionGuard implements CanActivate {
  private profile= inject(ListerProfileService);
  private notifier = inject(NotificationService);
  private router= inject(Router);

  canActivate(): Observable<boolean | UrlTree> {
    return this.profile.checkProfileCompletion().pipe(
      map(res => {
        if (res.isComplete) return true;
        this.notifier.warning('Please complete your profile before posting.');
        return this.router.createUrlTree(['/profile']);
      }),
      catchError(() => {
        this.notifier.error('Error checking profile.');
        return of(this.router.createUrlTree(['/']));
      })
    );
  }
}
