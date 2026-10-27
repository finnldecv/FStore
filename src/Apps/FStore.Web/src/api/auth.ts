import { jwtDecode } from 'jwt-decode';

export const getUserIdFromToken = (): string | null => {
  const token = localStorage.getItem('token');
  if (!token) return null;
  try {
    const decoded = jwtDecode<{ sub: string }>(token);
    return decoded.sub;
  } catch {
    return null;
  }
};