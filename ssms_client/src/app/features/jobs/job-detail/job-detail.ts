import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { JobService } from '../../../core/services/job';
import { AuthService } from '../../../core/auth/auth';
import { JobResponse, JobStatus, JobApplicationResponse, ApplicationStatus } from '../../../core/models/job.model';
import { Card } from '../../../shared/components/card/card';
import { LoadingSpinner } from '../../../shared/components/loading-spinner/loading-spinner';

@Component({
  selector: 'app-job-detail',
  standalone: true,
  imports: [ReactiveFormsModule, Card, LoadingSpinner],
  templateUrl: './job-detail.html',
  styleUrl: './job-detail.scss'
})
export class JobDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private jobService = inject(JobService);
  private fb = inject(FormBuilder);
  authService = inject(AuthService);

  readonly JobStatus = JobStatus;
  readonly ApplicationStatus = ApplicationStatus;

  job = signal<JobResponse | null>(null);
  applications = signal<JobApplicationResponse[]>([]);
  isLoading = signal(true);
  actionError = signal('');
  actionLoading = signal(false);

  applyForm = this.fb.group({
    proposedPrice: [0, [Validators.required, Validators.min(1)]],
    message: ['']
  });

  private get jobId(): string {
    return this.route.snapshot.paramMap.get('id')!;
  }

  get currentUserId(): string | undefined {
    return this.authService.currentUser()?.userId;
  }

  get isOwner(): boolean {
    return this.job()?.clientId === this.currentUserId;
  }

  get isAssignedWorker(): boolean {
    return this.job()?.assignedWorkerId === this.currentUserId;
  }

  get userRole(): string | undefined {
    return this.authService.currentUser()?.role;
  }

  ngOnInit(): void {
    this.loadJob();
  }

  loadJob(): void {
    this.isLoading.set(true);
    this.jobService.getById(this.jobId).subscribe({
      next: (job) => {
        this.job.set(job);
        this.isLoading.set(false);
        if (this.isOwner) this.loadApplications();
      },
      error: () => this.isLoading.set(false)
    });
  }

  loadApplications(): void {
    this.jobService.getApplications(this.jobId).subscribe(apps => this.applications.set(apps));
  }

  onApply(): void {
    if (this.applyForm.invalid) return;
    this.actionLoading.set(true);
    this.actionError.set('');

    const raw = this.applyForm.getRawValue();
    this.jobService.apply(this.jobId, {
      proposedPrice: raw.proposedPrice!,
      message: raw.message || null
    }).subscribe({
      next: () => {
        this.actionLoading.set(false);
        this.applyForm.reset();
        alert('Application submitted!');
      },
      error: (err) => {
        this.actionLoading.set(false);
        this.actionError.set(err.error?.message ?? 'Failed to apply.');
      }
    });
  }

  onAcceptApplication(applicationId: string): void {
    this.actionLoading.set(true);
    this.jobService.acceptApplication(this.jobId, applicationId).subscribe({
      next: () => {
        this.actionLoading.set(false);
        this.loadJob();
      },
      error: (err) => {
        this.actionLoading.set(false);
        this.actionError.set(err.error?.message ?? 'Failed to accept application.');
      }
    });
  }

  onStartJob(): void {
    this.actionLoading.set(true);
    this.jobService.start(this.jobId).subscribe({
      next: () => { this.actionLoading.set(false); this.loadJob(); },
      error: (err) => { this.actionLoading.set(false); this.actionError.set(err.error?.message ?? 'Failed to start job.'); }
    });
  }

  onCompleteJob(): void {
    this.actionLoading.set(true);
    this.jobService.complete(this.jobId).subscribe({
      next: () => { this.actionLoading.set(false); this.loadJob(); },
      error: (err) => { this.actionLoading.set(false); this.actionError.set(err.error?.message ?? 'Failed to complete job.'); }
    });
  }
}