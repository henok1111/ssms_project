import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth';

export const dashboardRedirectGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const user = authService.currentUser();
  if (!user) {
    router.navigate(['/login']);
    return false;
  }

  const roleRouteMap: Record<string, string> = {
    Client: '/dashboard/client',
    Worker: '/dashboard/worker',
    Supplier: '/dashboard/supplier',
    Admin: '/dashboard/admin'
  };

  const target = roleRouteMap[user.role] ?? '/login';
  router.navigate([target]);
  return false;
};