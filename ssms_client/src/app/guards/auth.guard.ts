import { inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = () => {
  const platformId = inject(PLATFORM_ID);
  const auth = inject(AuthService);
  const router = inject(Router);

  // Skip the check during server-side rendering — localStorage doesn't
  // exist there, so this would always look "logged out" regardless of
  // the real state. Let the client re-check once it hydrates in the browser.
  if (!isPlatformBrowser(platformId)) {
    return true;
  }

  if (auth.isAuthenticated()) {
    return true;
  }

  router.navigate(['/login']);
  return false;
};