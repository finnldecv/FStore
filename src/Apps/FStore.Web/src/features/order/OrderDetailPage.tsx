import { useParams, useNavigate } from "react-router-dom";
import { Container, Card, Row, Col, Table, Badge, Spinner, Alert, Button } from "react-bootstrap";
import { useQuery } from "@tanstack/react-query";
import { getOrderById } from "../../api/order";

export const OrderDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const { data: order, isLoading, error } = useQuery({
    queryKey: ['order', id],
    queryFn: () => getOrderById(id!),
    enabled: !!id,
  })

  if (isLoading) {
    return (
      <Container className="py-5 text-center">
        <Spinner animation="border" />
      </Container>
    );
  }

  if (error || !order) {
    return (
      <Container className="py-5 text-center">
        <Alert variant="danger">
          Order not found.
        </Alert>
      </Container>
    );
  }

  return (
    <Container>
      <Button variant="link" className="ps-0 mb-3" onClick={() => navigate("/orders")}>
        <i className="fa-solid fa-arrow-left"></i>Back to Orders
      </Button>

      <div className="d-flex justify-content-between align-item-center mb-4">
        <h1>Order Details</h1>
        <Badge bg={order.status === "Cancelled" ? "danger" : "success"} className="fs-6">
          {order.status}
        </Badge>
      </div>

      <Row>
        <Col lg={8}>
          <Card className="shadow-sm mb-4">
            <Card.Body className="p-0">
              <Table responsive className="mb-0">
                <thead className="table-light">
                  <tr>
                    <th>Product</th>
                    <th className="text-center">Qty</th>
                    <th className="text-end">Price</th>
                    <th className="text-end">Total</th>
                  </tr>
                </thead>
                <tbody>
                  {order.items.map((item) => (
                    <tr key={item.productId}>
                      <td>{item.productName}</td>
                      <td className="text-center"></td>
                      <td className="text-end"></td>
                      <td className="text-end fw-bold">${item.lineTotal.toFixed(2)}</td>
                    </tr>
                  ))}
                </tbody>
              </Table>
            </Card.Body>
          </Card>
        </Col>

        <Col lg={4}>
          <Card className="shadow-sm">
            <Card.Body>
              <h5 className="mb-3">Order Summary</h5>
              <div className="d-flex justify-content-between mb-2">
                <span>Order ID</span>
                <span className="font-monospace text-muted" style={{ fontSize: "0.85rem" }}>
                  {order.id.split('-')[0]}
                </span>
              </div>
              <div className="d-flex justify-content-between mb-2">
                <span>Date</span>
                <span>{new Date(order.createdAt).toLocaleDateString()}</span>
              </div>
              <div className="d-flex justify-content-between mb-2">
                <span>Shipping Address</span>
                <span className="text-end ms-3">{order.shippingAddress}</span>
              </div>
              <hr />
              <div className="d-flex justify-content-between">
                <strong>Total</strong>
                <strong className="text-primary fs-5">${order.totalAmount.toFixed(2)}</strong>
              </div>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
};