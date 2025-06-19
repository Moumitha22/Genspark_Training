import { Routes } from '@angular/router';
import { UserList } from './user-list/user-list';
import { WeatherDashboardComponent } from './weather-dashboard/weather-dashboard';

export const routes: Routes = [
    {path:'dashboard',component:WeatherDashboardComponent},
    {path:'users',component:UserList}
];
