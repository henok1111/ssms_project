import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PaymentResponse, InitiatePaymentResponse } from '../models/payment.model';

@Injectable({ providedIn: 'root' })
export class PaymentService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/payments`;

  getForJob(jobId: string): Observable<PaymentResponse> {
    return this.http.get<PaymentResponse>(`${this.baseUrl}/job/${jobId}`);
  }

  initiate(quoteId: string): Observable<InitiatePaymentResponse> {
    return this.http.post<InitiatePaymentResponse>(`${this.baseUrl}/initiate/${quoteId}`, {});
  }

  release(paymentId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/${paymentId}/release`, {});
  }
}