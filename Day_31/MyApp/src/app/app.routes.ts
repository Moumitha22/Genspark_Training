import { Routes } from '@angular/router';
import { First } from './first/first';
import { Login } from './login/login';
import { MyProducts } from './myproducts/myproducts';

export const routes: Routes = [
    {path:'home',component:First},
    {path:'login',component:Login},
    {path:'products',component:MyProducts}
];