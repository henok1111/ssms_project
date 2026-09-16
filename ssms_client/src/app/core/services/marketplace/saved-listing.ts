import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ListingResponse } from '../../models/marketplace/listing.model';

@Injectable({ providedIn: 'root' })
export class SavedListingService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/market_place/saved-listings`;

  getMine(): Observable<ListingResponse[]> {
    return this.http.get<ListingResponse[]>(this.baseUrl);
  }

  save(listingId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/${listingId}`, {});
  }

  unsave(listingId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${listingId}`);
  }
}