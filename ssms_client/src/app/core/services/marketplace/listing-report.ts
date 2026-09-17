import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ListingReportResponse, ReportListingRequest, ResolveReportRequest } from '../../models/marketplace/listing-report.model';

@Injectable({ providedIn: 'root' })
export class ListingReportService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/market_place/listing-reports`;

  report(listingId: string, request: ReportListingRequest): Observable<ListingReportResponse> {
    return this.http.post<ListingReportResponse>(`${this.baseUrl}/listings/${listingId}`, request);
  }

  getPending(): Observable<ListingReportResponse[]> {
    return this.http.get<ListingReportResponse[]>(`${this.baseUrl}/pending`);
  }

  resolve(reportId: string, request: ResolveReportRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/${reportId}/resolve`, request);
  }
}