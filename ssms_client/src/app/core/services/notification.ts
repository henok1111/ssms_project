import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { NotificationResponse } from '../models/notification.model';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/notifications`;

  getMine(): Observable<NotificationResponse[]> {
    return this.http.get<NotificationResponse[]>(this.baseUrl);
  }

  markAsRead(id: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${id}/mark-read`, {});
  }
}