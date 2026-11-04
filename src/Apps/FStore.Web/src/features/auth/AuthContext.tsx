import { createContext, useContext, useState } from 'react';
import type { ReactNode } from 'react';
import { jwtDecode } from 'jwt-decode';

interface JwtPayload {
  sub: string;
  email: string;
  name: string;
  exp: number;
  role?: string | string[];
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: string | string[];
}

interface AuthContextType {
  token: string | null;
  userId: string | null;
  email: string | null;
  isAdmin: boolean;
  login: (token: string) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [token, setToken] = useState<string | null>(() => {
    const savedToken = localStorage.getItem('token');
    if (!savedToken) return null;

    try {
      const decoded = jwtDecode<JwtPayload>(savedToken);
      if (decoded.exp * 1000 < Date.now()) {
        localStorage.removeItem('token');
        return null;
      }
      return savedToken;
    } catch {
      localStorage.removeItem('token');
      return null;
    }
  });

  let userId: string | null = null;
  let email: string | null = null;
  let isAdmin = false;

  if (token) {
    try {
      const decoded = jwtDecode<JwtPayload>(token);
      userId = decoded.sub;
      email = decoded.email;

      const rolesClaim = decoded.role || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

      if (rolesClaim) {
        const rolesArray = Array.isArray(rolesClaim) ? rolesClaim : [rolesClaim];
        isAdmin = rolesArray.includes('Admin');
      }
    } catch {
      // If token is malformed, these remain null
    }
  }

  const login = (newToken: string) => {
    localStorage.setItem('token', newToken);
    setToken(newToken);
  };

  const logout = () => {
    localStorage.removeItem('token');
    setToken(null);
  };

  return (
    <AuthContext.Provider value={{ token, userId, email, isAdmin, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

// eslint-disable-next-line react-refresh/only-export-components
export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within AuthProvider');
  return context;
};