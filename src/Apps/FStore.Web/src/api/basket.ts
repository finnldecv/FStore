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

export const getBasket = async (userId: string): Promise<Basket | null> => {
  try {
    const { data } = await http.get<Basket>(`/basket/api/basket/${userId}`);
    return data;
  } catch (error) {
    if (isNotFound(error)) return null;
    throw error;
  }
};

export const addToBasket = async (
  userId: string,
  item: BasketItem
): Promise<void> => {
  const existing = await getBasket(userId);

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

export const updateBasketItem = async (
  userId: string,
  items: BasketItem[]
): Promise<void> => {
  await http.put(`/basket/api/basket/${userId}`, { userId, items });
};

export const clearBasket = async (userId: string): Promise<void> => {
  try {
    await http.delete(`/basket/api/basket/${userId}`);
  } catch (error) {
    if (isNotFound(error)) return;
    throw error;
  }
};