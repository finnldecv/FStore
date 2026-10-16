import { Container, Alert, Row, Col, Spinner } from 'react-bootstrap';
import { useQuery } from '@tanstack/react-query';
import { getProducts } from '../../api/catalog';
import { ProductCard } from './ProductCard';

export const ProductListPage = () => {
  const { data: products, isLoading, error } = useQuery({
    queryKey: ['product'],
    queryFn: () => getProducts(),
  });

  return (
    <Container className='py-4'>
      <div className='d-flex justify-content-between align-items-center mb-4'>
        <h1>Products</h1>
        <span className='text-muted'>
          {products?.length ?? 0} {products?.length === 1 ? 'item' : 'items'}
        </span>
      </div>

      {isLoading && (
        <div className='text-center py-5'>
          <Spinner animation="border" role="status" />
          <p className='mt-2 text-muted'>Loading products...</p>
        </div>
      )}

      {error && (
        <Alert variant='danger'>
          Failed to load products: {(error as Error).message}
        </Alert>
      )}

      {products && products.length === 0 && (
        <Alert variant="info">
          No products yet. Add some via the API and refresh.
        </Alert>
      )}

      {products && products.length > 0 && (
        <Row xs={1} md={2} lg={3} xl={4} className='g-4'>
          {products.map((product) => (
            <Col key={product.id}>
              <ProductCard product={product} />
            </Col>
          ))}
        </Row>
      )}
    </Container>
  );
};