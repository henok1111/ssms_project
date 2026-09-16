import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { MaterialOrderResponse } from '../models/material.model';

@Injectable({ providedIn: 'root' })
export class MaterialOrderService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/material-orders`;

  getMine(): Observable<MaterialOrderResponse[]> {
    return this.http.get<MaterialOrderResponse[]>(`${this.baseUrl}/mine`);
  }

  confirm(orderId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/${orderId}/confirm`, {});
  }

  fulfill(orderId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/${orderId}/fulfill`, {});
  }

  cancel(orderId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/${orderId}/cancel`, {});
  }
}