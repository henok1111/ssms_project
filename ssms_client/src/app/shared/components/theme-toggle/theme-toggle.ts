import { Component, inject } from '@angular/core';
import { ThemeService } from '../../theme/theme';

@Component({
  selector: 'app-theme-toggle',
  standalone: true,
  template: `
    <button (click)="theme.toggleTheme()" [attr.aria-label]="'Switch theme'">
      {{ theme.currentTheme() === 'light' ? '🌙' : '☀️' }}
    </button>
  `
})
export class ThemeToggleComponent {
  theme = inject(ThemeService);
}