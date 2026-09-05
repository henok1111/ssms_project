import { Component, inject } from '@angular/core';
import { ThemeService } from '../../services/theme.service';

@Component({
  selector: 'app-theme-toggle',
  standalone: true,
  template: `
    <button
      class="theme-toggle"
      (click)="theme.toggle()"
      [attr.aria-label]="theme.theme() === 'light' ? 'Switch to dark mode' : 'Switch to light mode'"
    >
      {{ theme.theme() === 'light' ? '🌙' : '☀️' }}
    </button>
  `,
  styles: [`
    .theme-toggle {
  position: fixed;
  top: 1.25rem;      // was: bottom: 1.25rem;
  right: 1.25rem;
  width: 44px;
  height: 44px;
  border-radius: 50%;
  border: 1px solid var(--line);
  background: var(--surface);
  font-size: 1.1rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 2px 8px rgba(0,0,0,0.12);
  z-index: 100;

  &:focus-visible {
    outline: 2px solid var(--accent);
    outline-offset: 2px;
  }

    }
  `]
})
export class ThemeToggleComponent {
  theme = inject(ThemeService);
}