import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { QuoteResponse, GenerateQuoteRequest } from '../models/quote.model';

@Injectable({ providedIn: 'root' })
export class QuoteService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/jobs`;

  getForJob(jobId: string): Observable<QuoteResponse> {
    return this.http.get<QuoteResponse>(`${this.baseUrl}/${jobId}/quote`);
  }

  generate(jobId: string, request: GenerateQuoteRequest): Observable<QuoteResponse> {
    return this.http.post<QuoteResponse>(`${this.baseUrl}/${jobId}/quote`, request);
  }

  approve(jobId: string, quoteId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/${jobId}/quote/approve?quoteId=${quoteId}`, {});
  }

  reject(jobId: string, quoteId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/${jobId}/quote/reject?quoteId=${quoteId}`, {});
  }
}