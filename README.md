# 🛒 FStore — Cloud-Native E-Commerce Platform

A production-grade, event-driven microservices e-commerce platform built with **.NET 10**, **React 19**, **Docker**, and **RabbitMQ**.

## 🏗 Architecture

FStore is built using a **Choreography-based Saga** pattern. Services are completely decoupled, own their databases, and communicate asynchronously via a message bus.

### Backend Services
*   **API Gateway (YARP):** Single entry point, routing, and CORS handling.
*   **Catalog Service:** Manages products and categories (SQL Server).
*   **Basket Service:** Manages user shopping carts (Redis).
*   **Order Service:** Orchestrates the checkout Saga, state machine, and order history (SQL Server).

### Infrastructure & Patterns
*   **Message Broker:** RabbitMQ (MassTransit)
*   **Architecture:** Clean Architecture, CQRS (MediatR), Vertical Slice (Feature folders)
*   **Testing:** xUnit, Testcontainers (real DBs in Docker for integration tests), Respawn, NSubstitute.
*   **DevOps:** Fully containerized (Docker Compose), CI/CD via GitHub Actions.

## 🔄 The Event-Driven Checkout Flow

1. User checks out via the frontend.
2. **Basket API** clears the cart and publishes a `BasketCheckoutEvent`.
3. **Order API** consumes the event, creates a `Pending` Order, and publishes `OrderCreatedEvent`.
4. *(Future)* **Inventory API** will reserve stock and publish success/failure events.

## 🚀 Quick Start (Run Locally)

You only need **Docker Desktop** installed.

```bash
git clone https://github.com/finnldecv/FStore.git
cd FStore
docker compose up -d --build