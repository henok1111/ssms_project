import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { JobService } from '../../services/job.service';
import { CategoryService } from '../../services/category.service';
import { AuthService } from '../../services/auth.service';
import { JobResponse, JobStatus } from '../../models/job.model';
import { CategoryResponse } from '../../models/category.model';

@Component({
  selector: 'app-job-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './job-list.component.html',
  styleUrl: './job-list.component.scss'
})
export class JobListComponent implements OnInit {
  private jobService = inject(JobService);
  private categoryService = inject(CategoryService);
  auth = inject(AuthService); // public — template needs to read auth.currentUser()

  jobs = signal<JobResponse[]>([]);
  categories = signal<CategoryResponse[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);

  selectedCategoryId = signal<string>('');
  location = signal<string>('');
  minBudget = signal<number | null>(null);
  maxBudget = signal<number | null>(null);

  // apply-form state — tracks which job's inline form is open
  applyingJobId = signal<string | null>(null);
  proposedPrice = signal<number | null>(null);
  message = signal<string>('');
  applySubmitting = signal(false);
  applyError = signal<string | null>(null);
  appliedJobIds = signal<Set<string>>(new Set());

  JobStatus = JobStatus;

  ngOnInit(): void {
    this.categoryService.getAll().subscribe({ next: (cats) => this.categories.set(cats), error: () => {} });
    this.loadOpenJobs();
  }

  get isWorker(): boolean {
    return this.auth.currentUser()?.role === 'Worker';
  }

  loadOpenJobs(): void {
    this.loading.set(true);
    this.error.set(null);
    this.jobService.getOpenJobs().subscribe({
      next: (jobs) => { this.jobs.set(jobs); this.loading.set(false); },
      error: () => { this.error.set('Failed to load jobs. Please try again.'); this.loading.set(false); }
    });
  }

  onSearch(): void {
    this.loading.set(true);
    this.error.set(null);
    this.jobService.search({
      categoryId: this.selectedCategoryId() || undefined,
      location: this.location() || undefined,
      minBudget: this.minBudget() ?? undefined,
      maxBudget: this.maxBudget() ?? undefined
    }).subscribe({
      next: (jobs) => { this.jobs.set(jobs); this.loading.set(false); },
      error: () => { this.error.set('Search failed. Please try again.'); this.loading.set(false); }
    });
  }

  clearFilters(): void {
    this.selectedCategoryId.set('');
    this.location.set('');
    this.minBudget.set(null);
    this.maxBudget.set(null);
    this.loadOpenJobs();
  }

  categoryName(categoryId: string): string {
    return this.categories().find(c => c.id === categoryId)?.name ?? 'Uncategorized';
  }

  openApplyForm(jobId: string): void {
    this.applyingJobId.set(jobId);
    this.proposedPrice.set(null);
    this.message.set('');
    this.applyError.set(null);
  }

  cancelApplyForm(): void {
    this.applyingJobId.set(null);
  }

  submitApplication(jobId: string): void {
    this.applyError.set(null);

    if (!this.proposedPrice() || this.proposedPrice()! <= 0) {
      this.applyError.set('Enter a proposed price.');
      return;
    }

    this.applySubmitting.set(true);

    this.jobService.apply(jobId, {
      proposedPrice: this.proposedPrice()!,
      message: this.message().trim() || null
    }).subscribe({
      next: () => {
        this.applySubmitting.set(false);
        this.applyingJobId.set(null);
        this.appliedJobIds.update(set => new Set(set).add(jobId));
      },
      error: (err) => {
        this.applySubmitting.set(false);
        this.applyError.set(err?.error?.message ?? 'Could not submit application. Try again.');
      }
    });
  }

  hasApplied(jobId: string): boolean {
    return this.appliedJobIds().has(jobId);
  }
}