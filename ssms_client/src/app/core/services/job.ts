import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  JobResponse,
  CreateJobRequest,
  UpdateJobRequest,
  JobApplicationResponse,
  ApplyToJobRequest
} from '../models/job.model';

@Injectable({ providedIn: 'root' })
export class JobService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/jobs`;

  getById(id: string): Observable<JobResponse> {
    return this.http.get<JobResponse>(`${this.baseUrl}/${id}`);
  }

  getOpen(): Observable<JobResponse[]> {
    return this.http.get<JobResponse[]>(`${this.baseUrl}/open`);
  }

  search(params: { categoryId?: string; location?: string; minBudget?: number; maxBudget?: number }): Observable<JobResponse[]> {
    return this.http.get<JobResponse[]>(`${this.baseUrl}/search`, { params: params as any });
  }
  getMine(): Observable<JobResponse[]> {
    return this.http.get<JobResponse[]>(`${this.baseUrl}/mine`);
  }
  getAssignedToMe(): Observable<JobResponse[]> {
    return this.http.get<JobResponse[]>(`${this.baseUrl}/assigned-to-me`);
  }
  create(request: CreateJobRequest): Observable<JobResponse> {
    return this.http.post<JobResponse>(this.baseUrl, request);
  }

  update(id: string, request: UpdateJobRequest): Observable<JobResponse> {
    return this.http.put<JobResponse>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  apply(jobId: string, request: ApplyToJobRequest): Observable<JobApplicationResponse> {
    // Updated route to match [HttpPost("{id:guid}/apply")] on the backend
    return this.http.post<JobApplicationResponse>(`${this.baseUrl}/${jobId}/apply`, request);
  }

  getApplications(jobId: string): Observable<JobApplicationResponse[]> {
    return this.http.get<JobApplicationResponse[]>(`${this.baseUrl}/${jobId}/applications`);
  }

  acceptApplication(jobId: string, applicationId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/${jobId}/applications/${applicationId}/accept`, {});
  }

  start(jobId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/${jobId}/start`, {});
  }

  complete(jobId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/${jobId}/complete`, {});
  }

  cancel(jobId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/${jobId}/cancel`, {});
  }
}