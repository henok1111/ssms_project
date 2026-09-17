import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ListingMediaResponse } from '../../models/marketplace/listing.model';

@Injectable({ providedIn: 'root' })
export class ListingMediaService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/market_place/listings`;

  upload(listingId: string, file: File): Observable<ListingMediaResponse> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<ListingMediaResponse>(`${this.baseUrl}/${listingId}/media`, formData);
  }

  delete(listingId: string, mediaId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${listingId}/media/${mediaId}`);
  }
}