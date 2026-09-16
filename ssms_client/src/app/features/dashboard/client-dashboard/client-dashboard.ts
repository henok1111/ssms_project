import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { JobService } from '../../../core/services/job';
import { AuthService } from '../../../core/auth/auth';
import { JobResponse, JobStatus } from '../../../core/models/job.model';
import { Card } from '../../../shared/components/card/card';
import { LoadingSpinner } from '../../../shared/components/loading-spinner/loading-spinner';

@Component({
  selector: 'app-client-dashboard',
  standalone: true,
  imports: [RouterLink, Card, LoadingSpinner],
  templateUrl: './client-dashboard.html',
  styleUrl: './client-dashboard.scss'
})
export class ClientDashboard implements OnInit {
  private jobService = inject(JobService);
  authService = inject(AuthService);

  readonly JobStatus = JobStatus;
  jobs = signal<JobResponse[]>([]);
  isLoading = signal(true);

  ngOnInit(): void {
    this.jobService.getMine().subscribe({
      next: (jobs) => {
        this.jobs.set(jobs);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  get activeJobsCount(): number {
    return this.jobs().filter(j => j.status !== JobStatus.Closed && j.status !== JobStatus.Cancelled).length;
  }

  get pendingQuotesCount(): number {
    return this.jobs().filter(j => j.status === JobStatus.InProgress).length;
  }
}