import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DisputeResponse, RaiseDisputeRequest, ResolveDisputeRequest } from '../models/dispute.model';

@Injectable({ providedIn: 'root' })
export class DisputeService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/disputes`;

  getForJob(jobId: string): Observable<DisputeResponse[]> {
    return this.http.get<DisputeResponse[]>(`${this.baseUrl}/job/${jobId}`);
  }

  getAllOpen(): Observable<DisputeResponse[]> {
    return this.http.get<DisputeResponse[]>(`${this.baseUrl}/open`);
  }

  raise(request: RaiseDisputeRequest): Observable<DisputeResponse> {
    return this.http.post<DisputeResponse>(this.baseUrl, request);
  }

  resolve(disputeId: string, request: ResolveDisputeRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/${disputeId}/resolve`, request);
  }
}