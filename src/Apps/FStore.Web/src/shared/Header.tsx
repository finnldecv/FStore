import { Navbar, Nav, Container, Badge, Button } from 'react-bootstrap';
import { Link, useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { getBasket } from '../api/basket';
import { useAuth } from '../features/auth/AuthContext';

export const Header = () => {
  const { userId, email, logout } = useAuth();
  const navigate = useNavigate();

  const { data: basket } = useQuery({
    queryKey: ['basket', userId],
    queryFn: () => getBasket(),
    enabled: !!userId,
  });

  const itemCount = basket?.items.reduce((sum, item) => sum + item.quantity, 0) ?? 0;

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <Navbar bg="dark" variant="dark" expand="lg">
      <Container>
        <Navbar.Brand as={Link} to="/">
          <i className="fa-solid fa-shop me-2"></i> FStore
        </Navbar.Brand>

        <Navbar.Toggle />

        <Navbar.Collapse>
          <Nav className="ms-auto align-items-center">
            {userId ? (
              <>
                <Nav.Link as={Link} to="/orders">Orders</Nav.Link>
                <Nav.Link as={Link} to="/cart">
                  Cart {itemCount > 0 && <Badge bg="danger" pill className="ms-1">{itemCount}</Badge>}
                </Nav.Link>
                <span className="text-light mx-3 small">{email}</span>
                <Button variant="outline-light" size="sm" onClick={handleLogout}>Logout</Button>
              </>
            ) : (
              <>
                <Nav.Link as={Link} to="/login">Login</Nav.Link>
                <Nav.Link as={Link} to="/register">Register</Nav.Link>
              </>
            )}
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
};