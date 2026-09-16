import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ListingService } from '../../../core/services/marketplace/listing';
import { ListingCategoryService } from '../../../core/services/marketplace/listing-category';
import { ListingResponse, ListingCategoryResponse } from '../../../core/models/marketplace/listing.model';
import { Card } from '../../../shared/components/card/card';
import { LoadingSpinner } from '../../../shared/components/loading-spinner/loading-spinner';

@Component({
  selector: 'app-listing-feed',
  standalone: true,
  imports: [RouterLink, FormsModule, Card, LoadingSpinner],
  templateUrl: './listing-feed.html',
  styleUrl: './listing-feed.scss'
})
export class ListingFeed implements OnInit {
  private listingService = inject(ListingService);
  private categoryService = inject(ListingCategoryService);

  listings = signal<ListingResponse[]>([]);
  categories = signal<ListingCategoryResponse[]>([]);
  isLoading = signal(true);

  keyword = '';
  selectedCategoryId = '';
  maxPrice: number | null = null;
  sortBy = '';

  ngOnInit(): void {
    this.categoryService.getAll().subscribe(cats => this.categories.set(cats));
    this.loadListings();
  }

  loadListings(): void {
    this.isLoading.set(true);
    this.listingService.search({
      categoryId: this.selectedCategoryId || undefined,
      keyword: this.keyword || undefined,
      maxPrice: this.maxPrice || undefined,
      sortBy: this.sortBy || undefined
    }).subscribe({
      next: (listings) => { this.listings.set(listings); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  onFilterChange(): void {
    this.loadListings();
  }
}