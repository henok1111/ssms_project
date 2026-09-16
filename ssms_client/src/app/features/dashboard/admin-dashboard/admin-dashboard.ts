import { Component, inject, OnInit, signal } from '@angular/core';
import { AdminService } from '../../../core/services/admin';
import { AuthService } from '../../../core/auth/auth';
import { PendingApprovalResponse } from '../../../core/models/admin.model';
import { Card } from '../../../shared/components/card/card';
import { LoadingSpinner } from '../../../shared/components/loading-spinner/loading-spinner';
import { SignupsChart } from './charts/signups-chart/signups-chart';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [Card, LoadingSpinner, SignupsChart],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.scss'
})
export class AdminDashboard implements OnInit {
  private adminService = inject(AdminService);
  authService = inject(AuthService);

  pendingWorkers = signal<PendingApprovalResponse[]>([]);
  pendingSuppliers = signal<PendingApprovalResponse[]>([]);
  isLoading = signal(true);
  actionLoading = signal<string | null>(null);

  // Placeholder chart data until we build a real analytics endpoint —
  // flagged clearly so we remember this isn't real yet.
  chartLabels = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
  chartData = [2, 4, 3, 6, 5, 8, 7];

  ngOnInit(): void {
    this.adminService.getPendingWorkers().subscribe(w => this.pendingWorkers.set(w));
    this.adminService.getPendingSuppliers().subscribe({
      next: (s) => { this.pendingSuppliers.set(s); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  onApproveWorker(id: string): void {
    this.actionLoading.set(id);
    this.adminService.approveWorker(id).subscribe(() => this.refresh());
  }

  onRejectWorker(id: string): void {
    this.actionLoading.set(id);
    this.adminService.rejectWorker(id).subscribe(() => this.refresh());
  }

  onApproveSupplier(id: string): void {
    this.actionLoading.set(id);
    this.adminService.approveSupplier(id).subscribe(() => this.refresh());
  }

  onRejectSupplier(id: string): void {
    this.actionLoading.set(id);
    this.adminService.rejectSupplier(id).subscribe(() => this.refresh());
  }

  private refresh(): void {
    this.adminService.getPendingWorkers().subscribe(w => this.pendingWorkers.set(w));
    this.adminService.getPendingSuppliers().subscribe(s => { this.pendingSuppliers.set(s); this.actionLoading.set(null); });
  }
}