import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { JobService } from '../../services/job.service';
import { JobResponse, JobStatus } from '../../models/job.model';
import { JobApplicationResponse, ApplicationStatus } from '../../models/job-application.model';

@Component({
  selector: 'app-my-jobs',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-jobs.component.html',
  styleUrl: './my-jobs.component.scss'
})
export class MyJobsComponent implements OnInit {
  private jobService = inject(JobService);

  jobs = signal<JobResponse[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);

  expandedJobId = signal<string | null>(null);
  applications = signal<JobApplicationResponse[]>([]);
  applicationsLoading = signal(false);
  acceptingId = signal<string | null>(null);

  JobStatus = JobStatus;
  ApplicationStatus = ApplicationStatus;

  ngOnInit(): void {
    this.loading.set(true);
    this.jobService.getMine().subscribe({
      next: (jobs) => { this.jobs.set(jobs); this.loading.set(false); },
      error: () => { this.error.set('Failed to load your jobs.'); this.loading.set(false); }
    });
  }

  toggleApplications(jobId: string): void {
    if (this.expandedJobId() === jobId) {
      this.expandedJobId.set(null);
      return;
    }

    this.expandedJobId.set(jobId);
    this.applicationsLoading.set(true);
    this.applications.set([]);

    this.jobService.getApplications(jobId).subscribe({
      next: (apps) => { this.applications.set(apps); this.applicationsLoading.set(false); },
      error: () => { this.applicationsLoading.set(false); }
    });
  }

  accept(jobId: string, applicationId: string): void {
    this.acceptingId.set(applicationId);

    this.jobService.acceptApplication(jobId, applicationId).subscribe({
      next: () => {
        this.acceptingId.set(null);
        // Refresh both the job list (status changes to Assigned) and the applications (statuses update)
        this.jobService.getMine().subscribe(jobs => this.jobs.set(jobs));
        this.jobService.getApplications(jobId).subscribe(apps => this.applications.set(apps));
      },
      error: () => { this.acceptingId.set(null); }
    });
  }
}