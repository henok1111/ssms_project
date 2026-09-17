import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ListingService } from '../../../core/services/marketplace/listing';
import { ListingOfferService } from '../../../core/services/marketplace/listing-offer';
import { SavedListingService } from '../../../core/services/marketplace/saved-listing';
import { AuthService } from '../../../core/auth/auth';
import { ListingResponse, ListingStatus } from '../../../core/models/marketplace/listing.model';
import { ListingOfferResponse, OfferStatus } from '../../../core/models/marketplace/listing-offer.model';
import { Card } from '../../../shared/components/card/card';
import { LoadingSpinner } from '../../../shared/components/loading-spinner/loading-spinner';
import { ListingReportService } from '../../../core/services/marketplace/listing-report';

@Component({
  selector: 'app-listing-detail',
  standalone: true,
  imports: [ReactiveFormsModule, Card, LoadingSpinner],
  templateUrl: './listing-detail.html',
  styleUrl: './listing-detail.scss'
})
export class ListingDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private listingService = inject(ListingService);
  private offerService = inject(ListingOfferService);
  private savedListingService = inject(SavedListingService);
  private fb = inject(FormBuilder);
  authService = inject(AuthService);

  readonly ListingStatus = ListingStatus;
  readonly OfferStatus = OfferStatus;

  listing = signal<ListingResponse | null>(null);
  offers = signal<ListingOfferResponse[]>([]);
  isLoading = signal(true);
  actionLoading = signal(false);
  actionError = signal('');
  isSaved = signal(false);
private reportService = inject(ListingReportService);
showReportForm = signal(false);

reportForm = this.fb.group({
  reason: ['', [Validators.required]]
});
  offerForm = this.fb.group({
    offeredPrice: [0, [Validators.required, Validators.min(1)]],
    message: ['']
  });

  private get listingId(): string {
    return this.route.snapshot.paramMap.get('id')!;
  }

  get isOwner(): boolean {
    return this.listing()?.sellerId === this.authService.currentUser()?.userId;
  }

  get isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  ngOnInit(): void {
    this.listingService.getById(this.listingId).subscribe({
      next: (listing) => {
        this.listing.set(listing);
        this.isLoading.set(false);
        if (this.isOwner) this.loadOffers();
      },
      error: () => this.isLoading.set(false)
    });
  }

  loadOffers(): void {
    this.offerService.getForListing(this.listingId).subscribe(offers => this.offers.set(offers));
  }

  onMakeOffer(): void {
    if (this.offerForm.invalid) return;
    this.actionLoading.set(true);
    this.actionError.set('');

    const raw = this.offerForm.getRawValue();
    this.offerService.makeOffer(this.listingId, {
      offeredPrice: raw.offeredPrice!,
      message: raw.message || null
    }).subscribe({
      next: () => { this.actionLoading.set(false); this.offerForm.reset(); alert('Offer submitted!'); },
      error: (err) => { this.actionLoading.set(false); this.actionError.set(err.error?.message ?? 'Failed to submit offer.'); }
    });
  }

  onAcceptOffer(offerId: string): void {
    this.actionLoading.set(true);
    this.offerService.accept(this.listingId, offerId).subscribe({
      next: () => { this.actionLoading.set(false); this.loadOffers(); this.refreshListing(); },
      error: (err) => { this.actionLoading.set(false); this.actionError.set(err.error?.message ?? 'Failed to accept offer.'); }
    });
  }

  onRejectOffer(offerId: string): void {
    this.actionLoading.set(true);
    this.offerService.reject(this.listingId, offerId).subscribe({
      next: () => { this.actionLoading.set(false); this.loadOffers(); },
      error: (err) => { this.actionLoading.set(false); this.actionError.set(err.error?.message ?? 'Failed to reject offer.'); }
    });
  }

  onToggleSave(): void {
    if (this.isSaved()) {
      this.savedListingService.unsave(this.listingId).subscribe(() => this.isSaved.set(false));
    } else {
      this.savedListingService.save(this.listingId).subscribe(() => this.isSaved.set(true));
    }
  }

  onStartChat(): void {
    this.router.navigate(['/marketplace/chat'], { queryParams: { listingId: this.listingId } });
  }

  private refreshListing(): void {
    this.listingService.getById(this.listingId).subscribe(listing => this.listing.set(listing));
  }

onReportListing(): void {
  if (this.reportForm.invalid) return;
  this.actionLoading.set(true);
  const reason = this.reportForm.getRawValue().reason!;
  this.reportService.report(this.listingId, { reason }).subscribe({
    next: () => {
      this.actionLoading.set(false);
      this.showReportForm.set(false);
      this.reportForm.reset();
      alert('Listing reported. An admin will review it.');
    },
    error: (err) => {
      this.actionLoading.set(false);
      this.actionError.set(err.error?.message ?? 'Failed to report listing.');
    }
  });
}
}