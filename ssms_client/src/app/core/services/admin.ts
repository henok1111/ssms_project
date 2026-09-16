import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PendingApprovalResponse, UserSummaryResponse } from '../models/admin.model';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/admin`;

  getPendingWorkers(): Observable<PendingApprovalResponse[]> {
    return this.http.get<PendingApprovalResponse[]>(`${this.baseUrl}/workers/pending`);
  }

  getPendingSuppliers(): Observable<PendingApprovalResponse[]> {
    return this.http.get<PendingApprovalResponse[]>(`${this.baseUrl}/suppliers/pending`);
  }

  approveWorker(id: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/workers/${id}/approve`, {});
  }

  rejectWorker(id: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/workers/${id}/reject`, {});
  }

  approveSupplier(id: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/suppliers/${id}/approve`, {});
  }

  rejectSupplier(id: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/suppliers/${id}/reject`, {});
  }

  getUsers(role?: number): Observable<UserSummaryResponse[]> {
    const params = role !== undefined ? { role } : {};
    return this.http.get<UserSummaryResponse[]>(`${this.baseUrl}/users`, { params: params as any });
  }

  deactivateUser(id: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/users/${id}/deactivate`, {});
  }

  reactivateUser(id: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/users/${id}/reactivate`, {});
  }
}