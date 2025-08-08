import { ApplicationConfig, ErrorHandler, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { LoginService } from './core/services/login.service';
import { AuthService } from './core/services/auth.service';
import { AuthGuard } from './core/guard/auth-guard';
import { LoginGuard } from './core/guard/login-guard';
import { authInterceptor } from './core/interceptor/auth-interceptor';
import { UserService } from './core/services/user.service';
import { AuctionService } from './core/services/auction.service';
import { BiddingService } from './core/services/bidding.service';
import { WalletService } from './core/services/wallet.service';
import { EAgreementService } from './core/services/e-agreement.service';
import { ErrorHandlerService } from './core/services/errorhandler.service';
import { AuctionDeleteService } from './core/services/auctionDelete.service';
import { UserAccountService } from './core/services/userAccount.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    LoginService,
    AuthService,
    AuthGuard,
    LoginGuard,
    AuctionService,
    UserService,
    BiddingService,
    WalletService,
    EAgreementService,
    ErrorHandlerService,
    AuctionDeleteService,
    UserAccountService
  ]
};
