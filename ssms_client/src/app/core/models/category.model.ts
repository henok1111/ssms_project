export interface CategoryResponse {
  id: string;
  name: string;
  isServiceCategory: boolean;
}

export interface CreateCategoryRequest {
  name: string;
  isServiceCategory: boolean;
}

export interface UpdateCategoryRequest {
  name: string;
  isServiceCategory: boolean;
}