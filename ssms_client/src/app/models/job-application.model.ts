export enum ApplicationStatus {
  Pending = 0,
  Accepted = 1,
  Rejected = 2,
  Withdrawn = 3
}

export interface CreateJobApplicationRequest {
  proposedPrice: number;
  message?: string | null;
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