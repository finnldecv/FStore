import { getUserIdFromToken } from './auth.ts'
import { http } from './client';
import { isNotFound } from './errors';

export interface BasketItem {
  productId: string;
  productName: string;
  price: number;
  quantity: number;
}

export interface Basket {
  userId: string;
  items: BasketItem[];
}

export const getBasket = async (): Promise<Basket | null> => {
  const userId = getUserIdFromToken();
  if (!userId) return null;

  try {
    const { data } = await http.get<Basket>(`/basket/api/basket/${userId}`);
    return data;
  } catch (error) {
    if (isNotFound(error)) return null;
    throw error;
  }
};

export const addToBasket = async (item: BasketItem): Promise<void> => {
  const userId = getUserIdFromToken();
  if (!userId) throw new Error('User not authenticated');

  const existing = await getBasket();
  const items = existing?.items ?? [];
  const existingIndex = items.findIndex((i) => i.productId === item.productId);

  if (existingIndex >= 0) {
    items[existingIndex] = {
      ...items[existingIndex],
      quantity: items[existingIndex].quantity + item.quantity,
    };
  } else {
    items.push(item);
  }

  const payload = { userId, items };

  if (existing) {
    await http.put(`/basket/api/basket/${userId}`, payload);
  } else {
    await http.post('/basket/api/basket', payload);
  }
};

export const updateBasketItem = async (items: BasketItem[]): Promise<void> => {
  const userId = getUserIdFromToken();
  if (!userId) throw new Error('User not authenticated');

  await http.put(`/basket/api/basket/${userId}`, { userId, items });
};

export const clearBasket = async (): Promise<void> => {
  const userId = getUserIdFromToken();
  if (!userId) return;

  try {
    await http.delete(`/basket/api/basket/${userId}`);
  } catch (error) {
    if (isNotFound(error)) return;
    throw error;
  }
};