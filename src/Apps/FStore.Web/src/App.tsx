import { Container } from 'react-bootstrap';
import { useQuery } from '@tanstack/react-query';
import { getProducts } from './api/catalog';

function App() {
  const { data: products, isLoading, error } = useQuery({
    queryKey: ['products'],
    queryFn: getProducts,
  });

  return (
    <Container className="py-5">
      <h1>FStore</h1>

      {isLoading && <p>Loading products...</p>}
      {error && <p className="text-danger">Error: {error.message}</p>}

      {products && (
        <ul>
          {products.map((p) => (
            <li key={p.id}>
              {p.name} — ${p.price}
            </li>
          ))}
        </ul>
      )}

      {products && products.length === 0 && <p>No products yet. Add some via the API.</p>}
    </Container>
  );
}

export default App;