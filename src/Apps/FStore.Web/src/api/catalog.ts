import { http } from './client';

export interface Product {
  id: string;
  name: string;
  description: string;
  price: number;
  currency: string;
  stockQuantity: number;
  categoryId: string;
}

export interface Category {
  id: string;
  name: string;
  productCount: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export const getProducts = async (
  searchTerm?: string,
  pageNumber: number = 1,
  pageSize: number = 12
): Promise<PagedResult<Product>> => {
  const params = new URLSearchParams();
  if (searchTerm) params.append('search', searchTerm)
  params.append('pageNumber', pageNumber.toString());
  params.append('pageSize', pageSize.toString());

  const { data } = await http.get<PagedResult<Product>>('/catalog/api/products', { params });
  return data;
};

export const getProductById = async (id: string): Promise<Product> => {
  const { data } = await http.get<Product>(`/catalog/api/products/${id}`);
  return data;
};

export const getCategories = async (): Promise<Category[]> => {
  const { data } = await http.get<Category[]>('/catalog/api/categories');
  return data;
};

export const getCategoryById = async (id: string): Promise<Category> => {
  const { data } = await http.get<Category>(`/catalog/api/categories/${id}`);
  return data;
};