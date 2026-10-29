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

export const getProducts = async (searchTerm?: string): Promise<Product[]> => {
  const params = new URLSearchParams();
  if (searchTerm) params.append('search', searchTerm)

  const { data } = await http.get<Product[]>('/catalog/api/products', { params });
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