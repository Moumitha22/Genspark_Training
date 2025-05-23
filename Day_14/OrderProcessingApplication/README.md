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
  * `EmailNotifier` and `SmsNotifier` are responsible for sending notifications.
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

  * `IEmailNotifier` and `ISmsNotifier` define small, focused interfaces tailored to different types of notifications.
    * `IEmailNotifier` includes methods specifically for sending email notifications, while  `ISmsNotifier` is responsible only for SMS-based notifications.

  * Similarly, `IOrderService` and `IPaymentProcessor` are focused on distinct responsibilities:
    * `IOrderService` handles order-related operations, while `IPaymentProcessor` is concerned solely with processing payments.

**D - Dependency Inversion Principle (DIP)**

* High-level modules depend on abstractions, not on low-level modules:

  * `OrderService` depends on `IRepository<int, Order>`, `IPaymentProcessor`, and `IEmailNotifier`, not on concrete implementations like `OrderRepository`, `PaymentProcessor`, or `EmailNotifier`.
  * This allows for flexibility in substituting different repository, payment, or notification implementations.