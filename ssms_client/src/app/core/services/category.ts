import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CategoryResponse } from '../models/category.model';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/categories`;

  getAll(isServiceCategory?: boolean): Observable<CategoryResponse[]> {
    const params = isServiceCategory !== undefined ? { isServiceCategory } : {};
    return this.http.get<CategoryResponse[]>(this.baseUrl, { params: params as any });
  }
}