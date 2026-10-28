import { Card, Button, Badge } from 'react-bootstrap';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import type { Product } from '../../api/catalog';
import { useAddToBasket } from '../cart/hooks';
import { getCategoryById } from '../../api/catalog';

interface ProductCardProps {
  product: Product
}

export const ProductCard = ({ product }: ProductCardProps) => {
  const addToBasket = useAddToBasket();

  const handleAddToCard = () => {
    addToBasket.mutate({
      productId: product.id,
      productName: product.name,
      price: product.price,
      quantity: 1,
    })
  };

  const { data: category, isLoading } = useQuery({
    queryKey: ['category', product.categoryId],
    queryFn: () => getCategoryById(product.categoryId),
    enabled: !!product.categoryId
  });

  const inStock = product.stockQuantity > 0;

  return (
    <Card className='h-100 shadow-sm'>
      <Card.Body className='d-flex flex-column'>
        <div className='d-flex justify-content-between align-items-start mb-2'>
          <Card.Title className='mb-0'>{product.name}</Card.Title>
          {!inStock && <Badge bg='secondary'>Out of stock</Badge>}
        </div>

        <Badge bg='light' text='dark' className='align-seft-start mb-2'>
          {isLoading ? 'Loading...' : category?.name}
        </Badge>

        <Card.Text className="text-muted flex-grow-1">
          {product.description}
        </Card.Text>

        <div className='d-flex justify-content-between align-items-center mt-3'>
          <span className='fs-5 fw-bold'>
            ${product.price.toFixed(2)}
          </span>
          <div>
            <Button
              // eslint-disable-next-line @typescript-eslint/no-explicit-any
              as={Link as any}
              to={`/products/${product.id}`}
              variant='outline-secondary'
              size='sm'
              className='me-2'
            >
              Details
            </Button>
            <Button
              variant='primary'
              size='sm'
              onClick={handleAddToCard}
              disabled={!inStock || addToBasket.isPending}
            >
              {addToBasket.isPending ? "Adding..." : "Add to Cart"}
            </Button>
          </div>
        </div>
      </Card.Body>
    </Card>
  );
};