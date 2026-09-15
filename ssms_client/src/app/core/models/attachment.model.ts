export interface JobAttachmentResponse {
  id: string;
  jobId: string;
  fileUrl: string;
  fileType: string;
  isAiAnalyzed: boolean;
  createdAt: string;
}