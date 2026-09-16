import { Component, inject, signal, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { JobService } from '../../../core/services/job';
import { CategoryService } from '../../../core/services/category';
import { JobType } from '../../../core/models/job.model';
import { CategoryResponse } from '../../../core/models/category.model';
import { LoadingSpinner } from '../../../shared/components/loading-spinner/loading-spinner';

@Component({
  selector: 'app-post-job',
  standalone: true,
  imports: [ReactiveFormsModule, LoadingSpinner],
  templateUrl: './post-job.html',
  styleUrl: './post-job.scss'
})
export class PostJob implements OnInit {
  private fb = inject(FormBuilder);
  private jobService = inject(JobService);
  private categoryService = inject(CategoryService);
  private router = inject(Router);

  readonly JobType = JobType;
  categories = signal<CategoryResponse[]>([]);
  errorMessage = '';
  isLoading = false;

  form = this.fb.group({
    categoryId: ['', [Validators.required]],
    title: ['', [Validators.required, Validators.minLength(5)]],
    description: ['', [Validators.required, Validators.minLength(20)]],
    jobType: [JobType.OnSite, [Validators.required]],
    location: [''],
    budget: [0, [Validators.required, Validators.min(1)]]
  });

  ngOnInit(): void {
    this.categoryService.getAll(true).subscribe(categories => this.categories.set(categories));
  }
toNumber(value: unknown): number {
  return Number(value);
}
  onSubmit(): void {
    if (this.form.invalid) return;

    this.isLoading = true;
    this.errorMessage = '';

    const raw = this.form.getRawValue();
    this.jobService.create({
      categoryId: raw.categoryId!,
      title: raw.title!,
      description: raw.description!,
      jobType: Number(raw.jobType) as JobType,
      location: raw.location || null,
      budget: raw.budget!
    }).subscribe({
      next: (job) => {
        this.isLoading = false;
        this.router.navigate(['/dashboard/client']);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message ?? 'Failed to post job. Please try again.';
      }
    });
  }
}