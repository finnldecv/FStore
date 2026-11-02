import { Container, Alert, Row, Col, Spinner, InputGroup, Form, Pagination } from 'react-bootstrap';
import { useQuery } from '@tanstack/react-query';
import { getCategories, getProducts } from '../../api/catalog';
import { ProductCard } from './ProductCard';
import { useCallback, useState } from 'react';
import { useDebounce } from 'use-debounce';
import { PriceRangeFilter } from './PriceRangeFilter';

export const ProductListPage = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedCategory, setSelectedCategory] = useState('');
  const [priceRange, setPriceRange] = useState<{ min: number, max: number }>({ min: 0, max: 2500 });
  const [currentPage, setCurrentPage] = useState(1);
  const [debouncedSearch] = useDebounce(searchTerm, 300);
  const { data: categories } = useQuery({
    queryKey: ['categories'],
    queryFn: getCategories,
  });

  const { data, isLoading, error } = useQuery({
    queryKey: ['products', debouncedSearch, selectedCategory, priceRange, currentPage],
    queryFn: () => getProducts(debouncedSearch, selectedCategory, priceRange.min, priceRange.max, currentPage, 12),
  });

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(e.target.value)
    setCurrentPage(1);
  };

  const handleCategoryChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setSelectedCategory(e.target.value);
    setCurrentPage(1);
  }

  const handlePriceChange = useCallback((min: number, max: number) => {
    setPriceRange({ min, max });
    setCurrentPage(1);
  }, []);

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
    <Container className="py-4">
      {/* ─── Top Header ─── */}
      <div className="d-flex flex-column flex-md-row justify-content-between align-items-md-center mb-4 gap-3">
        <div>
          <h1 className="mb-0">Products</h1>
          {data && (
            <span className="text-muted">
              {data.totalCount} {data.totalCount === 1 ? 'item' : 'items'} found.
            </span>
          )}
        </div>

        <InputGroup style={{ maxWidth: '400px' }}>
          <InputGroup.Text>
            <i className="fa-solid fa-magnifying-glass"></i>
          </InputGroup.Text>
          <Form.Control
            type="text"
            placeholder="Search products..."
            value={searchTerm}
            onChange={handleSearchChange}
          />
        </InputGroup>
      </div>

      <Row>
        {/* ─── Left Sidebar (Filters) ─── */}
        <Col lg={3} className="mb-4">
          <div className="bg-white p-3 rounded shadow-sm mb-4">
            <h5 className="mb-3">Price Range</h5>
            <PriceRangeFilter
              min={0}
              max={2500}
              onChange={handlePriceChange}
            />
          </div>

          <div className="bg-white p-3 rounded shadow-sm">
            <h5 className="mb-3">Categories</h5>
            <Form.Select
              value={selectedCategory}
              onChange={handleCategoryChange}
            >
              <option value="">All Categories</option>
              {categories?.map((cat) => (
                <option key={cat.id} value={cat.id}>
                  {cat.name}
                </option>
              ))}
            </Form.Select>
          </div>
        </Col>

        {/* ─── Right Main Area (Products) ─── */}
        <Col lg={9}>
          {isLoading && (
            <div className="text-center py-5">
              <Spinner animation="border" role="status" />
              <p className="mt-2 text-muted">Loading products...</p>
            </div>
          )}

          {error && (
            <Alert variant="danger">
              Failed to load products: {(error as Error).message}
            </Alert>
          )}

          {data?.items && data.items.length === 0 && (
            <Alert variant="info">
              No products found matching your criteria.
            </Alert>
          )}

          {data?.items && data.items.length > 0 && (
            <>
              <Row xs={1} md={2} xl={3} className="g-4">
                {data.items.map((product) => (
                  <Col key={product.id}>
                    <ProductCard product={product} />
                  </Col>
                ))}
              </Row>

              {/* ─── Pagination ─── */}
              {data.totalPages > 1 && (
                <div className="d-flex justify-content-center mt-5">
                  <Pagination>
                    <Pagination.Prev
                      disabled={!data.hasPreviousPage}
                      onClick={() => {
                        setCurrentPage(currentPage - 1);
                        window.scrollTo(0, 0);
                      }}
                    />
                    {paginationItems}
                    <Pagination.Next
                      disabled={!data.hasNextPage}
                      onClick={() => {
                        setCurrentPage(currentPage + 1);
                        window.scrollTo(0, 0);
                      }}
                    />
                  </Pagination>
                </div>
              )}
            </>
          )}
        </Col>
      </Row>
    </Container>
  );
}