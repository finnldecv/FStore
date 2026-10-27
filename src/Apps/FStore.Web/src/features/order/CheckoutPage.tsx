import { useState } from 'react';
import { Container, Row, Col, Card, Button, Form, Alert, Spinner } from 'react-bootstrap';
import { useNavigate, Link } from 'react-router-dom';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { http } from '../../api/client';
import { useBasket } from '../cart/hooks';
import { useAuth } from '../auth/AuthContext';

interface CheckoutResponse {
  message: string;
}

export const CheckoutPage = () => {
  const { userId } = useAuth();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { data: basket, isLoading } = useBasket();

  const [shippingAddress, setShippingAddress] = useState('');
  const [notes, setNotes] = useState('');
  const [errorMsg, setErrorMsg] = useState<string | null>(null);

  const checkoutMutation = useMutation({
    mutationFn: async () => {
      await http.post<CheckoutResponse>('/basket/api/basket/checkout', {
        userId,
        shippingAddress,
        notes,
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['basket', userId] });
      navigate('/checkout/success');
    },
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    onError: (error: any) => {
      setErrorMsg(error.message || 'Checkout failed. Please try again.');
    },
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setErrorMsg(null);
    if (!shippingAddress.trim()) {
      setErrorMsg('Shipping address is required.');
      return;
    }
    checkoutMutation.mutate();
  };

  if (isLoading) {
    return (
      <Container className="py-5 text-center">
        <Spinner animation="border" />
      </Container>
    );
  }

  if (!basket || basket.items.length === 0) {
    return (
      <Container className="py-5 text-center">
        <h2>Your cart is empty</h2>
        <p className="text-muted">You cannot checkout with an empty cart.</p>
        <Button
          // eslint-disable-next-line @typescript-eslint/no-explicit-any
          as={Link as any}
          to="/" variant="primary"
          className="mt-3">
          Browse Products
        </Button>
      </Container>
    );
  }

  const total = basket.items.reduce((sum, item) => sum + item.price * item.quantity, 0);

  return (
    <Container className="py-4">
      <h1 className="mb-4">Checkout</h1>

      <Row>
        <Col lg={8}>
          <Card className="shadow-sm mb-4">
            <Card.Body className="p-4">
              <h5 className="mb-4">Shipping Information</h5>

              {errorMsg && <Alert variant="danger">{errorMsg}</Alert>}

              <Form onSubmit={handleSubmit}>
                <Form.Group className="mb-3">
                  <Form.Label>Shipping Address</Form.Label>
                  <Form.Control
                    as="textarea"
                    rows={3}
                    placeholder="123 Main St, City, Country, Zip Code"
                    value={shippingAddress}
                    onChange={(e) => setShippingAddress(e.target.value)}
                    required
                  />
                </Form.Group>

                <Form.Group className="mb-4">
                  <Form.Label>Order Notes (Optional)</Form.Label>
                  <Form.Control
                    as="textarea"
                    rows={2}
                    placeholder="e.g., Leave at the front door"
                    value={notes}
                    onChange={(e) => setNotes(e.target.value)}
                  />
                </Form.Group>

                <Button
                  type="submit"
                  variant="primary"
                  size="lg"
                  className="w-100"
                  disabled={checkoutMutation.isPending}
                >
                  {checkoutMutation.isPending ? 'Processing Order...' : `Place Order ($${total.toFixed(2)})`}
                </Button>
              </Form>
            </Card.Body>
          </Card>
        </Col>

        <Col lg={4}>
          <Card className="shadow-sm">
            <Card.Body>
              <h5 className="mb-3">Order Summary</h5>
              <ul className="list-unstyled mb-3">
                {basket.items.map((item) => (
                  <li key={item.productId} className="d-flex justify-content-between mb-2">
                    <span className="text-truncate me-2">
                      {item.quantity}x {item.productName}
                    </span>
                    <span>${(item.price * item.quantity).toFixed(2)}</span>
                  </li>
                ))}
              </ul>
              <hr />
              <div className="d-flex justify-content-between mb-3">
                <strong>Total</strong>
                <strong className="text-primary fs-5">${total.toFixed(2)}</strong>
              </div>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
};