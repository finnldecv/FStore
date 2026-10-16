import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import {
  getBasket,
  addToBasket,
  updateBasketItem,
  clearBasket,
} from '../../api/basket';
import type { BasketItem } from '../../api/basket';
import { getUserId } from '../../lib/userId';

export const useBasket = () => {
  const userId = getUserId();

  return useQuery({
    queryKey: ['basket', userId],
    queryFn: () => getBasket(userId)
  });
};

export const useAddToBasket = () => {
  const queryClient = useQueryClient();
  const userId = getUserId();

  return useMutation({
    mutationFn: (item: BasketItem) => addToBasket(userId, item),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: ['basket', userId] })
  });
};

export const useUpdateBasket = () => {
  const queryClient = useQueryClient();
  const userId = getUserId();

  return useMutation({
    mutationFn: (item: BasketItem[]) => updateBasketItem(userId, item),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: ['basket', userId] })
  });
};

export const useClearBasket = () => {
  const queryClient = useQueryClient();
  const userId = getUserId();

  return useMutation({
    mutationFn: () => clearBasket(userId),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: ['basket', userId] })
  });
}