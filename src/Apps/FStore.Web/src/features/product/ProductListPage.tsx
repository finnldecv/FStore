import { Container, Alert, Row, Col, Spinner, InputGroup, Form, Pagination } from 'react-bootstrap';
import { useQuery } from '@tanstack/react-query';
import { getProducts } from '../../api/catalog';
import { ProductCard } from './ProductCard';
import { useState } from 'react';
import { useDebounce } from 'use-debounce';

export const ProductListPage = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [debouncedSearch] = useDebounce(searchTerm, 300);

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(e.target.value)
    setCurrentPage(1);
  }
  const { data, isLoading, error } = useQuery({
    queryKey: ['products', debouncedSearch, currentPage],
    queryFn: () => getProducts(debouncedSearch, currentPage, 12),
  });

  const paginationItems = [];
  if (data) {
    for (let number = 1; number <= data.totalPages; number++)
      paginationItems.push(
        <Pagination.Item
          key={number}
          active={number === currentPage}
          onClick={() => {
            setCurrentPage(number);
            window.scrollTo(0, 0);
          }}
        >
          {number}
        </Pagination.Item>
      );
  }
  return (
    <Container className='py-4'>
      <div className='d-flex justify-content-between align-items-center mb-4'>
        <h1>Products</h1>
        {data && <span className="text-muted">{data.totalCount} items found</span>}
        <InputGroup style={{ maxWidth: '400px' }}>
          <Form.Control
            type='text'
            placeholder='Search products...'
            value={searchTerm}
            onChange={() => handleSearchChange} />
        </InputGroup>
        <span className='text-muted'>
          {data?.items.length ?? 0} {data?.items.length === 1 ? 'item' : 'items'}
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

      {data?.items && data?.items.length === 0 && (
        <Alert variant="info">
          No products yet. Add some via the API and refresh.
        </Alert>
      )}

      {data?.items && data?.items.length > 0 && (
        <>
          <Row xs={1} md={2} lg={3} xl={4} className='g-4'>
            {data?.items.map((product) => (
              <Col key={product.id}>
                <ProductCard product={product} />
              </Col>
            ))}
          </Row>

          {data.totalPages > 1 && (
            <div className='d-flex justify-content-center'>
              <Pagination>
                <Pagination.Prev
                  disabled={!data?.hasPreviousPage}
                  onClick={() => {
                    setCurrentPage(currentPage - 1);
                    window.scrollTo(0, 0);
                  }}
                />
                {paginationItems}
                <Pagination.Next
                  disabled={!data?.hasNextPage}
                  onClick={() => {
                    setCurrentPage(currentPage + 1);
                    window.scrollTo(0, 0);
                  }} />
              </Pagination>
            </div>
          )}
        </>
      )}
    </Container>
  );
};