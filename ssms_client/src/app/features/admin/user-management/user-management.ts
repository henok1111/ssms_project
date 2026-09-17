import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../../core/services/admin';
import { UserSummaryResponse } from '../../../core/models/admin.model';
import { Card } from '../../../shared/components/card/card';
import { LoadingSpinner } from '../../../shared/components/loading-spinner/loading-spinner';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [FormsModule, Card, LoadingSpinner],
  templateUrl: './user-management.html',
  styleUrl: './user-management.scss'
})
export class UserManagement implements OnInit {
  private adminService = inject(AdminService);

  users = signal<UserSummaryResponse[]>([]);
  isLoading = signal(true);
  actionLoading = signal<string | null>(null);
  selectedRole: number | undefined;

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.isLoading.set(true);
    this.adminService.getUsers(this.selectedRole).subscribe({
      next: (u) => { this.users.set(u); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  onFilterChange(): void {
    this.loadUsers();
  }

  onToggleActive(user: UserSummaryResponse): void {
    this.actionLoading.set(user.id);
    const action = user.isActive
      ? this.adminService.deactivateUser(user.id)
      : this.adminService.reactivateUser(user.id);

    action.subscribe({
      next: () => { this.actionLoading.set(null); this.loadUsers(); },
      error: () => this.actionLoading.set(null)
    });
  }
}