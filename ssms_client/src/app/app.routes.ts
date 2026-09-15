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

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'register', component: Register },

  { path: 'dashboard', canActivate: [dashboardRedirectGuard], children: [] },

  {
    path: '',
    component: MainLayout,
    canActivate: [authGuard],
    children: [
      { path: 'dashboard/client', component: ClientDashboard, canActivate: [roleGuard(['Client'])] },
      { path: 'dashboard/worker', component: WorkerDashboard, canActivate: [roleGuard(['Worker'])] },
      { path: 'dashboard/supplier', component: SupplierDashboard, canActivate: [roleGuard(['Supplier'])] },
      { path: 'dashboard/admin', component: AdminDashboard, canActivate: [roleGuard(['Admin'])] }
    ]
  },

  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: '**', redirectTo: 'login' }
];