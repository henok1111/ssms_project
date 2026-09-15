import { Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/auth/auth';

interface NavItem {
  label: string;
  path: string;
  icon: string;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss'
})
export class Sidebar {
  private authService = inject(AuthService);

  private readonly navByRole: Record<string, NavItem[]> = {
    Client: [
      { label: 'Dashboard', path: '/dashboard/client', icon: '🏠' },
      { label: 'My Jobs', path: '/jobs/mine', icon: '📋' },
      { label: 'Post a Job', path: '/jobs/post', icon: '➕' },
      { label: 'Payments', path: '/payments', icon: '💳' },
      { label: 'Marketplace', path: '/marketplace', icon: '🛒' },
      { label: 'My Listings', path: '/marketplace/mine', icon: '🏷️' },
      { label: 'Profile', path: '/profile', icon: '👤' }
    ],
    Worker: [
      { label: 'Dashboard', path: '/dashboard/worker', icon: '🏠' },
      { label: 'Open Jobs', path: '/jobs', icon: '🔍' },
      { label: 'My Applications', path: '/jobs/applications', icon: '📄' },
      { label: 'My Skills', path: '/profile/skills', icon: '🛠️' },
      { label: 'Marketplace', path: '/marketplace', icon: '🛒' },
      { label: 'My Listings', path: '/marketplace/mine', icon: '🏷️' },
      { label: 'Profile', path: '/profile', icon: '👤' }
    ],
    Supplier: [
      { label: 'Dashboard', path: '/dashboard/supplier', icon: '🏠' },
      { label: 'My Catalog', path: '/materials/mine', icon: '📦' },
      { label: 'Orders', path: '/materials/orders', icon: '🚚' },
      { label: 'Marketplace', path: '/marketplace', icon: '🛒' },
      { label: 'My Listings', path: '/marketplace/mine', icon: '🏷️' },
      { label: 'Profile', path: '/profile', icon: '👤' }
    ],
    Admin: [
      { label: 'Dashboard', path: '/dashboard/admin', icon: '🏠' },
      { label: 'Pending Approvals', path: '/admin/approvals', icon: '✅' },
      { label: 'Users', path: '/admin/users', icon: '👥' },
      { label: 'Disputes', path: '/disputes', icon: '⚖️' },
      { label: 'Listing Reports', path: '/admin/listing-reports', icon: '🚩' },
      { label: 'Categories', path: '/admin/categories', icon: '🗂️' }
    ]
  };

  readonly navItems = computed<NavItem[]>(() => {
    const role = this.authService.currentUser()?.role;
    return role ? (this.navByRole[role] ?? []) : [];
  });
}