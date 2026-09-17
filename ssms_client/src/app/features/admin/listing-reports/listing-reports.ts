import { Component, inject, OnInit, signal } from '@angular/core';
import { ListingReportService } from '../../../core/services/marketplace/listing-report';
import { ListingReportResponse } from '../../../core/models/marketplace/listing-report.model';
import { Card } from '../../../shared/components/card/card';
import { LoadingSpinner } from '../../../shared/components/loading-spinner/loading-spinner';

@Component({
  selector: 'app-listing-reports',
  standalone: true,
  imports: [Card, LoadingSpinner],
  templateUrl: './listing-reports.html',
  styleUrl: './listing-reports.scss'
})
export class ListingReports implements OnInit {
  private reportService = inject(ListingReportService);

  reports = signal<ListingReportResponse[]>([]);
  isLoading = signal(true);
  actionLoading = signal<string | null>(null);

  ngOnInit(): void {
    this.loadReports();
  }

  loadReports(): void {
    this.reportService.getPending().subscribe({
      next: (r) => { this.reports.set(r); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  onResolve(reportId: string, removeListing: boolean): void {
    this.actionLoading.set(reportId);
    this.reportService.resolve(reportId, { removeListing }).subscribe({
      next: () => { this.actionLoading.set(null); this.loadReports(); },
      error: () => this.actionLoading.set(null)
    });
  }
}