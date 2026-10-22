import { Navbar, Nav, Container, Badge } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { getBasket } from '../api/basket';
import { getUserId } from '../lib/userId';

export const Header = () => {
  const userId = getUserId();

  const { data: basket } = useQuery({
    queryKey: ['basket', userId],
    queryFn: () => getBasket(userId)
  });

  const itemCount = basket?.items.reduce((sum, item) => sum + item.quantity, 0) ?? 0;

  return (
    <Navbar bg="dark" variant="dark" expand="lg">
      <Container>
        <Navbar.Brand as={Link} to="/">
          <i className="fa-solid fa-shop"></i> FStore
        </Navbar.Brand>

        <Navbar.Toggle />
        <Navbar.Collapse>
          <Nav className="ms-auto">
            <Nav.Link as={Link} to="/orders">
              Orders
            </Nav.Link>
            <Nav.Link as={Link} to="/cart">
              Cart
              {itemCount > 0 && (
                <Badge bg="danger" pill className="ms-2">
                  {itemCount}
                </Badge>
              )}
            </Nav.Link>
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
};