import { Component, inject } from '@angular/core';
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
  authService = inject(AuthService);

  private navByRole: Record<string, NavItem[]> = {
    Client: [
      { label: 'Dashboard', path: '/dashboard/client', icon: '🏠' },
      { label: 'Post a Job', path: '/jobs/post', icon: '➕' },
      { label: 'Payments', path: '/payments', icon: '💳' },
      { label: 'Marketplace', path: '/marketplace', icon: '🛒' },
      { label: 'My Profile', path: '/profile', icon: '👤' }
    ],
    Worker: [
      { label: 'Dashboard', path: '/dashboard/worker', icon: '🏠' },
      { label: 'My Skills', path: '/profile/skills', icon: '🛠️' },
      { label: 'Marketplace', path: '/marketplace', icon: '🛒' },
      { label: 'My Profile', path: '/profile', icon: '👤' }
    ],
    Supplier: [
      { label: 'Dashboard', path: '/dashboard/supplier', icon: '🏠' },
      { label: 'List Material', path: '/materials/post', icon: '➕' },
      { label: 'Marketplace', path: '/marketplace', icon: '🛒' },
      { label: 'My Profile', path: '/profile', icon: '👤' }
    ],
    Admin: [
      { label: 'Dashboard', path: '/dashboard/admin', icon: '🏠' },
      { label: 'Users', path: '/admin/users', icon: '👥' },
      { label: 'Disputes', path: '/disputes', icon: '⚖️' },
      { label: 'Listing Reports', path: '/admin/listing-reports', icon: '🚩' }
    ]
  };

  get navItems(): NavItem[] {
    const role = this.authService.currentUser()?.role;
    return role ? this.navByRole[role] ?? [] : [];
  }
}