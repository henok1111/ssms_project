export enum PaymentStatus {
  Pending = 0,
  Held = 1,
  Released = 2,
  Refunded = 3,
  Failed = 4
}

export interface PaymentResponse {
  id: string;
  jobId: string;
  amount: number;
  platformCommission: number;
  amountReleasedToWorker: number;
  status: PaymentStatus;
  txRef: string;
  paidAt: string | null;
  releasedAt: string | null;
}

export interface InitiatePaymentResponse {
  paymentId: string;
  checkoutUrl: string;
  txRef: string;
  amount: number;
}