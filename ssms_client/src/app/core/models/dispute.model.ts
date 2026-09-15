export enum DisputeStatus {
  Open = 0,
  UnderReview = 1,
  Resolved = 2,
  Rejected = 3
}

export interface DisputeResponse {
  id: string;
  jobId: string;
  raisedById: string;
  raisedByName: string;
  reason: string;
  status: DisputeStatus;
  adminResolutionNote: string | null;
  createdAt: string;
}

export interface RaiseDisputeRequest {
  jobId: string;
  reason: string;
}

export interface ResolveDisputeRequest {
  adminResolutionNote: string;
  approve: boolean;
}