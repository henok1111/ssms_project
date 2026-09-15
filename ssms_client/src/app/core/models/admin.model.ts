export interface PendingApprovalResponse {
  id: string;
  userId: string;
  fullName: string;
  email: string;
  role: string;
  approvalStatus: string;
  createdAt: string;
}

export interface UserSummaryResponse {
  id: string;
  fullName: string;
  email: string;
  role: string;
  isActive: boolean;
  approvalStatus: string | null;
  createdAt: string;
}