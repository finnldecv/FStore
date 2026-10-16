import { useState } from "react";
import { useParams, Link, useNavigate } from 'react-router-dom';
import { Container, Row, Col, Card, Button, Form, Alert, Spinner, Badge } from 'react-bootstrap';
import { useQuery } from "@tanstack/react-query";
import { getCategoryById, getProductById } from "../../api/catalog";
import { useAddToBasket } from "../cart/hooks";

export const ProductDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [quantity, setQuantity] = useState(1);
  const [added, setAdded] = useState(false);
  const addToBasket = useAddToBasket();

  const { data: product, isLoading, error } = useQuery({
    queryKey: ['product', id],
    queryFn: () => getProductById(id!),
    enabled: !!id
  });

  const { data: category } = useQuery({
    queryKey: ['category', product?.categoryId],
    queryFn: () => getCategoryById(product!.categoryId),
    enabled: !!product?.categoryId,
  });

  const handleAddToCard = () => {
    if (!product) return;

    addToBasket.mutate(
      {
        productId: product.id,
        productName: product.name,
        price: product.price,
        quantity,
      },
      {
        onSuccess: () => {
          setAdded(true);
          setTimeout(() => setAdded(false), 2000)
        },
      }
    );
  };

  if (isLoading) {
    return (
      <Container className="py-5 text-center">
        <Spinner animation="border" />
      </Container>
    );
  }

  if (error || !product) {
    return (
      <Container className="py-5">
        <Alert variant="danger">
          Product not found. <Link to="/">Back to products</Link>
        </Alert>
      </Container>
    );
  }

  const inStock = product.stockQuantity > 0;

  return (
    <Container className="py-4">
      <Button
        variant="link"
        className="ps-0 mb-3"
        onClick={() => navigate(-1)}
      >
        <i className="fa-solid fa-arrow-left"></i>Back
      </Button>

      <Row>
        <Col md={6}>
          <Card className="shadow-sm">
            <Card.Body className="text-center p-5">
              <div style={{ fontSize: '8rem' }}><i className="fa-solid fa-cube"></i></div>
            </Card.Body>
          </Card>
        </Col>

        <Col md={6}>
          <Badge bg="light" text="dark" className="mb-2">
            {category?.name}
          </Badge>

          <h1>{product.name}</h1>

          <p className="text-muted">{product.description}</p>

          <h2 className="text-primary">${product.price.toFixed(2)}</h2>

          <p className={inStock ? 'text-success' : 'text-danger'}>
            {inStock ? `In sotck: ${product.stockQuantity}` : 'Out of stock'}
          </p>

          {added && (
            <Alert variant="success" className="py-2">Added to card</Alert>
          )}

          <Form.Group className="mb-3" style={{ maxWidth: '150px' }}>
            <Form.Label>Quantity</Form.Label>
            <Form.Control type="number" min={1} max={product.stockQuantity} value={quantity}
              onChange={(e) => setQuantity(Math.max(1, parseInt(e.target.value) || 1))} disabled={!inStock}
            />
          </Form.Group>

          <Button variant="primary" size="lg" onClick={handleAddToCard} disabled={!inStock || addToBasket.isPending}>
            {addToBasket.isPending ? 'Adding...' : 'Add to Card'}
          </Button>

          <Button
            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            as={Link as any}
            to="/cart"
            variant="outline-primary"
            size="lg"
            className="ms-2"
          >
            View Cart
          </Button>
        </Col>
      </Row>
    </Container>
  );
}