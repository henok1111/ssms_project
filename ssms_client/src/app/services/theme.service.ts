import { Injectable, signal, effect, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

export type Theme = 'light' | 'dark';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private platformId = inject(PLATFORM_ID);
  private isBrowser = isPlatformBrowser(this.platformId);

  theme = signal<Theme>(this.getInitialTheme());

  constructor() {
    effect(() => {
      if (!this.isBrowser) return;

      document.documentElement.setAttribute('data-theme', this.theme());
      localStorage.setItem('ssms-theme', this.theme());
    });
  }

  toggle(): void {
    this.theme.set(this.theme() === 'light' ? 'dark' : 'light');
  }

  private getInitialTheme(): Theme {
    if (!this.isBrowser) return 'light'; // safe default during server render

    const saved = localStorage.getItem('ssms-theme') as Theme | null;
    if (saved) return saved;
    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
  }
}