export enum OrderStatus {
  Pending = 0,
  Confirmed = 1,
  Fulfilled = 2,
  Cancelled = 3
}

export interface MaterialItemResponse {
  id: string;
  supplierId: string;
  supplierShopName: string;
  categoryId: string;
  categoryName: string;
  name: string;
  unit: string;
  pricePerUnit: number;
  stockQuantity: number;
  createdAt: string;
}

export interface CreateMaterialItemRequest {
  categoryId: string;
  name: string;
  unit: string;
  pricePerUnit: number;
  stockQuantity: number;
}

export interface UpdateMaterialItemRequest {
  categoryId: string;
  name: string;
  unit: string;
  pricePerUnit: number;
  stockQuantity: number;
}

export interface MaterialRequestResponse {
  id: string;
  jobId: string;
  materialItemId: string;
  materialItemName: string;
  supplierShopName: string;
  quantityNeeded: number;
  unitPriceAtRequest: number;
  lineTotal: number;
}

export interface AddMaterialRequestRequest {
  materialItemId: string;
  quantityNeeded: number;
}

export interface MaterialOrderResponse {
  id: string;
  jobMaterialRequestId: string;
  materialItemName: string;
  supplierId: string;
  supplierShopName: string;
  quantityOrdered: number;
  totalPrice: number;
  status: OrderStatus;
  createdAt: string;
}