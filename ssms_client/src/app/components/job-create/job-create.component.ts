import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { JobService } from '../../services/job.service';
import { CategoryService } from '../../services/category.service';
import { CategoryResponse } from '../../models/category.model';
import { JobType, CreateJobRequest } from '../../models/job.model';

@Component({
  selector: 'app-job-create',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './job-create.component.html',
  styleUrl: './job-create.component.scss'
})
export class JobCreateComponent implements OnInit {
  private jobService = inject(JobService);
  private categoryService = inject(CategoryService);
  private router = inject(Router);

  categories = signal<CategoryResponse[]>([]);
  submitting = signal(false);
  error = signal<string | null>(null);

  JobType = JobType; // expose enum to template

  // form fields
  categoryId = signal('');
  title = signal('');
  description = signal('');
  jobType = signal<JobType>(JobType.OnSite);
  location = signal('');
  budget = signal<number | null>(null);

  ngOnInit(): void {
    this.categoryService.getAll().subscribe({
      next: (cats) => this.categories.set(cats),
      error: () => this.error.set('Could not load categories. Try reloading the page.')
    });
  }

  get isRemote(): boolean {
    return this.jobType() === JobType.Remote;
  }

  onSubmit(): void {
    this.error.set(null);

    if (!this.categoryId() || !this.title().trim() || !this.description().trim() || !this.budget()) {
      this.error.set('Fill in category, title, description, and budget before posting.');
      return;
    }

    const request: CreateJobRequest = {
      categoryId: this.categoryId(),
      title: this.title().trim(),
      description: this.description().trim(),
      jobType: this.jobType(),
      location: this.isRemote ? null : (this.location().trim() || null),
      budget: this.budget()!
    };

    this.submitting.set(true);

    this.jobService.create(request).subscribe({
      next: (job) => {
        this.submitting.set(false);
        this.router.navigate(['/jobs', job.id]);
      },
      error: (err) => {
        this.submitting.set(false);
        this.error.set(
          err?.status === 401
            ? 'You need to be logged in as a client to post a job.'
            : 'Could not post the job. Check your details and try again.'
        );
      }
    });
  }
}