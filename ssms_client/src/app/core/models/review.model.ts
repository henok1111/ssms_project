export interface ReviewResponse {
  id: string;
  jobId: string;
  reviewerId: string;
  reviewerName: string;
  revieweeId: string;
  revieweeName: string;
  rating: number;
  comment: string | null;
  createdAt: string;
}

export interface CreateReviewRequest {
  jobId: string;
  revieweeId: string;
  rating: number;
  comment: string | null;
}