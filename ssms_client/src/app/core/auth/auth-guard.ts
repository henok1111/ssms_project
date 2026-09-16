import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth';
import { catchError, map, of } from 'rxjs';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  
  const router = inject(Router);

  if (authService.currentUser()) return true;

  // On a fresh page load, currentUser() is empty even if a valid cookie
  // exists — check with the backend once before deciding to redirect.
  return authService.getMe().pipe(
    map(() => true),
    catchError(() => {
      router.navigate(['/login']);
      return of(false);
    })
  );
};