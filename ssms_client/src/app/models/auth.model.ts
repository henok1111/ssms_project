export enum UserRole {
  Client = 0,
  Worker = 1,
  Supplier = 2,
  Admin = 3
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  phoneNumber: string;
  password: string;
  role: UserRole;
  workerType?: number | null;
  serviceArea?: string | null;
  shopName?: string | null;
  supplierLocation?: string | null;
}

export interface LoginRequest {
  email: string;
  password: string;
}

// ASSUMPTION: adjust field names to match your actual AuthResponse DTO
export interface AuthUser {
  userId: string;   // was: id
  fullName: string;
  email: string;
  role: UserRole | string;  // comes back as "Client" (string), not the numeric enum
}