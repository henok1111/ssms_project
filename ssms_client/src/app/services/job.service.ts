import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { JobResponse, JobSearchParams ,CreateJobRequest} from '../models/job.model';

@Injectable({ providedIn: 'root' })
export class JobService {
    private http = inject(HttpClient);
    private baseUrl = `${environment.apiUrl}/Jobs`;

    getAll(): Observable<JobResponse[]> {
        return this.http.get<JobResponse[]>(this.baseUrl);
    }

    getOpenJobs(): Observable<JobResponse[]> {
        return this.http.get<JobResponse[]>(`${this.baseUrl}/open`);
    }

    getById(id: string): Observable<JobResponse> {
        return this.http.get<JobResponse>(`${this.baseUrl}/${id}`);
    }

    search(params: JobSearchParams): Observable<JobResponse[]> {
        let httpParams = new HttpParams();

        if (params.categoryId) httpParams = httpParams.set('categoryId', params.categoryId);
        if (params.location) httpParams = httpParams.set('location', params.location);
        if (params.minBudget != null) httpParams = httpParams.set('minBudget', params.minBudget);
        if (params.maxBudget != null) httpParams = httpParams.set('maxBudget', params.maxBudget);

        return this.http.get<JobResponse[]>(`${this.baseUrl}/search`, { params: httpParams });
    }
    create(request: CreateJobRequest): Observable<JobResponse> {
        return this.http.post<JobResponse>(this.baseUrl, request);
    }
}