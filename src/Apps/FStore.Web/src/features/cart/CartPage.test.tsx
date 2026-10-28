import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '../../test/test-utils';
import { CartPage } from './CartPage';

vi.mock('../auth/AuthContext', async (importOriginal) => {
  const actual = await importOriginal<typeof import('../auth/AuthContext')>();
  return {
    ...actual,
    useAuth: () => ({ userId: '123', email: 'test@test.com', login: vi.fn(), logout: vi.fn() }),
  };
});

const mockUpdateBasket = vi.fn();
const mockClearBasket = vi.fn();
vi.mock('./hooks', () => ({
  useBasket: vi.fn(),
  useUpdateBasket: () => ({ mutate: mockUpdateBasket, isPending: false }),
  useClearBasket: () => ({ mutate: mockClearBasket, isPending: false }),
}));

import { useBasket } from './hooks';

describe('CartPage', () => {
  it('shows empty state when basket is empty', () => {
    vi.mocked(useBasket).mockReturnValue({
      data: null,
      isLoading: false,
      error: null,
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } as any);

    render(<CartPage />);

    expect(screen.getByText(/Your cart is empty/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /Browse Products/i })).toBeInTheDocument();
  });

  it('renders items and calculates total correctly', () => {
    vi.mocked(useBasket).mockReturnValue({
      data: {
        userId: 'user1',
        items: [
          { productId: '1', productName: 'Laptop', price: 1000, quantity: 2 }, // 2000
          { productId: '2', productName: 'Mouse', price: 50, quantity: 1 },    // 50
        ],
      },
      isLoading: false,
      error: null,
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } as any);

    render(<CartPage />);

    expect(screen.getByText('Laptop')).toBeInTheDocument();
    expect(screen.getByText('Mouse')).toBeInTheDocument();

    expect(screen.getByText(/\$2000\.00/i)).toBeInTheDocument();

    expect(screen.getAllByText(/\$2050\.00/i).length).toBeGreaterThan(0);

    expect(screen.getByRole('button', { name: /Proceed to Checkout/i })).toBeInTheDocument();
  });
});