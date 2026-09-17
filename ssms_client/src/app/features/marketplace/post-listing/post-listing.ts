import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ListingService } from '../../../core/services/marketplace/listing';
import { ListingCategoryService } from '../../../core/services/marketplace/listing-category';
import { ListingMediaService } from '../../../core/services/marketplace/listing-media';
import { ListingCondition } from '../../../core/models/marketplace/listing.model';
import { ListingCategoryResponse } from '../../../core/models/marketplace/listing.model';
import { LoadingSpinner } from '../../../shared/components/loading-spinner/loading-spinner';

@Component({
  selector: 'app-post-listing',
  standalone: true,
  imports: [ReactiveFormsModule, LoadingSpinner],
  templateUrl: './post-listing.html',
  styleUrl: './post-listing.scss'
})
export class PostListing implements OnInit {
  private fb = inject(FormBuilder);
  private listingService = inject(ListingService);
  private categoryService = inject(ListingCategoryService);
  private mediaService = inject(ListingMediaService);
  private router = inject(Router);

  readonly ListingCondition = ListingCondition;
  categories = signal<ListingCategoryResponse[]>([]);
  selectedFiles: File[] = [];
  errorMessage = '';
  isLoading = false;

  form = this.fb.group({
    categoryId: ['', [Validators.required]],
    title: ['', [Validators.required, Validators.minLength(5)]],
    description: ['', [Validators.required, Validators.minLength(10)]],
    price: [0, [Validators.required, Validators.min(1)]],
    condition: [ListingCondition.Used, [Validators.required]],
    location: ['', [Validators.required]]
  });

  ngOnInit(): void {
    this.categoryService.getAll().subscribe(cats => this.categories.set(cats));
  }

  toNumber(value: unknown): number {
    return Number(value);
  }

  onFilesSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      this.selectedFiles = Array.from(input.files).slice(0, 8); // matches backend's 8-media cap
    }
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.isLoading = true;
    this.errorMessage = '';

    const raw = this.form.getRawValue();
    this.listingService.create({
      categoryId: raw.categoryId!,
      title: raw.title!,
      description: raw.description!,
      price: raw.price!,
      condition: Number(raw.condition) as ListingCondition,
      location: raw.location!
    }).subscribe({
      next: (listing) => this.uploadMediaThenNavigate(listing.id),
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message ?? 'Failed to post listing.';
      }
    });
  }

  private uploadMediaThenNavigate(listingId: string): void {
    if (this.selectedFiles.length === 0) {
      this.isLoading = false;
      this.router.navigate(['/marketplace', listingId]);
      return;
    }

    let completed = 0;
    this.selectedFiles.forEach(file => {
      this.mediaService.upload(listingId, file).subscribe({
        next: () => {
          completed++;
          if (completed === this.selectedFiles.length) {
            this.isLoading = false;
            this.router.navigate(['/marketplace', listingId]);
          }
        },
        error: () => {
          completed++;
          if (completed === this.selectedFiles.length) {
            this.isLoading = false;
            this.router.navigate(['/marketplace', listingId]);
          }
        }
      });
    });
  }
}