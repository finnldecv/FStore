import { http } from '../../../lib/http';

export interface Product {
  id: string;
  name: string;
  description: string;
  price: number;
  currency: string;
  stockQuantity: number;
}

export const getProducts = async (): Promise<Product[]> => {
  const { data } = await http.get('/catalog/api/products');
  return data;
};