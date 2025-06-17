import { Routes } from '@angular/router';
import { First } from './first/first';
import { Login } from './login/login';
import { MyProducts } from './myproducts/myproducts';
import { Home } from './home/home';
import { Recipes } from './recipes/recipes';
import { Profile } from './profile/profile';
import { AuthGuard } from './auth-guard';

export const routes: Routes = [
    {path:'landing',component:First},
    {path:'login',component:Login},
    {path:'products',component:MyProducts},
    {path:'home/:un',component:Home, children:
        [
            {path:'recipes', component: Recipes},
            {path:'products',component:MyProducts}
        ]
    },
    {path:'profile',component:Profile,canActivate:[AuthGuard]}
];