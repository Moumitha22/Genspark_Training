import { Routes } from '@angular/router';
import { UserDashboard } from './user-dashboard/user-dashboard';
import { AddUser } from './add-user/add-user';

export const routes: Routes = [
    {path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    {path:'dashboard',component:UserDashboard},
    {path:'add-user',component:AddUser},
];
