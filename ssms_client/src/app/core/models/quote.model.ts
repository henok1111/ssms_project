export enum QuoteStatus {
  PendingApproval = 0,
  Approved = 1,
  Rejected = 2
}

export interface QuoteResponse {
  id: string;
  jobId: string;
  laborCost: number;
  materialsCost: number;
  totalCost: number;
  status: QuoteStatus;
  createdAt: string;
}

export interface GenerateQuoteRequest {
  laborCost: number;
}