import { Routes, Route, Navigate } from 'react-router-dom';
import { Layout } from './shared/Layout';
import { ProductListPage } from './features/product/ProductListPage';
import { ProductDetailPage } from './features/product/ProductDetailPage';
import { CartPage } from './features/cart/CartPage';

function App() {
  return (
    <Layout>
      <Routes>
        <Route path="/" element={<ProductListPage />} />
        <Route path="/product/:id" element={<ProductDetailPage />} />
        <Route path="/cart" element={<CartPage />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </Layout>
  );
}

export default App;