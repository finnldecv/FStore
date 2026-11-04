import { useState } from 'react';
import { Container, Form, Button, Card, Row, Col, Alert, Spinner } from 'react-bootstrap';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getCategories, getProductById } from '../../api/catalog';
import { http } from '../../api/client';

export const ProductFormPage = () => {
  const { id } = useParams<{ id: string }>();
  const isEditMode = !!id;
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const { data: existingProduct, isLoading: isLoadingProduct } = useQuery({
    queryKey: ['product', id],
    queryFn: () => getProductById(id!),
    enabled: isEditMode,
  });

  const [formData, setFormData] = useState(() => {
    if (existingProduct) {
      return {
        name: existingProduct.name,
        description: existingProduct.description,
        price: existingProduct.price,
        stockQuantity: existingProduct.stockQuantity,
        categoryId: existingProduct.categoryId,
      };
    }
    return {
      name: '',
      description: '',
      price: 0,
      stockQuantity: 0,
      categoryId: '',
    };
  });
  const [errorMsg, setErrorMsg] = useState<string | null>(null);

  const { data: categories } = useQuery({
    queryKey: ['categories'],
    queryFn: getCategories,
  });

  const mutation = useMutation({
    mutationFn: async () => {
      const payload = {
        name: formData.name,
        description: formData.description,
        price: Number(formData.price),
        stockQuantity: Number(formData.stockQuantity),
        categoryId: formData.categoryId,
      };

      if (isEditMode) {
        await http.put(`/catalog/api/products/${id}`, { id, ...payload });
      } else {
        await http.post('/catalog/api/products', payload);
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-products'] });
      queryClient.invalidateQueries({ queryKey: ['products'] });
      navigate('/admin/products');
    },
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    onError: (err: any) => {
      setErrorMsg(err.response?.data?.error || 'Failed to save product.');
    },
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setErrorMsg(null);
    mutation.mutate();
  };

  if (isEditMode && isLoadingProduct) {
    return <Container className="py-5 text-center"><Spinner animation="border" /></Container>;
  }

  return (
    <Container className="py-4" style={{ maxWidth: '800px' }}>
      <Button
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        as={Link as any}
        to="/admin/products"
        variant="link"
        className="ps-0 mb-3"
      >
        ← Back to Products
      </Button>

      <Card className="shadow-sm">
        <Card.Body className="p-4">
          <h2 className="mb-4">{isEditMode ? 'Edit Product' : 'Create New Product'}</h2>

          {errorMsg && <Alert variant="danger">{errorMsg}</Alert>}

          <Form onSubmit={handleSubmit}>
            <Row>
              <Col md={8}>
                <Form.Group className="mb-3">
                  <Form.Label>Product Name</Form.Label>
                  <Form.Control
                    type="text"
                    required
                    value={formData.name}
                    onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                  />
                </Form.Group>
              </Col>
              <Col md={4}>
                <Form.Group className="mb-3">
                  <Form.Label>Category</Form.Label>
                  <Form.Select
                    required
                    value={formData.categoryId}
                    onChange={(e) => setFormData({ ...formData, categoryId: e.target.value })}
                  >
                    <option value="">Select a category...</option>
                    {categories?.map((cat) => (
                      <option key={cat.id} value={cat.id}>{cat.name}</option>
                    ))}
                  </Form.Select>
                </Form.Group>
              </Col>
            </Row>

            <Form.Group className="mb-3">
              <Form.Label>Description</Form.Label>
              <Form.Control
                as="textarea"
                rows={3}
                required
                value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
              />
            </Form.Group>

            <Row>
              <Col md={6}>
                <Form.Group className="mb-4">
                  <Form.Label>Price ($)</Form.Label>
                  <Form.Control
                    type="number"
                    step="0.01"
                    min="0"
                    required
                    value={formData.price}
                    onChange={(e) => setFormData({ ...formData, price: parseFloat(e.target.value) || 0 })}
                  />
                </Form.Group>
              </Col>
              <Col md={6}>
                <Form.Group className="mb-4">
                  <Form.Label>Stock Quantity</Form.Label>
                  <Form.Control
                    type="number"
                    min="0"
                    required
                    value={formData.stockQuantity}
                    onChange={(e) => setFormData({ ...formData, stockQuantity: parseInt(e.target.value) || 0 })}
                  />
                </Form.Group>
              </Col>
            </Row>

            <div className="d-flex justify-content-end gap-2">
              <Button
                // eslint-disable-next-line @typescript-eslint/no-explicit-any
                as={Link as any}
                to="/admin/products"
                variant="secondary"
                disabled={mutation.isPending}
              >
                Cancel
              </Button>
              <Button type="submit" variant="primary" disabled={mutation.isPending}>
                {mutation.isPending ? 'Saving...' : isEditMode ? 'Save Changes' : 'Create Product'}
              </Button>
            </div>
          </Form>
        </Card.Body>
      </Card>
    </Container>
  );
};