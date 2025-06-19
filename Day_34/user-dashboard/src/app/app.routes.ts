import { Routes } from '@angular/router';
import { UserDashboard } from './user-dashboard/user-dashboard';
import { AddUser } from './add-user/add-user';
import { UserListComponent } from './user-list/user-list';

export const routes: Routes = [
    { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    {path:'dashboard',component:UserDashboard},
    {path:'add-user',component:AddUser},
    {path:'user-list',component:UserListComponent}
];
