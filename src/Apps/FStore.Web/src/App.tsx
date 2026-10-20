import { Routes, Route, Navigate } from 'react-router-dom';
import { Layout } from './shared/Layout';
import { ProductListPage } from './features/product/ProductListPage';
import { ProductDetailPage } from './features/product/ProductDetailPage';
import { CartPage } from './features/cart/CartPage';
import { CheckoutPage } from './features/order/CheckoutPage';
import { CheckoutSuccessPage } from './features/order/CheckoutSuccessPage';

function App() {
  return (
    <Layout>
      <Routes>
        <Route path="/" element={<ProductListPage />} />
        <Route path="/product/:id" element={<ProductDetailPage />} />
        <Route path="/cart" element={<CartPage />} />
        <Route path="/checkout" element={<CheckoutPage />} />
        <Route path="/checkout/success" element={<CheckoutSuccessPage />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </Layout>
  );
}

export default App;