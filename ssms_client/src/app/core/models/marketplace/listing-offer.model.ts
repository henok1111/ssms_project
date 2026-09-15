export enum OfferStatus {
  Pending = 0,
  Accepted = 1,
  Rejected = 2
}

export interface ListingOfferResponse {
  id: string;
  listingId: string;
  buyerId: string;
  buyerName: string;
  offeredPrice: number;
  message: string | null;
  status: OfferStatus;
  createdAt: string;
}

export interface MakeOfferRequest {
  offeredPrice: number;
  message: string | null;
}