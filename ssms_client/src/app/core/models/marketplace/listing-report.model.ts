export enum ReportStatus {
  Pending = 0,
  Reviewed = 1,
  Dismissed = 2
}

export interface ListingReportResponse {
  id: string;
  listingId: string;
  listingTitle: string;
  reportedById: string;
  reportedByName: string;
  reason: string;
  status: ReportStatus;
  createdAt: string;
}

export interface ReportListingRequest {
  reason: string;
}

export interface ResolveReportRequest {
  removeListing: boolean;
}