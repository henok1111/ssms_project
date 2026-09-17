import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth/auth';
import { ThemeToggleComponent } from '../../shared/components/theme-toggle/theme-toggle';
import { NotificationBell } from '../../shared/components/notification-bell/notification-bell';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [ThemeToggleComponent,NotificationBell],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss'
})
export class Navbar {
  authService = inject(AuthService);
  private router = inject(Router);

  onLogout(): void {
    this.authService.logout().subscribe(() => {
      this.router.navigate(['/login']);
    });
  }
}