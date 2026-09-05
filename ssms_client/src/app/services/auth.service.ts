import { Injectable, inject, signal, computed, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { AuthUser, LoginRequest, RegisterRequest } from '../models/auth.model';

const STORAGE_KEY = 'ssms-user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private platformId = inject(PLATFORM_ID);
  private isBrowser = isPlatformBrowser(this.platformId);
  private baseUrl = `${environment.apiUrl}/Auth`;

  currentUser = signal<AuthUser | null>(this.restoreUser());
  isAuthenticated = computed(() => this.currentUser() !== null);

  login(request: LoginRequest): Observable<AuthUser> {
    return this.http
      .post<AuthUser>(`${this.baseUrl}/login`, request, { withCredentials: true })
      .pipe(tap((user) => this.setUser(user)));
  }

  register(request: RegisterRequest): Observable<AuthUser> {
    return this.http
      .post<AuthUser>(`${this.baseUrl}/register`, request, { withCredentials: true })
      .pipe(tap((user) => this.setUser(user)));
  }

  logout(): Observable<unknown> {
    return this.http
      .post(`${this.baseUrl}/logout`, {}, { withCredentials: true })
      .pipe(tap(() => this.clearUser()));
  }

  private setUser(user: AuthUser): void {
    this.currentUser.set(user);
    if (this.isBrowser) {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(user));
    }
  }

  private clearUser(): void {
    this.currentUser.set(null);
    if (this.isBrowser) {
      localStorage.removeItem(STORAGE_KEY);
    }
  }

  private restoreUser(): AuthUser | null {
    if (!this.isBrowser) return null;
    const raw = localStorage.getItem(STORAGE_KEY);
    return raw ? JSON.parse(raw) : null;
  }
}