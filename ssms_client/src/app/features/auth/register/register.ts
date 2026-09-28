import { Component, inject, signal } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { AuthService } from '../../../core/auth/auth';
import {
  UserRole,
  WorkerType
} from '../../../core/models/user.model';

import { LoadingSpinner } from '../../../shared/components/loading-spinner/loading-spinner';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    LoadingSpinner
  ],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class Register {

  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  // Make UserRole available in the HTML template
  readonly UserRole = UserRole;

  // Loading and error signals
  errorMessage = signal('');
  isLoading = signal(false);

  // Registration form
  form = this.fb.group({

    // Basic information
    fullName: [
      '',
      [Validators.required]
    ],

    email: [
      '',
      [
        Validators.required,
        Validators.email
      ]
    ],

    phoneNumber: [
      '',
      [Validators.required]
    ],

    password: [
      '',
      [
        Validators.required,
        Validators.minLength(8)
      ]
    ],

    // Default role is Client
    role: [
      UserRole.Client,
      [Validators.required]
    ],

    // Worker fields
    onSite: [false],
    remote: [false],
    serviceArea: [''],

    // Supplier fields
    shopName: [''],
    supplierLocation: ['']
  });


  // Returns true when Worker is selected
  get isWorker(): boolean {
    return this.form.get('role')?.value === UserRole.Worker;
  }


  // Returns true when Supplier is selected
  get isSupplier(): boolean {
    return this.form.get('role')?.value === UserRole.Supplier;
  }


  // Extract error message from backend
  private extractError(err: any): string {

    const errors = err.error?.errors;

    // Example:
    // errors: ["Email already exists"]
    if (Array.isArray(errors)) {
      return errors[0];
    }

    // Example:
    // errors: {
    //   Email: ["Email is already taken"]
    // }
    if (errors && typeof errors === 'object') {

      const firstError = Object.values(errors)
        .flat()[0];

      if (firstError) {
        return String(firstError);
      }
    }

    // ASP.NET Core ProblemDetails
    return (
      err.error?.title ??
      'Registration failed. Please try again.'
    );
  }


  // Submit registration
  onSubmit(): void {

    // Stop if form is invalid
    if (this.form.invalid) {

      this.form.markAllAsTouched();

      return;
    }


    // Get form values
    const raw = this.form.getRawValue();


    // --------------------------------
    // Worker Type
    // --------------------------------

    let workerType: number | undefined;

    if (raw.role === UserRole.Worker) {

      let flags = 0;

      // On-site selected
      if (raw.onSite) {
        flags |= WorkerType.OnSite;
      }

      // Remote selected
      if (raw.remote) {
        flags |= WorkerType.Remote;
      }

      workerType = flags;
    }


    // Start loading
    this.isLoading.set(true);

    // Clear previous error
    this.errorMessage.set('');


    // --------------------------------
    // Send registration request
    // --------------------------------

    this.authService.register({

      fullName: raw.fullName!,

      email: raw.email!,

      phoneNumber: raw.phoneNumber!,

      password: raw.password!,

      role: raw.role!,

      // Worker information
      workerType:
        workerType as WorkerType | undefined,

      serviceArea:
        raw.role === UserRole.Worker
          ? raw.serviceArea ?? undefined
          : undefined,

      // Supplier information
      shopName:
        raw.role === UserRole.Supplier
          ? raw.shopName ?? undefined
          : undefined,

      supplierLocation:
        raw.role === UserRole.Supplier
          ? raw.supplierLocation ?? undefined
          : undefined

    }).subscribe({

      // Registration successful
      next: () => {

        this.isLoading.set(false);

        // Go to login page
        this.router.navigate(['/login']);
      },


      // Registration failed
      error: (err) => {

        this.isLoading.set(false);

        this.errorMessage.set(
          this.extractError(err)
        );
      }

    });
  }
}