import { useState } from 'react';
import { Container, Card, Form, Button, Alert } from 'react-bootstrap';
import { Link, useNavigate } from 'react-router-dom';
import { http } from '../../api/client';

export const RegisterPage = () => {
  const [formData, setFormData] = useState({ email: '', password: '', firstName: '', lastName: '' });
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    try {
      await http.post('/identity/api/auth/register', formData);
      navigate('/login');
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } catch (err: any) {
      setError(err.response?.data?.error || 'Registration failed');
    }
  };

  return (
    <Container className="py-5 d-flex justify-content-center">
      <Card className="shadow-sm" style={{ width: '450px' }}>
        <Card.Body className="p-4">
          <h2 className="text-center mb-4">Register</h2>
          {error && <Alert variant="danger">{error}</Alert>}
          <Form onSubmit={handleSubmit}>
            <div className="d-flex gap-2 mb-3">
              <Form.Group className="w-50">
                <Form.Label>First Name</Form.Label>
                <Form.Control type="text" value={formData.firstName} onChange={(e) => setFormData({ ...formData, firstName: e.target.value })} required />
              </Form.Group>
              <Form.Group className="w-50">
                <Form.Label>Last Name</Form.Label>
                <Form.Control type="text" value={formData.lastName} onChange={(e) => setFormData({ ...formData, lastName: e.target.value })} required />
              </Form.Group>
            </div>
            <Form.Group className="mb-3">
              <Form.Label>Email</Form.Label>
              <Form.Control type="email" value={formData.email} onChange={(e) => setFormData({ ...formData, email: e.target.value })} required />
            </Form.Group>
            <Form.Group className="mb-4">
              <Form.Label>Password</Form.Label>
              <Form.Control type="password" value={formData.password} onChange={(e) => setFormData({ ...formData, password: e.target.value })} required />
            </Form.Group>
            <Button type="submit" variant="primary" className="w-100 mb-3">Register</Button>
            <div className="text-center">
              <small>Already have an account? <Link to="/login">Login</Link></small>
            </div>
          </Form>
        </Card.Body>
      </Card>
    </Container>
  );
};