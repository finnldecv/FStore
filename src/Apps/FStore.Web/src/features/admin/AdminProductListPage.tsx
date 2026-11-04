import { useState } from "react";
import { Container, Table, Button, Badge, Modal } from "react-bootstrap";
import { Link } from "react-router-dom";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getCategories, getProducts } from "../../api/catalog";
import { http } from "../../api/client";

export const AdminProductListPage = () => {
  const queryClient = useQueryClient();
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [productToDelete, setProductToDelete] = useState<string | null>(null);

  const { data, isLoading } = useQuery({
    queryKey: ['admin-products'],
    queryFn: () => getProducts(undefined, undefined, 0, 9999, 1, 100)
  });

  const { data: categories } = useQuery({
    queryKey: ['categories'],
    queryFn: () => getCategories(),
  });

  const getCategoryName = (categoryId: string) => {
    const category = categories?.find(c => c.id === categoryId)
    return category?.name || 'Unknown'
  }
  const deleteMutation = useMutation({
    mutationFn: (id: string) => http.delete(`/catalog/api/products/${id}`),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-products'] });
      queryClient.invalidateQueries({ queryKey: ['products'] });
      setShowDeleteModal(false);
    }
  });

  const confirmDelete = () => {
    if (productToDelete) deleteMutation.mutate(productToDelete);
  };

  if (isLoading) return <Container className="py-5">Loading...</Container>

  return (
    <Container className="py-4">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h1>Manage Products</h1>
        <Link to="/admin/products/create" className="btn btn-success">
          <i className="fa-solid fa-plus"></i> Add New Product
        </Link>
      </div>

      <Table striped bordered hover responsive>
        <thead>
          <tr>
            <th>Name</th>
            <th>Category</th>
            <th>Price</th>
            <th>Stock</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {data?.items.map((product) => (
            <tr key={product.id}>
              <td>{product.name}</td>
              <td>{getCategoryName(product.categoryId)}</td>
              <td>${product.price.toFixed(2)}</td>
              <td>
                {product.stockQuantity > 0 ? (
                  <Badge bg="success">{product.stockQuantity}</Badge>
                ) : (
                  <Badge bg="danger">Out of Stock</Badge>
                )}
              </td>
              <td>
                <Link to={`/admin/products/edit/${product.id}`} className="btn btn-sm btn-outline-primary me-2">
                  Edit
                </Link>
                <Button
                  size="sm"
                  variant="outline-danger"
                  onClick={() => {
                    setProductToDelete(product.id)
                    setShowDeleteModal(true)
                  }}
                >
                  Delete
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
      <Modal show={showDeleteModal} onHide={() => setShowDeleteModal(false)}>
        <Modal.Header closeButton>
          <Modal.Title>Confirm Delete</Modal.Title>
        </Modal.Header>
        <Modal.Body>Are you sure you want to delete this product? This action cannot be undone.</Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowDeleteModal(false)}>Cancel</Button>
          <Button variant="danger" onClick={confirmDelete} disabled={deleteMutation.isPending}>
            {deleteMutation.isPending ? 'Deleting...' : 'Delete'}
          </Button>
        </Modal.Footer>
      </Modal>
    </Container>
  );
};