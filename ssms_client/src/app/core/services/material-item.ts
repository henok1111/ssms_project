import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { MaterialItemResponse, CreateMaterialItemRequest, UpdateMaterialItemRequest } from '../models/material.model';

@Injectable({ providedIn: 'root' })
export class MaterialItemService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/materialitems`;

  getById(id: string): Observable<MaterialItemResponse> {
    return this.http.get<MaterialItemResponse>(`${this.baseUrl}/${id}`);
  }

  search(params: { categoryId?: string; name?: string; sortBy?: string }): Observable<MaterialItemResponse[]> {
    return this.http.get<MaterialItemResponse[]>(`${this.baseUrl}/search`, { params: params as any });
  }

  getMine(): Observable<MaterialItemResponse[]> {
    return this.http.get<MaterialItemResponse[]>(`${this.baseUrl}/mine`);
  }

  create(request: CreateMaterialItemRequest): Observable<MaterialItemResponse> {
    return this.http.post<MaterialItemResponse>(this.baseUrl, request);
  }

  update(id: string, request: UpdateMaterialItemRequest): Observable<MaterialItemResponse> {
    return this.http.put<MaterialItemResponse>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}