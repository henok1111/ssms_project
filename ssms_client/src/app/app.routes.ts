import { Routes } from '@angular/router';

import { Home } from './features/home/home';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';

import { JobList } from './features/jobs/job-list/job-list';
import { JobDetail } from './features/jobs/job-detail/job-detail';
import { PostJob } from './features/jobs/post-job/post-job';

import { ListingFeed } from './features/marketplace/listing-feed/listing-feed';
import { ListingDetail } from './features/marketplace/listing-detail/listing-detail';
import { PostListing } from './features/marketplace/post-listing/post-listing';
import { ListingChat } from './features/marketplace/listing-chat/listing-chat';

import { MyProfile } from './features/profile/my-profile/my-profile';

import { ClientDashboard } from './features/dashboard/client-dashboard/client-dashboard';
import { WorkerDashboard } from './features/dashboard/worker-dashboard/worker-dashboard';
import { SupplierDashboard } from './features/dashboard/supplier-dashboard/supplier-dashboard';
import { AdminDashboard } from './features/dashboard/admin-dashboard/admin-dashboard';

import { DisputeList } from './features/disputes/dispute-list/dispute-list';
import { PaymentHistory } from './features/payments/payment-history/payment-history';

import { ListingReports } from './features/admin/listing-reports/listing-reports';
import { UserManagement } from './features/admin/user-management/user-management';

import { authGuard } from './core/auth/auth-guard';
import { roleGuard } from './core/auth/role-guard';
import { dashboardRedirectGuard } from './core/auth/dashboard-redirect-guard';

import { MainLayout } from './layout/main-layout/main-layout';
import { PublicLayout } from './layout/public-layout/public-layout';

export const routes: Routes = [

  {
    path: 'marketplace/post',
    component: MainLayout,
    children: [
      {
        path: '',
        component: PostListing,
        canActivate: [authGuard]
      }
    ]
  },

  {
    path: 'marketplace/chat',
    component: MainLayout,
    children: [
      {
        path: '',
        component: ListingChat,
        canActivate: [authGuard]
      }
    ]
  },

  {
    path: 'jobs/post',
    component: MainLayout,
    children: [
      {
        path: '',
        component: PostJob,
        canActivate: [
          authGuard,
          roleGuard(['Client'])
        ]
      }
    ]
  },

  {
    path: 'profile',
    component: MainLayout,
    children: [
      {
        path: '',
        component: MyProfile,
        canActivate: [authGuard]
      }
    ]
  },

  {
    path: 'dashboard',
    component: MainLayout,
    children: [
      {
        path: '',
        canActivate: [dashboardRedirectGuard],
        children: []
      }
    ]
  },

  {
    path: 'dashboard/client',
    component: MainLayout,
    children: [
      {
        path: '',
        component: ClientDashboard,
        canActivate: [
          authGuard,
          roleGuard(['Client'])
        ]
      }
    ]
  },

  {
    path: 'dashboard/worker',
    component: MainLayout,
    children: [
      {
        path: '',
        component: WorkerDashboard,
        canActivate: [
          authGuard,
          roleGuard(['Worker'])
        ]
      }
    ]
  },

  {
    path: 'dashboard/supplier',
    component: MainLayout,
    children: [
      {
        path: '',
        component: SupplierDashboard,
        canActivate: [
          authGuard,
          roleGuard(['Supplier'])
        ]
      }
    ]
  },

  {
    path: 'dashboard/admin',
    component: MainLayout,
    children: [
      {
        path: '',
        component: AdminDashboard,
        canActivate: [
          authGuard,
          roleGuard(['Admin'])
        ]
      }
    ]
  },

  {
    path: 'disputes',
    component: MainLayout,
    children: [
      {
        path: '',
        component: DisputeList,
        canActivate: [
          authGuard,
          roleGuard(['Admin'])
        ]
      }
    ]
  },

  {
    path: 'payments',
    component: MainLayout,
    children: [
      {
        path: '',
        component: PaymentHistory,
        canActivate: [
          authGuard,
          roleGuard(['Client'])
        ]
      }
    ]
  },

  {
    path: 'admin/listing-reports',
    component: MainLayout,
    children: [
      {
        path: '',
        component: ListingReports,
        canActivate: [
          authGuard,
          roleGuard(['Admin'])
        ]
      }
    ]
  },

  {
    path: 'admin/users',
    component: MainLayout,
    children: [
      {
        path: '',
        component: UserManagement,
        canActivate: [
          authGuard,
          roleGuard(['Admin'])
        ]
      }
    ]
  },

  {
    path: '',
    component: PublicLayout,
    children: [
      {
        path: '',
        component: Home
      },
      {
        path: 'login',
        component: Login
      },
      {
        path: 'register',
        component: Register
      },
      {
        path: 'jobs',
        component: JobList
      },
      {
        path: 'jobs/:id',
        component: JobDetail
      },
      {
        path: 'marketplace',
        component: ListingFeed
      },
      {
        path: 'marketplace/:id',
        component: ListingDetail
      }
    ]
  },

  {
    path: '**',
    redirectTo: ''
  }

];