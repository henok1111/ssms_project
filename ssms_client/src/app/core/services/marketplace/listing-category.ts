import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ListingCategoryResponse } from '../../models/marketplace/listing.model';

@Injectable({ providedIn: 'root' })
export class ListingCategoryService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/market_place/listing-categories`;

  getAll(): Observable<ListingCategoryResponse[]> {
    return this.http.get<ListingCategoryResponse[]>(this.baseUrl);
  }
}