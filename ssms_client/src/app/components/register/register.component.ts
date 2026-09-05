import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { UserRole } from '../../models/auth.model';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  UserRole = UserRole;

  fullName = signal('');
  email = signal('');
  phoneNumber = signal('');
  password = signal('');
  role = signal<UserRole>(UserRole.Client);

  submitting = signal(false);
  error = signal<string | null>(null);

  onSubmit(): void {
    this.error.set(null);

    if (!this.fullName().trim() || !this.email().trim() || !this.phoneNumber().trim() || !this.password()) {
      this.error.set('Fill in all fields.');
      return;
    }

    this.submitting.set(true);

    this.auth.register({
      fullName: this.fullName().trim(),
      email: this.email().trim(),
      phoneNumber: this.phoneNumber().trim(),
      password: this.password(),
      role: this.role()
    }).subscribe({
      next: () => {
        this.submitting.set(false);
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.submitting.set(false);
        this.error.set(err?.error?.errors?.[0] ?? 'Registration failed. Check your details.');
      }
    });
  }
}