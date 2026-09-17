import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { MessageResponse, SendMessageRequest } from '../models/message.model';

@Injectable({ providedIn: 'root' })
export class MessageService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/jobs`;

  getForJob(jobId: string): Observable<MessageResponse[]> {
    return this.http.get<MessageResponse[]>(`${this.baseUrl}/${jobId}/messages`);
  }

  send(jobId: string, request: SendMessageRequest): Observable<MessageResponse> {
    return this.http.post<MessageResponse>(`${this.baseUrl}/${jobId}/messages`, request);
  }

  markAsRead(jobId: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${jobId}/messages/mark-read`, {});
  }
}