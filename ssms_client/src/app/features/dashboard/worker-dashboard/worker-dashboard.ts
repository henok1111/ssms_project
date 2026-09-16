import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { JobService } from '../../../core/services/job';
import { AuthService } from '../../../core/auth/auth';
import { JobResponse, JobStatus } from '../../../core/models/job.model';
import { Card } from '../../../shared/components/card/card';
import { LoadingSpinner } from '../../../shared/components/loading-spinner/loading-spinner';

@Component({
  selector: 'app-worker-dashboard',
  standalone: true,
  imports: [RouterLink, Card, LoadingSpinner],
  templateUrl: './worker-dashboard.html',
  styleUrl: './worker-dashboard.scss'
})
export class WorkerDashboard implements OnInit {
  private jobService = inject(JobService);
  authService = inject(AuthService);

  readonly JobStatus = JobStatus;
  openJobs = signal<JobResponse[]>([]);
  assignedJobs = signal<JobResponse[]>([]);
  isLoading = signal(true);

  ngOnInit(): void {
    this.jobService.getOpen().subscribe(jobs => this.openJobs.set(jobs));
    this.jobService.getAssignedToMe().subscribe({
      next: (jobs) => {
        this.assignedJobs.set(jobs);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  get activeAssignedCount(): number {
    return this.assignedJobs().filter(j => j.status === JobStatus.Assigned || j.status === JobStatus.InProgress).length;
  }
}