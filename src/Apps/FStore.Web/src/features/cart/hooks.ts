import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { getBasket, addToBasket, updateBasketItem, clearBasket } from '../../api/basket';
import type { BasketItem } from '../../api/basket';
import { useAuth } from '../auth/AuthContext';

export const useBasket = () => {
  const { userId } = useAuth();

  return useQuery({
    queryKey: ['basket', userId],
    queryFn: () => getBasket(),
    enabled: !!userId,
  });
};

export const useAddToBasket = () => {
  const queryClient = useQueryClient();
  const { userId } = useAuth();

  return useMutation({
    mutationFn: (item: BasketItem) => addToBasket(item),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['basket', userId] });
    },
  });
};

export const useUpdateBasket = () => {
  const queryClient = useQueryClient();
  const { userId } = useAuth();

  return useMutation({
    mutationFn: (items: BasketItem[]) => updateBasketItem(items),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['basket', userId] });
    },
  });
};

export const useClearBasket = () => {
  const queryClient = useQueryClient();
  const { userId } = useAuth();

  return useMutation({
    mutationFn: clearBasket,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['basket', userId] });
    },
  });
};