export enum UserRole {
  Client = 0,
  Worker = 1,
  Supplier = 2,
  Admin = 3
}

export enum WorkerType {
  OnSite = 1,
  Remote = 2
}

export enum ApprovalStatus {
  Pending = 0,
  Approved = 1,
  Rejected = 2
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  phoneNumber: string;
  password: string;
  role: UserRole;
  workerType?: WorkerType;
  serviceArea?: string;
  shopName?: string;
  supplierLocation?: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  userId: string;
  fullName: string;
  email: string;
  role: string;
  profilePictureUrl?: string | null;
}

export interface ResetPasswordRequest {
  email: string;
  token: string;
  newPassword: string;
}