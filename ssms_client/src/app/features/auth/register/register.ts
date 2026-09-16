import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/auth';
import { UserRole, WorkerType } from '../../../core/models/user.model';
import { LoadingSpinner } from '../../../shared/components/loading-spinner/loading-spinner';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, LoadingSpinner],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class Register {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  readonly UserRole = UserRole; // exposed for the template's role dropdown

  errorMessage = '';
  isLoading = false;

  form = this.fb.group({
    fullName: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: ['', [Validators.required]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    role: [UserRole.Client, [Validators.required]],

    // Worker-only fields
    onSite: [false],
    remote: [false],
    serviceArea: [''],

    // Supplier-only fields
    shopName: [''],
    supplierLocation: ['']
  });

  toNumber(value: unknown): number {
    return Number(value);
  }


  get isWorker(): boolean {
    return this.form.get('role')?.value === UserRole.Worker;
  }

  get isSupplier(): boolean {
    return this.form.get('role')?.value === UserRole.Supplier;
  }

 onSubmit(): void {
  if (this.form.invalid) return;

  const raw = this.form.getRawValue();

  let workerType: number | undefined;
  if (raw.role === UserRole.Worker) {
    let flags = 0;
    if (raw.onSite) flags |= WorkerType.OnSite;
    if (raw.remote) flags |= WorkerType.Remote;
    workerType = flags;
  }

  this.isLoading = true;
  this.errorMessage = '';

  this.authService.register({
    fullName: raw.fullName!,
    email: raw.email!,
    phoneNumber: raw.phoneNumber!,
    password: raw.password!,
    role: raw.role!,
    workerType: workerType as WorkerType | undefined,
    serviceArea: raw.role === UserRole.Worker ? raw.serviceArea ?? undefined : undefined,
    shopName: raw.role === UserRole.Supplier ? raw.shopName ?? undefined : undefined,
    supplierLocation: raw.role === UserRole.Supplier ? raw.supplierLocation ?? undefined : undefined
  }).subscribe({
    next: () => {
      this.isLoading = false;
      this.router.navigate(['/login']);
    },
    error: (err) => {
      this.isLoading = false;
      this.errorMessage = err.error?.errors?.[0] ?? 'Registration failed. Please try again.';
    }
  });
}
}