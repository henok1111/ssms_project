export enum ListingCondition {
  New = 0,
  LikeNew = 1,
  Used = 2,
  Refurbished = 3
}

export enum ListingStatus {
  Active = 0,
  Sold = 1,
  Removed = 2,
  Reserved = 3,
  Expired = 4
}

export enum ListingMediaType {
  Image = 0,
  Audio = 1
}

export interface ListingResponse {
  id: string;
  sellerId: string;
  sellerName: string;
  categoryId: string;
  categoryName: string;
  title: string;
  description: string;
  price: number;
  condition: ListingCondition;
  location: string;
  status: ListingStatus;
  imageUrls: string[];
  audioUrls: string[];
  viewCount: number;
  createdAt: string;
}

export interface CreateListingRequest {
  categoryId: string;
  title: string;
  description: string;
  price: number;
  condition: ListingCondition;
  location: string;
}

export interface UpdateListingRequest {
  categoryId: string;
  title: string;
  description: string;
  price: number;
  condition: ListingCondition;
  location: string;
}

export interface ListingMediaResponse {
  id: string;
  listingId: string;
  fileUrl: string;
  mediaType: ListingMediaType;
}

export interface ListingCategoryResponse {
  id: string;
  name: string;
}

export interface CreateListingCategoryRequest {
  name: string;
}