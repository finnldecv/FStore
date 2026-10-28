import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor } from '../../test/test-utils';
import userEvent from '@testing-library/user-event';
import { CheckoutPage } from './CheckoutPage';
import { http } from '../../api/client';
import { useBasket } from '../cart/hooks';

vi.mock('../auth/AuthContext', async (importOriginal) => {
  const actual = await importOriginal<typeof import('../auth/AuthContext')>();
  return {
    ...actual,
    useAuth: () => ({ userId: '123', email: 'test@test.com', login: vi.fn(), logout: vi.fn() }),
  };
});

vi.mock('../../api/client', () => ({
  http: { post: vi.fn() },
}));

vi.mock('../cart/hooks', () => ({
  useBasket: vi.fn(),
}));

const mockNavigate = vi.fn();
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom');
  return { ...actual, useNavigate: () => mockNavigate };
});

describe('CheckoutPage', () => {
  beforeEach(() => {
    vi.clearAllMocks();

    vi.mocked(useBasket).mockReturnValue({
      data: {
        userId: '123',
        items: [{ productId: 'p1', productName: 'Laptop', price: 1000, quantity: 1 }],
      },
      isLoading: false,
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } as any);
  });

  it('renders the checkout form and total', () => {
    render(<CheckoutPage />);

    expect(screen.getByText('Checkout')).toBeInTheDocument();
    expect(screen.getByText(/Laptop/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /Place Order/i })).toBeInTheDocument();
  });

  it('submits order and redirects on success', async () => {
    const user = userEvent.setup();
    vi.mocked(http.post).mockResolvedValueOnce({ data: { message: 'Success' } });

    render(<CheckoutPage />);

    await user.type(screen.getByLabelText(/Shipping Address/i), '123 Main St');
    await user.type(screen.getByLabelText(/Order Notes/i), 'Leave at door');

    await user.click(screen.getByRole('button', { name: /Place Order/i }));

    await waitFor(() => {
      expect(http.post).toHaveBeenCalledWith('/basket/api/basket/checkout', {
        userId: '123',
        shippingAddress: '123 Main St',
        notes: 'Leave at door',
      });
      expect(mockNavigate).toHaveBeenCalledWith('/checkout/success');
    });
  });
});