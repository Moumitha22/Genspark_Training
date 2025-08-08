import { Routes } from '@angular/router';
import { SellerDashboard } from './feature/seller/dashboard/seller-dashboard';
import { AuthGuard } from './core/guard/auth-guard';
import { LoginGuard } from './core/guard/login-guard';
import { Login } from './feature/auth/login/login';
import { AuctionForm } from './feature/seller/auction-form/auction-form';
import { ViewAuction } from './shared/components/view-auction/view-auction';
import { ViewAuctionAttachements } from './feature/seller/view-auction-attachements/view-auction-attachements';
import { UpdateAuctionWinner } from './feature/seller/update-auction-winner/update-auction-winner';
import { LayoutComponent } from './layout/layout-component/layout-component';
import { BidderDashboard } from './feature/bidder/bidder-dashboard/bidder-dashboard';
import { VirtualWallet } from './feature/bidder/virtual-wallet/virtual-wallet';
import { FindAuctions } from './feature/bidder/find-auctions/find-auctions';
import { ViewBids } from './feature/bidder/view-bids/view-bids';
import { EAggrements } from './feature/bidder/e-aggrements/e-aggrements';
import { Unauthorized } from './feature/auth/unauthorized/unauthorized';
import { Register } from './feature/auth/register/register';
import { ForgetPassword } from './feature/auth/forget-password/forget-password';
import { AdminDashboard } from './feature/admin/admin-dashboard/admin-dashboard';
import { ChangePassword } from './feature/auth/change-password/change-password';
import { ManageUsers } from './feature/admin/manage-users/manage-users';
import { LoginSuperAdmin } from './feature/login-super-admin/login-super-admin';
import { ManageBids } from './feature/admin/manage-bids/manage-bids';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: Login, canActivate: [LoginGuard] },
  {
    path: 'register',
    component: Register,
    // canActivate: [AuthGuard],
    // data: {
    //   roles: ['Admin'],
    // },
  },
  { path: 'login-super-admin', component: LoginSuperAdmin, canActivate: [LoginGuard] },
  {
    path: 'change-password',
    component: ChangePassword,
    canActivate: [AuthGuard],
  },
  {
    path: 'forget-password',
    component: ForgetPassword,
    canActivate: [LoginGuard],
  },
  { path: 'unauthorized', component: Unauthorized },
  {
    path: 'seller',
    component: LayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: SellerDashboard },
      { path: 'create-auction', component: AuctionForm },
      { path: 'view-auctions', component: ViewAuction },
      {
        path: 'view-auction-attachements/:auctionId',
        component: ViewAuctionAttachements,
      },
      { path: 'update-auction-winner', component: UpdateAuctionWinner },
    ],
    data: { roles: ['Seller'] },
  },

  {
    path: 'bidder',
    component: LayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: BidderDashboard },
      { path: 'find-auctions', component: FindAuctions },
      { path: 'my-bids', component: ViewBids },
      { path: 'vitual-wallet', component: VirtualWallet },
      { path: 'e-agreements', component: EAggrements },
    ],
    data: { roles: ['Bidder'] },
  },

  {
    path: 'admin',
    component: LayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: AdminDashboard },
      { path: 'manage-users', component: ManageUsers },
      { path: 'manage-auctions', component: ViewAuction },
      { path: 'manage-bids', component: ManageBids },
      { path: 'view-auction-attachements/:auctionId', component: ViewAuctionAttachements },
    ],
    data: { roles: ['Admin'] },
  },
];
