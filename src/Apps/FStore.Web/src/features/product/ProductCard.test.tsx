import { describe, it, expect, vi } from 'vitest';
import { render, screen, fireEvent } from '../../test/test-utils';
import { ProductCard } from './ProductCard';
import { type Product } from '../../api/catalog';
import * as catalogApi from '../../api/catalog';

vi.mock('../../api/catalog', () => ({
  getCategoryById: vi.fn(),
}));

const mockMutate = vi.fn();
vi.mock('../cart/hooks', () => ({
  useAddToBasket: () => ({ mutate: mockMutate, isPending: false }),
}));

const mockProduct: Product = {
  id: '1',
  name: 'Gaming Laptop',
  description: 'A very fast laptop',
  price: 1499.99,
  currency: 'USD',
  stockQuantity: 5,
  categoryId: 'cat1',
};

describe('ProductCard', () => {
  it('renders product details and category correctly', async () => {
    vi.mocked(catalogApi.getCategoryById).mockResolvedValueOnce({
      id: 'cat1',
      name: 'Electronics',
      productCount: 1,
    });

    render(<ProductCard product={mockProduct} />);

    expect(screen.getByText('Gaming Laptop')).toBeInTheDocument();
    expect(screen.getByText('$1499.99')).toBeInTheDocument();

    expect(await screen.findByText('Electronics')).toBeInTheDocument();
  });

  it('calls addToBasket when button is clicked', async () => {
    vi.mocked(catalogApi.getCategoryById).mockResolvedValueOnce({
      id: 'cat1', name: 'Electronics', productCount: 1,
    });

    render(<ProductCard product={mockProduct} />);

    await screen.findByText('Electronics');

    const button = screen.getByRole('button', { name: /Add to Cart/i });
    fireEvent.click(button);

    expect(mockMutate).toHaveBeenCalledWith({
      productId: '1',
      productName: 'Gaming Laptop',
      price: 1499.99,
      quantity: 1,
    });
  });

  it('disables button when out of stock', async () => {
    vi.mocked(catalogApi.getCategoryById).mockResolvedValueOnce({
      id: 'cat1', name: 'Electronics', productCount: 1,
    });

    const outOfStockProduct = { ...mockProduct, stockQuantity: 0 };
    render(<ProductCard product={outOfStockProduct} />);

    await screen.findByText('Electronics');

    const button = screen.getByRole('button', { name: /Add to Cart/i });
    expect(button).toBeDisabled();
    expect(screen.getByText('Out of stock')).toBeInTheDocument();
  });
});