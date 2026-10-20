import { Container, Button, Card } from 'react-bootstrap';
import { Link } from 'react-router-dom';

export const CheckoutSuccessPage = () => {
  return (
    <Container className="py-5 text-center">
      <Card className="shadow-sm mx-auto" style={{ maxWidth: '600px' }}>
        <Card.Body className="p-5">
          <div style={{ fontSize: '4rem' }}><i className="fa-solid fa-hands-clapping"></i></div>
          <h1 className="mt-3 text-success">Order Placed!</h1>
          <p className="text-muted mb-4">
            Thank you for your purchase. We have received your order and are processing it now.
          </p>
          <p className="mb-4">
            You will receive an email confirmation shortly.
          </p>

          <Button
            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            as={Link as any}
            to="/" variant="primary"
            size="lg">
            Continue Shopping
          </Button>
        </Card.Body>
      </Card>
    </Container>
  );
};