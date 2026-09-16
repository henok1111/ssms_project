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
export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'register', component: Register },

  {
    path: '',
    component: MainLayout,
    children: [
      { path: 'marketplace', component: ListingFeed },
      { path: 'marketplace/:id', component: ListingDetail },

      { path: 'dashboard', canActivate: [dashboardRedirectGuard], children: [] },
      { path: 'dashboard/client', component: ClientDashboard, canActivate: [authGuard, roleGuard(['Client'])] },
      { path: 'dashboard/worker', component: WorkerDashboard, canActivate: [authGuard, roleGuard(['Worker'])] },
      { path: 'dashboard/supplier', component: SupplierDashboard, canActivate: [authGuard, roleGuard(['Supplier'])] },
      { path: 'dashboard/admin', component: AdminDashboard, canActivate: [authGuard, roleGuard(['Admin'])] },
      { path: 'jobs/post', component: PostJob, canActivate: [authGuard, roleGuard(['Client'])] },
      { path: 'jobs/:id', component: JobDetail, canActivate: [authGuard] }
    ]
  },

  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: '**', redirectTo: 'login' }
];