import { Routes } from '@angular/router';
import { Landing } from './pages/landing/landing';
import { PropertyList } from './pages/property-list/property-list';
import { LoginComponent } from './pages/login/login';
import { SignupComponent } from './pages/signup/signup';
import { PostPropertyStepperComponent } from './pages/post-property-stepper/post-property-stepper';
import { RoleGuard } from './core/guards/role.guard';
import { ProfileCompletionGuard } from './core/guards/profile.guard';
import { MyPropertiesListComponent } from './pages/my-properties-list/my-properties-list';
import { ListerInquiriesComponent } from './pages/lister-inquiries/lister-inquiries';
import { BuyerInquiriesComponent } from './pages/buyer-inquiries/buyer-inquiries';
import { PropertyDetailsComponent } from './pages/property-details/property-details';
import { PropertyEditComponent } from './pages/property-edit/property-edit';
import { UploadPropertyImagesComponent } from './pages/upload-images/upload-images';
import { PropertyInquiriesComponent } from './pages/property-inquiries/property-inquiries';
import { ProfilePageComponent } from './pages/profile-page/profile-page';
import { ListerDashboardComponent } from './pages/lister-dashboard/lister-dashboard';
import { AdminDashboardComponent } from './pages/admin-dashboard/admin-dashboard';
import { UsersComponent } from './pages/users-list/users-list';
import { AdminPropertyListComponent } from './pages/admin-properties-list/admin-properties-list';
import { AdminFeaturesListComponent } from './pages/admin-features-list/admin-features-list';
import { AuthGuard } from './core/guards/auth.guard';
import { AdminDiscountList } from './pages/admin-discount-list/admin-discount-list';

export const routes: Routes = [
  { path: '', component: Landing, pathMatch: 'full' },  
  { path: 'landing', component: Landing },
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },
  { path: 'properties', component: PropertyList },
  { path: 'property/:id', component: PropertyDetailsComponent},
  {
    path: 'buyer/inquiries',
    component: BuyerInquiriesComponent,
    canActivate: [RoleGuard],
    data: { expectedRoles: ['Buyer'] }
  },
  {
    path: 'profile',
    component: ProfilePageComponent,
    canActivate: [AuthGuard],
    // data: { expectedRoles: ['Lister', 'Buyer', 'Admin'] }
  },
  { path: 'my-properties', 
    component: MyPropertiesListComponent,
    canActivate: [RoleGuard],
    data: { expectedRoles: ['Lister'] }
  },
  {
    path: 'post-property',
    component: PostPropertyStepperComponent,
    canActivate: [RoleGuard, ProfileCompletionGuard],
    data: { expectedRoles: ['Lister'] }
  },
   {
    path: 'property/:id/edit',
    component: PropertyEditComponent,
    canActivate: [RoleGuard], 
    data: { expectedRoles: ['Lister','Admin'] } 
  },
  {
    path: 'property/:id/upload-images',
    component: UploadPropertyImagesComponent,
    canActivate: [RoleGuard], 
    data: { expectedRoles: ['Lister', 'Admin'] }
  },
  {
    path: 'lister/dashboard',
    component: ListerDashboardComponent,
    canActivate: [RoleGuard],
    data: { expectedRoles: ['Lister'] }
  },
  {
    path: 'lister/inquiries',
    component: ListerInquiriesComponent,
    canActivate: [RoleGuard],
    data: { expectedRoles: ['Lister','Admin'] }
  },
  { 
    path: 'property/:propertyId/inquiries', 
    component: PropertyInquiriesComponent ,
    data: { expectedRoles: ['Lister','Admin'] }
  },
   {
    path: 'admin/dashboard',
    component: AdminDashboardComponent,
    canActivate: [RoleGuard],
    data: { expectedRoles: ['Admin'] }
  },
  {
    path: 'admin/users',
    component:  UsersComponent,
    canActivate: [RoleGuard],
    data: { expectedRoles: ['Admin'] }
  },
  {
    path: 'admin/properties',
    component:  AdminPropertyListComponent,
    canActivate: [RoleGuard],
    data: { expectedRoles: ['Admin'] }
  },
  {
    path: 'admin/features',
    component:  AdminFeaturesListComponent,
    canActivate: [RoleGuard],
    data: { expectedRoles: ['Admin'] }
  },
  {
    path: 'admin/discounts',
    component: AdminDiscountList,
    canActivate: [RoleGuard],
    data: { expectedRoles: ['Admin'] }
  }
];
