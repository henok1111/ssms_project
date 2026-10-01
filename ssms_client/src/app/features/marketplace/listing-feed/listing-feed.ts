
import { Component, inject, OnInit, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ListingService } from '../../../core/services/marketplace/listing';
import { ListingCategoryService } from '../../../core/services/marketplace/listing-category';

import {
  ListingCondition,
  ListingResponse,
  ListingCategoryResponse
} from '../../../core/models/marketplace/listing.model';

import { Card } from '../../../shared/components/card/card';
import { LoadingSpinner } from '../../../shared/components/loading-spinner/loading-spinner';

@Component({
  selector: 'app-listing-feed',
  standalone: true,
  imports: [
    RouterLink,
    FormsModule,
    DecimalPipe,
    Card,
    LoadingSpinner
  ],
  templateUrl: './listing-feed.html',
  styleUrl: './listing-feed.scss'
})
export class ListingFeed implements OnInit {

  private listingService = inject(ListingService);
  private categoryService = inject(ListingCategoryService);

  listings = signal<ListingResponse[]>([]);
  categories = signal<ListingCategoryResponse[]>([]);

  isLoading = signal(true);
  hasError = signal(false);

  keyword = '';
  selectedCategoryId = '';
  maxPrice: number | null = null;
  location = '';
  sortBy = '';

  ngOnInit(): void {
    this.loadCategories();
    this.loadListings();
  }

  private loadCategories(): void {
    this.categoryService.getAll().subscribe({
      next: (categories) => {
        this.categories.set(categories);
      },
      error: () => {
        this.categories.set([]);
      }
    });
  }

  loadListings(): void {
    this.isLoading.set(true);
    this.hasError.set(false);

    const hasFilters =
      this.keyword.trim() !== '' ||
      this.selectedCategoryId !== '' ||
      this.maxPrice !== null ||
      this.location.trim() !== '' ||
      this.sortBy !== '';

    if (!hasFilters) {
      this.loadActiveListings();
      return;
    }

    this.searchListings();
  }

  private loadActiveListings(): void {
    this.listingService.getActive().subscribe({
      next: (listings) => {
        this.listings.set(listings);
        this.isLoading.set(false);
      },
      error: () => {
        this.listings.set([]);
        this.hasError.set(true);
        this.isLoading.set(false);
      }
    });
  }

  private searchListings(): void {
    this.listingService.search({
      categoryId: this.selectedCategoryId || undefined,
      keyword: this.keyword.trim() || undefined,
      maxPrice: this.maxPrice ?? undefined,
      location: this.location.trim() || undefined,
      sortBy: this.sortBy || undefined
    }).subscribe({
      next: (listings) => {
        this.listings.set(listings);
        this.isLoading.set(false);
      },
      error: () => {
        this.listings.set([]);
        this.hasError.set(true);
        this.isLoading.set(false);
      }
    });
  }

  onFilterChange(): void {
    this.loadListings();
  }

  clearFilters(): void {
    this.keyword = '';
    this.selectedCategoryId = '';
    this.maxPrice = null;
    this.location = '';
    this.sortBy = '';

    this.loadListings();
  }

  getConditionLabel(condition: ListingCondition): string {
    switch (condition) {
      case ListingCondition.New:
        return 'New';

      case ListingCondition.LikeNew:
        return 'Like New';

      case ListingCondition.Used:
        return 'Used';

      case ListingCondition.Refurbished:
        return 'Refurbished';

      default:
        return 'Unknown';
    }
  }
}