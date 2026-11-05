# 🛒 FStore — Cloud-Native E-Commerce Platform

A production-grade, event-driven microservices e-commerce platform built with **.NET 10**, **React 19**, **Docker**, and **RabbitMQ**. 

FStore demonstrates enterprise-level architectural patterns including Clean Architecture, CQRS, Event-Driven Sagas, and Role-Based Access Control (RBAC).

## 🏗 Architecture

FStore is built using a **Choreography-based Saga** pattern. Services are completely decoupled, own their databases, and communicate asynchronously via a message bus to maintain eventual consistency.

```mermaid
graph TD
    UI[React 19 Frontend] -->|HTTP| GW[YARP API Gateway]
    
    GW -->|/catalog| CAT[Catalog API]
    GW -->|/basket| BAS[Basket API]
    GW -->|/order| ORD[Order API]
    GW -->|/identity| ID[Identity API]
    
    CAT -->|SQL Server| DB1[(Catalog DB)]
    BAS -->|Redis| R[(Basket DB)]
    ORD -->|SQL Server| DB2[(Order DB)]
    ID -->|SQL Server| DB3[(Identity DB)]
    
    CAT -.->|Publish| MQ((RabbitMQ))
    BAS -.->|Publish| MQ
    ORD <-->|Consume/Publish| MQ
    PAY[Payment API] <-->|Consume/Publish| MQ
    PAY -->|SQL Server| DB4[(Payment DB)]