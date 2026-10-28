import { describe, it, vi, expect, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '../../test/test-utils';
import { LoginPage } from './LoginPage';
import { http } from '../../api/client';

vi.mock('../../api/client', () => ({
  http: {
    post: vi.fn(),
  }
}));

const mockLogin = vi.fn();
vi.mock('./AuthContext', async (importOriginal) => {
  const actual = await importOriginal<typeof import('./AuthContext')>();
  return {
    ...actual,
    useAuth: () => ({ login: mockLogin }),
  };
});

describe('LoginPage', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('renders login form', () => {
    render(<LoginPage />);
    expect(screen.getByLabelText(/Email/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/Password/i)).toBeInTheDocument();
  });

  it('submits the form and calls login on success', async () => {
    vi.mocked(http.post).mockResolvedValueOnce({ data: { token: 'fake-jwt-token' } });

    render(<LoginPage />);

    fireEvent.change(screen.getByLabelText(/Email/i), { target: { value: 'test@test.com' } });
    fireEvent.change(screen.getByLabelText(/Password/i), { target: { value: 'Password123!' } });
    fireEvent.click(screen.getByRole('button', { name: /Login/i }));

    await waitFor(() => {
      expect(http.post).toHaveBeenCalledWith('/identity/api/auth/login', {
        email: 'test@test.com',
        password: 'Password123!',
      });
      expect(mockLogin).toHaveBeenCalledWith('fake-jwt-token');
    });
  });

  it('shows error message on failed login', async () => {
    vi.mocked(http.post).mockRejectedValueOnce({
      response: { data: { error: 'Invalid credentials' } }
    });

    render(<LoginPage />);

    fireEvent.change(screen.getByLabelText(/Email/i), { target: { value: 'bad@test.com' } });
    fireEvent.change(screen.getByLabelText(/Password/i), { target: { value: 'worng' } });
    fireEvent.click(screen.getByRole('button', { name: /Login/i }));

    await waitFor(() => {
      expect(screen.getByText('Invalid credentials')).toBeInTheDocument();
    });
  });
});