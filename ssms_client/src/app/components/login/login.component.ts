import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  email = signal('');
  password = signal('');
  submitting = signal(false);
  error = signal<string | null>(null);

  onSubmit(): void {
    this.error.set(null);

    if (!this.email().trim() || !this.password()) {
      this.error.set('Enter your email and password.');
      return;
    }

    this.submitting.set(true);

    this.auth.login({ email: this.email().trim(), password: this.password() }).subscribe({
      next: () => {
        this.submitting.set(false);
        this.router.navigate(['/']);
      },
      error: () => {
        this.submitting.set(false);
        this.error.set('Incorrect email or password.');
      }
    });
  }
}