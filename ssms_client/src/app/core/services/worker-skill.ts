import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { WorkerSkillResponse, AddSkillRequest } from '../models/skill.model';

@Injectable({ providedIn: 'root' })
export class WorkerSkillService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/worker-skills`;

  getMine(): Observable<WorkerSkillResponse[]> {
    return this.http.get<WorkerSkillResponse[]>(`${this.baseUrl}/mine`);
  }

  add(request: AddSkillRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(this.baseUrl, request);
  }

  remove(categoryId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${categoryId}`);
  }
}