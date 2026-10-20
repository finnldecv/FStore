import { Container, Row, Col, Card, Button, Table, Alert, Spinner, Form } from 'react-bootstrap';
import { Link, useNavigate } from 'react-router-dom';
import { useBasket, useUpdateBasket, useClearBasket } from './hooks';

export const CartPage = () => {
  const navigate = useNavigate();
  const { data: basket, isLoading, error } = useBasket();
  const updateBasket = useUpdateBasket();
  const clearBasket = useClearBasket();

  const handleQuantityChange = (productId: string, newQuantity: number) => {
    if (!basket || newQuantity < 1) return;

    const updatedItems = basket.items.map((item) =>
      item.productId === productId ? { ...item, quantity: newQuantity } : item
    );

    updateBasket.mutate(updatedItems);
  };

  const handleRemoveItem = (productId: string) => {
    if (!basket) return;
    const updatedItems = basket.items.filter((item) => item.productId !== productId);

    if (updatedItems.length === 0) {
      clearBasket.mutate()
    } else {
      updateBasket.mutate(updatedItems);
    }
  };

  const handleClearCart = () => {
    if (window.confirm('Are you sure you want to clear your cart?')) {
      clearBasket.mutate();
    }
  };

  if (isLoading) {
    return (
      <Container className='py-5 text-center'>
        <Spinner animation='border' />
        <p className='mt-2 text-muted'>Loading your cart...</p>
      </Container>
    );
  }

  if (error) {
    return (
      <Container className='py-5'>
        <Alert variant='danger'>Failed to load cart: {(error as Error).message}</Alert>
      </Container>
    );
  }

  const isEmpty = !basket || basket.items.length === 0;

  if (isEmpty) {
    return (
      <Container className='py-5 text-center'>
        <div style={{ fontSize: '4rem' }}><i className="fa-solid fa-cart-shopping"></i></div>
        <h2 className='mt-3'>Your cart is empty</h2>
        <p className='text-muted mb-4'>Looks like you haven't added anything yet.</p>
        <Button
          // eslint-disable-next-line @typescript-eslint/no-explicit-any
          as={Link as any}
          to='/'
          variant='primary'
          size='lg'>
          Browse Products
        </Button>
      </Container>
    );
  }

  const total = basket.items.reduce((sum, item) => sum + item.price * item.quantity, 0);

  return (
    <Container className="py-4">
      <h1 className="mb-4">Your Cart</h1>

      <Row>
        <Col lg={8}>
          <Card className="shadow-sm mb-4">
            <Card.Body className="p-0">
              <Table responsive className="mb-0 align-middle">
                <thead className="table-light">
                  <tr>
                    <th>Product</th>
                    <th className="text-center" style={{ width: '150px' }}>Quantity</th>
                    <th className="text-end">Price</th>
                    <th className="text-end">Total</th>
                    <th></th>
                  </tr>
                </thead>
                <tbody>
                  {basket.items.map((item) => (
                    <tr key={item.productId}>
                      <td>
                        <Link to={`/products/${item.productId}`} className="text-decoration-none fw-medium">
                          {item.productName}
                        </Link>
                      </td>
                      <td>
                        <Form.Control
                          type="number"
                          min={1}
                          value={item.quantity}
                          onChange={(e) => handleQuantityChange(item.productId, parseInt(e.target.value) || 1)}
                          disabled={updateBasket.isPending}
                        />
                      </td>
                      <td className="text-end">${item.price.toFixed(2)}</td>
                      <td className="text-end fw-bold">${(item.price * item.quantity).toFixed(2)}</td>
                      <td className="text-end">
                        <Button
                          variant="outline-danger"
                          size="sm"
                          onClick={() => handleRemoveItem(item.productId)}
                          disabled={updateBasket.isPending || clearBasket.isPending}
                        >
                          Remove
                        </Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </Table>
            </Card.Body>
          </Card>

          <div className="d-flex justify-content-between">
            <Button
              // eslint-disable-next-line @typescript-eslint/no-explicit-any
              as={Link as any}
              to="/"
              variant="outline-secondary">
              ← Continue Shopping
            </Button>
            <Button
              variant="outline-danger"
              onClick={handleClearCart}
              disabled={clearBasket.isPending}
            >
              Clear Cart
            </Button>
          </div>
        </Col>

        <Col lg={4}>
          <Card className="shadow-sm">
            <Card.Body>
              <h5 className="mb-3">Order Summary</h5>

              <div className="d-flex justify-content-between mb-2">
                <span>Subtotal</span>
                <span>${total.toFixed(2)}</span>
              </div>
              <div className="d-flex justify-content-between mb-2">
                <span>Shipping</span>
                <span className="text-success">Free</span>
              </div>
              <hr />
              <div className="d-flex justify-content-between mb-3">
                <strong>Total</strong>
                <strong className="text-primary fs-5">${total.toFixed(2)}</strong>
              </div>

              <Button
                variant="primary"
                size="lg"
                className="w-100"
                onClick={() => navigate('/checkout')}
              >
                Proceed to Checkout
              </Button>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
};