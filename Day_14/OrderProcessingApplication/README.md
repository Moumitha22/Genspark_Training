# SOLID Principles

**S - Single Responsibility Principle (SRP)**
    A class should have only one reason to change, meaning it should have one responsibility.

**O - Open/Closed Principle (OCP)**
    Software entities should be open for extension, but closed for modification, allowing new functionality without altering existing code.

**L - Liskov Substitution Principle (LSP)**
    Objects of a superclass should be replaceable with subclass objects without affecting program correctness.

**I - Interface Segregation Principle (ISP)**
    Clients should not be forced to implement interfaces they don’t use. Keep interfaces small and focused.

**D - Dependency Inversion Principle (DIP)**
    High-level modules should not depend on low-level ones. Both should depend on abstractions, reducing tight coupling.

## Implementation of SOLID Principles

**S - Single Responsibility Principle (SRP)**

* Each class has a single responsibility:

  * `OrderService` handles order-related operations like placing an order, retrieving orders, etc.
  * `PaymentProcessor` and its subclasses(`CreditCardPaymentProcessor`, `GooglePayPaymentProcessor`) handle different payment processing methods.
  * `EmailNotifier` is responsible for sending notifications.
  * `ManageOrders` manages the user interface for interacting with the system.

**O - Open/Closed Principle (OCP)**

* The system is open for extension but closed for modification:

  * New payment processors (e.g., `CreditCardPaymentProcessor`, `GooglePayPaymentProcessor`) are added without modifying existing classes like `OrderService` or `ManageOrders`.
  * If a new payment method is added, only a new class implementing `IPaymentProcessor` is needed, keeping the existing code unchanged.

**L - Liskov Substitution Principle (LSP)**

* The subclasses `CreditCardPaymentProcessor` and `GooglePayPaymentProcessor` can replace the base class `PaymentProcessor` without affecting the functionality:

  * Both subclasses override the `ProcessPayment` method, and they can be used wherever the `PaymentProcessor` is expected, ensuring the system’s correctness.

**I - Interface Segregation Principle (ISP)**

* Interfaces are designed to be client-specific:

  * `IOrderService`, `IPaymentProcessor`, and `INotifier` define small, focused contracts that only expose the necessary methods for their specific responsibilities.
  * `IOrderService` does not include payment processing, `IPaymentProcessor` does not include order management, and `INotifier` does not deal with payments.

**D - Dependency Inversion Principle (DIP)**

* High-level modules depend on abstractions, not on low-level modules:

  * `OrderService` depends on `IRepository<int, Order>`, `IPaymentProcessor`, and `INotifier`, not on concrete implementations like `OrderRepository`, `PaymentProcessor`, or `EmailNotifier`.
  * This allows for flexibility in substituting different repository, payment, or notification implementations.