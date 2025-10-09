import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { PropertyService } from './core/services/property.service';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptors, withInterceptorsFromDi } from '@angular/common/http';
import { AuthService } from './core/services/auth.service';
import { AuthInterceptor } from './core/interceptors/auth.interceptor';
import { FeatureService } from './core/services/feature.service';
import { ListerProfileService } from './core/services/lister-profile.service';
import { AuthGuard } from './core/guards/auth.guard';
import { ProfileCompletionGuard } from './core/guards/profile.guard';
import { RoleGuard } from './core/guards/role.guard';
import { PropertyFormStateService } from './core/services/property-form-state.service';
import { InquiryService } from './core/services/inquiry.service';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideToastr } from 'ngx-toastr';
import { NotificationService } from './core/services/notification.service';
import { PropertyImageService } from './core/services/property-image.service';
import { UserService } from './core/services/user.service';
import { DashboardService } from './core/services/dashboard.service';
import { PushNotificationService } from './core/services/push-notification.service';
import { NominatimService } from './core/services/nominatim.service';
import { DiscountCodeService } from './core/services/discount-code.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    provideAnimations(),
    provideToastr({
      timeOut: 3000,
      positionClass: 'toast-top-right',
      preventDuplicates: true
    }),
    AuthService,
    RoleGuard,
    ProfileCompletionGuard,
    PropertyService,
    PropertyImageService,
    PropertyFormStateService,
    FeatureService,
    ListerProfileService,
    InquiryService,
    UserService,
    NotificationService,
    DashboardService,
    PushNotificationService,
    NominatimService,
    // AuthGuard,
    DiscountCodeService
  ]
};
