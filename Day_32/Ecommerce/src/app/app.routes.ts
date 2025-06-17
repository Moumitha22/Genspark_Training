import { Routes } from '@angular/router';
import { Products } from './products/products';
import { About } from './about/about';
import { ProductDetails } from './product-details/product-details';
import { Login } from './login/login';
import { AuthGuard } from './auth-guard';

export const routes: Routes = [
    {path:'login',component:Login},
    {path:'about',component:About},
    {path:'products',component:Products, canActivate:[AuthGuard]},
    {path:'products/:id',component:ProductDetails, canActivate:[AuthGuard]}

];