import { useProducts } from '../hooks/useProducts';

export const ProductList = () => {
  const { data, isLoading, error } = useProducts();
  if (isLoading) return <p>Loading...</p>;
  if (error) return <p>Error loading products</p>;
  return (
    <ul>
      {data?.map(p => (
        <li key={p.id}>{p.name} - ${p.price}</li>
      ))}
    </ul>
  );
};