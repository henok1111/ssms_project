export enum NotificationType {
  NewJobMatch = 0,
  ApplicationAccepted = 1,
  QuoteApproved = 2,
  PaymentReleased = 3,
  NewMessage = 4,
  OrderStatusUpdate = 5,
  DisputeRaised = 6,
  ApprovalStatusChanged = 7,
  NewOfferReceived = 8,
  OfferAccepted = 9,
  OfferRejected = 10,
  ListingReported = 11
}

export interface NotificationResponse {
  id: string;
  type: NotificationType;
  content: string;
  isRead: boolean;
  relatedJobId: string | null;
  createdAt: string;
}