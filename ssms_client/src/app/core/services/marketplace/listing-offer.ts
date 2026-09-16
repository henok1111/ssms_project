import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ListingOfferResponse, MakeOfferRequest } from '../../models/marketplace/listing-offer.model';

@Injectable({ providedIn: 'root' })
export class ListingOfferService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/market_place/listings`;

  getForListing(listingId: string): Observable<ListingOfferResponse[]> {
    return this.http.get<ListingOfferResponse[]>(`${this.baseUrl}/${listingId}/offers`);
  }

  makeOffer(listingId: string, request: MakeOfferRequest): Observable<ListingOfferResponse> {
    return this.http.post<ListingOfferResponse>(`${this.baseUrl}/${listingId}/offers`, request);
  }

  accept(listingId: string, offerId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/${listingId}/offers/${offerId}/accept`, {});
  }

  reject(listingId: string, offerId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/${listingId}/offers/${offerId}/reject`, {});
  }
}