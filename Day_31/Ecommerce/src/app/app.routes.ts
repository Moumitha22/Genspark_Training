import { Routes } from '@angular/router';
import { Products } from './products/products';
import { About } from './about/about';

export const routes: Routes = [
    {path:'home',component:Products},
    {path:'about',component:About}
];