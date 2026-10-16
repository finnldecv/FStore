import type { ReactNode } from 'react';
import { Container } from 'react-bootstrap';
import { Header } from './Header';

interface LayoutProps {
  children: ReactNode;
}

export const Layout = ({ children }: LayoutProps) => (
  <>
    <Header />
    <Container className="py-4" >
      {children}
    </Container>
  </>
);