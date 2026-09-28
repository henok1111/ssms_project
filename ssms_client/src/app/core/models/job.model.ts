export enum JobType {
  OnSite = 0,
  Remote = 1
}

export enum JobStatus {
  Open = 0,
  Assigned = 1,
  InProgress = 2,
  Completed = 3,
  Closed = 4,
  Cancelled = 5
}

export enum ApplicationStatus {
  Pending = 0,
  Accepted = 1,
  Rejected = 2,
  Withdrawn = 3
}

export interface JobResponse {
  id: string;
  clientId: string;
  clientName: string;
  categoryId: string;
  categoryName: string;
  title: string;
  description: string;
  jobType: JobType;
  location: string | null;
  budget: number;
  status: JobStatus;
  assignedWorkerId: string | null;
  assignedWorkerName: string | null;
  createdAt: string;
  clientUserId: string;
assignedWorkerUserId: string | null;
}

export interface CreateJobRequest {
  categoryId: string;
  title: string;
  description: string;
  jobType: JobType;
  location: string | null;
  budget: number;
}

export interface UpdateJobRequest {
  categoryId: string;
  title: string;
  description: string;
  jobType: JobType;
  location: string | null;
  budget: number;
}

export interface JobApplicationResponse {
  id: string;
  jobId: string;
  workerId: string;
  workerName: string;
  proposedPrice: number;
  message: string | null;
  status: ApplicationStatus;
  createdAt: string;
}

export interface ApplyToJobRequest {
  proposedPrice: number;
  message: string | null;
}