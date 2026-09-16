import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ListingResponse, CreateListingRequest, UpdateListingRequest } from '../../models/marketplace/listing.model';

@Injectable({ providedIn: 'root' })
export class ListingService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/market_place/listings`;

  getById(id: string): Observable<ListingResponse> {
    return this.http.get<ListingResponse>(`${this.baseUrl}/${id}`);
  }

  getActive(): Observable<ListingResponse[]> {
    return this.http.get<ListingResponse[]>(this.baseUrl);
  }

  search(params: { categoryId?: string; keyword?: string; maxPrice?: number; location?: string; sortBy?: string }): Observable<ListingResponse[]> {
    return this.http.get<ListingResponse[]>(`${this.baseUrl}/search`, { params: params as any });
  }

  getMine(): Observable<ListingResponse[]> {
    return this.http.get<ListingResponse[]>(`${this.baseUrl}/mine`);
  }

  create(request: CreateListingRequest): Observable<ListingResponse> {
    return this.http.post<ListingResponse>(this.baseUrl, request);
  }

  update(id: string, request: UpdateListingRequest): Observable<ListingResponse> {
    return this.http.put<ListingResponse>(`${this.baseUrl}/${id}`, request);
  }

  markAsSold(id: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/${id}/mark-sold`, {});
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}