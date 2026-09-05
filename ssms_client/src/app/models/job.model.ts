export enum JobType {
  OnSite = 0,
  Remote = 1
  // adjust to match your actual JobType enum values
}

export enum JobStatus {
  Open = 0,
  Assigned = 1,
  InProgress = 2,
  Completed = 3,
  Closed = 4,
  Cancelled = 5
}

export interface JobResponse {
  id: string;
  clientId: string;
  categoryId: string;
  title: string;
  description: string;
  jobType: JobType;
  location: string | null;
  budget: number;
  status: JobStatus;
  assignedWorkerId: string | null;
  createdAt: string;
}

export interface JobSearchParams {
  categoryId?: string;
  location?: string;
  minBudget?: number;
  maxBudget?: number;
}
export interface CreateJobRequest {
  categoryId: string;
  title: string;
  description: string;
  jobType: JobType;
  location: string | null;
  budget: number;
}