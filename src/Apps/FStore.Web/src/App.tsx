import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ProductList } from './features/products/components/ProductList';

const queryClient = new QueryClient();

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <h1>FStore</h1>
      <ProductList />
    </QueryClientProvider>
  )
}