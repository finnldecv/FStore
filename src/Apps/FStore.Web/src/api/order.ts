import { http } from './client';

export interface OrderItem {
  productId: string;
  productName: string;
  unitPrice: number;
  quantity: number;
  lineTotal: number;
}

export interface Order {
  id: string;
  userId: string;
  status: string;
  totalAmount: number;
  shippingAddress: string;
  notes?: string;
  createdAt: string;
  items: OrderItem[];
}

export const getOrderById = async (id: string): Promise<Order> => {
  const { data } = await http.get<Order>(`/order/api/orders/${id}`);
  return data;
};

export const getOrdersByUser = async (userId: string): Promise<Order[]> => {
  const { data } = await http.get<Order[]>(`/order/api/orders/user/${userId}`);
  return data;
};

export const cancelOrder = async (id: string, reason: string): Promise<void> => {
  await http.delete(`/order/api/orders/${id}`, { data: { reason } });
};