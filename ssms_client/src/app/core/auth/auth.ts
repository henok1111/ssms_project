import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  RegisterRequest,
  LoginRequest,
  AuthResponse,
  ResetPasswordRequest
} from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/auth`;

  // Holds the currently logged-in user's info app-wide, readable by any component.
  readonly currentUser = signal<AuthResponse | null>(null);

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/register`, request)
      .pipe(tap(user => this.currentUser.set(user)));
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/login`, request)
      .pipe(tap(user => this.currentUser.set(user)));
  }

  logout(): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/logout`, {})
      .pipe(tap(() => this.currentUser.set(null)));
  }

  getMe(): Observable<AuthResponse> {
    return this.http.get<AuthResponse>(`${this.baseUrl}/me`)
      .pipe(tap(user => this.currentUser.set(user)));
  }

  confirmEmail(userId: string, token: string): Observable<{ message: string }> {
    const params = { userId, token };
    return this.http.get<{ message: string }>(`${this.baseUrl}/confirm-email`, { params });
  }

  forgotPassword(email: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/forgot-password`, JSON.stringify(email), {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  resetPassword(request: ResetPasswordRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/reset-password`, request);
  }

  refreshToken(): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/refresh`, {});
  }

  uploadProfilePicture(file: File): Observable<{ profilePictureUrl: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ profilePictureUrl: string }>(`${this.baseUrl}/profile-picture`, formData);
  }

  isLoggedIn(): boolean {
    return this.currentUser() !== null;
  }
}