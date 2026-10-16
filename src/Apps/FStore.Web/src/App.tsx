import { Routes, Route, Navigate } from 'react-router-dom';
import { Layout } from './shared/Layout';
import { ProductListPage } from './features/product/ProductListPage';
import { ProductDetailPage } from './features/product/ProductDetailPage';

function App() {
  return (
    <Layout>
      <Routes>
        <Route path="/" element={<ProductListPage />} />
        <Route path="/products/:id" element={<ProductDetailPage />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </Layout>
  );
}

export default App;