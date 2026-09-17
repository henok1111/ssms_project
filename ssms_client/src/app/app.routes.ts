import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { authGuard } from './core/auth/auth-guard';
import { roleGuard } from './core/auth/role-guard';
import { dashboardRedirectGuard } from './core/auth/dashboard-redirect-guard';
import { MainLayout } from './layout/main-layout/main-layout';
import { ClientDashboard } from './features/dashboard/client-dashboard/client-dashboard';
import { WorkerDashboard } from './features/dashboard/worker-dashboard/worker-dashboard';
import { SupplierDashboard } from './features/dashboard/supplier-dashboard/supplier-dashboard';
import { AdminDashboard } from './features/dashboard/admin-dashboard/admin-dashboard';
import { PostJob } from './features/jobs/post-job/post-job';
import { JobDetail } from './features/jobs/job-detail/job-detail';
import { ListingFeed } from './features/marketplace/listing-feed/listing-feed';
import { ListingDetail } from './features/marketplace/listing-detail/listing-detail';
import { PostListing } from './features/marketplace/post-listing/post-listing';
import { MyProfile } from './features/profile/my-profile/my-profile';
import { ListingChat } from './features/marketplace/listing-chat/listing-chat';
import { DisputeList } from './features/disputes/dispute-list/dispute-list';
import { PaymentHistory } from './features/payments/payment-history/payment-history';
import { ListingReports } from './features/admin/listing-reports/listing-reports';
import { UserManagement } from './features/admin/user-management/user-management';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'register', component: Register },

  {
    path: '',
    component: MainLayout,
    children: [
      { path: 'profile', component: MyProfile, canActivate: [authGuard] },
      { path: 'disputes', component: DisputeList, canActivate: [authGuard, roleGuard(['Admin'])] },
{ path: 'payments', component: PaymentHistory, canActivate: [authGuard, roleGuard(['Client'])] },

      { path: 'marketplace', component: ListingFeed },
      { path: 'marketplace/:id', component: ListingDetail },
       { path: 'marketplace/chat', component: ListingChat, canActivate: [authGuard] },
{ path: 'marketplace/post', component: PostListing, canActivate: [authGuard] },
      { path: 'dashboard', canActivate: [dashboardRedirectGuard], children: [] },
      { path: 'dashboard/client', component: ClientDashboard, canActivate: [authGuard, roleGuard(['Client'])] },
      { path: 'dashboard/worker', component: WorkerDashboard, canActivate: [authGuard, roleGuard(['Worker'])] },
      { path: 'dashboard/supplier', component: SupplierDashboard, canActivate: [authGuard, roleGuard(['Supplier'])] },
      { path: 'dashboard/admin', component: AdminDashboard, canActivate: [authGuard, roleGuard(['Admin'])] },
      { path: 'jobs/post', component: PostJob, canActivate: [authGuard, roleGuard(['Client'])] },
      { path: 'jobs/:id', component: JobDetail, canActivate: [authGuard] },{ path: 'admin/listing-reports', component: ListingReports, canActivate: [authGuard, roleGuard(['Admin'])] },
{ path: 'admin/users', component: UserManagement, canActivate: [authGuard, roleGuard(['Admin'])] },
    ]
  },

  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: '**', redirectTo: 'login' }
];