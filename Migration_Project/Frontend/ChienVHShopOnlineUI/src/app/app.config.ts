import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { ProductService } from './services/product.service';
import { CategoryService } from './services/category.service';
import { ModelService } from './services/model.service';
import { ColorService } from './services/color.service';
import { StorageService } from './services/storage.service';
import { CartService } from './services/cart.service';
import { NewsService } from './services/news.service';
import { OrderService } from './services/order.service';
import { AuthInterceptor } from './interceptors/auth.interceptor';

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
    ProductService,
    CategoryService,
    ModelService,
    ColorService,
    StorageService,
    CartService,
    OrderService,
    NewsService
  ]
};
