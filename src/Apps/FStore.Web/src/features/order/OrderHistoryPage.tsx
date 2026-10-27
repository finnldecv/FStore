import { Container, Table, Badge, Spinner, Alert, Card } from 'react-bootstrap';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { getOrdersByUser } from '../../api/order';

export const OrderHistoryPage = () => {
  const { data: orders, isLoading, error } = useQuery({
    queryKey: ['orders'],
    queryFn: getOrdersByUser,
  });

  if (isLoading) {
    return (
      <Container className="py-5 text-center">
        <Spinner animation="border" />
      </Container>
    );
  }

  if (error) {
    return (
      <Container className="py-5">
        <Alert variant="danger">Failed to load orders.</Alert>
      </Container>
    );
  }

  if (!orders || orders.length === 0) {
    return (
      <Container className="py-5 text-center">
        <h2>No orders yet</h2>
        <p className="text-muted">You haven't placed any orders.</p>
        <Link to="/" className="btn btn-primary mt-3">Start Shopping</Link>
      </Container>
    );
  }

  const getStatusBadge = (status: string) => {
    switch (status) {
      case 'Pending': return <Badge bg="warning" text="dark">Pending</Badge>;
      case 'Confirmed': return <Badge bg="info">Confirmed</Badge>;
      case 'Shipped': return <Badge bg="primary">Shipped</Badge>;
      case 'Delivered': return <Badge bg="success">Delivered</Badge>;
      case 'Cancelled': return <Badge bg="danger">Cancelled</Badge>;
      default: return <Badge bg="secondary">{status}</Badge>;
    }
  };

  return (
    <Container className="py-4">
      <h1 className="mb-4">Your Orders</h1>

      <Card className="shadow-sm">
        <Card.Body className="p-0">
          <Table responsive className="mb-0 align-middle">
            <thead className="table-light">
              <tr>
                <th>Order ID</th>
                <th>Date</th>
                <th>Status</th>
                <th>Total</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {orders.map((order) => (
                <tr key={order.id}>
                  <td className="font-monospace text-muted">
                    {order.id.split('-')[0]}...
                  </td>
                  <td>{new Date(order.createdAt).toLocaleDateString()}</td>
                  <td>{getStatusBadge(order.status)}</td>
                  <td className="fw-bold">${order.totalAmount.toFixed(2)}</td>
                  <td className="text-end">
                    <Link to={`/orders/${order.id}`} className="btn btn-sm btn-outline-primary">
                      Details
                    </Link>
                  </td>
                </tr>
              ))}
            </tbody>
          </Table>
        </Card.Body>
      </Card>
    </Container>
  );
};