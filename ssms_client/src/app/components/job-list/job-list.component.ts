import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { JobService } from '../../services/job.service';
import { CategoryService } from '../../services/category.service';
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

  jobs = signal<JobResponse[]>([]);
  categories = signal<CategoryResponse[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);

  // filter state
  selectedCategoryId = signal<string>('');
  location = signal<string>('');
  minBudget = signal<number | null>(null);
  maxBudget = signal<number | null>(null);

  JobStatus = JobStatus; // expose enum to template

  ngOnInit(): void {
    this.categoryService.getAll().subscribe({
      next: (cats) => this.categories.set(cats),
      error: () => {} // categories failing shouldn't block job listing
    });

    this.loadOpenJobs();
  }

  loadOpenJobs(): void {
    this.loading.set(true);
    this.error.set(null);

    this.jobService.getOpenJobs().subscribe({
      next: (jobs) => {
        this.jobs.set(jobs);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load jobs. Please try again.');
        this.loading.set(false);
      }
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
      next: (jobs) => {
        this.jobs.set(jobs);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Search failed. Please try again.');
        this.loading.set(false);
      }
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
}