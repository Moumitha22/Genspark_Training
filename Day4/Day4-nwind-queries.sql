USE Northwind;

-- 1) List all orders with the customer name and the employee who handled the order.
-- (Join Orders, Customers, and Employees)

SELECT OrderID, CompanyName AS Customer_Name, CONCAT(FirstName,' ',LastName) Employee_Name
FROM Orders o 
JOIN Customers c ON o.CustomerID = c.CustomerID
JOIN Employees e ON o.EmployeeID = e.EmployeeID

-- OR
SELECT o.*,CompanyName AS Customer_Name, CONCAT(FirstName,' ',LastName) Employee_Name 
FROM Orders o 
JOIN Customers c ON o.CustomerID = c.CustomerID
JOIN Employees e ON o.EmployeeID = e.EmployeeID

-- 2) Get a list of products along with their category and supplier name.
-- (Join Products, Categories, and Suppliers)

SELECT ProductName, CategoryName, CompanyName AS SupplierName 
FROM Products p
JOIN Categories c ON p.CategoryID = c.CategoryID
JOIN Suppliers s ON p.SupplierID = s.SupplierID

-- 3) Show all orders and the products included in each order with quantity and unit price.
-- (Join Orders, Order Details, Products)

SELECT od.OrderID, ProductName, Quantity, od.UnitPrice 
FROM [Order Details] od 
JOIN Orders o ON od.OrderID = o.OrderID
JOIN Products p ON od.ProductID = p.ProductID

-- 4) List employees who report to other employees (manager-subordinate relationship).
-- (Self join on Employees)

SELECT CONCAT(e1.FirstName,' ',e1.LastName) Employee_Name, CONCAT(e2.FirstName,' ',e2.LastName) Manager
FROM Employees e1
JOIN Employees e2 ON e1.ReportsTo = e2.EmployeeID

-- 5) Display each customer and their total order count.
-- (Join Customers and Orders, then GROUP BY)

SELECT CompanyName AS Customer_Name, COUNT(*) Order_Count 
FROM Customers c 
JOIN Orders o ON c.CustomerID = o.CustomerID
GROUP BY CompanyName

-- 6) Find the average unit price of products per category.
-- Use AVG() with GROUP BY

SELECT CategoryName, AVG(UnitPrice) Average_Unit_Price
FROM Products p
JOIN Categories c ON p.CategoryID = c.CategoryID
GROUP BY CategoryName

-- 7) List customers where the contact title starts with 'Owner'.
-- Use LIKE or LEFT(ContactTitle, 5)

SELECT * FROM Customers 
WHERE ContactTitle LIKE 'Owner%'

-- 8) Show the top 5 most expensive products.
-- Use ORDER BY UnitPrice DESC and TOP 5

SELECT TOP 5 ProductName, UnitPrice FROM Products
ORDER BY UnitPrice DESC

-- 9) Return the total sales amount (quantity × unit price) per order.
-- Use SUM(OrderDetails.Quantity * OrderDetails.UnitPrice) and GROUP BY

SELECT o.OrderID, SUM(od.Quantity * od.UnitPrice) Total_Sales_Amount FROM Orders o
JOIN [Order Details] od ON o.OrderID = od.OrderID
GROUP BY o.OrderID 

-- 10) Create a stored procedure that returns all orders for a given customer ID.
-- Input: @CustomerID

-- Checking data type of CustomerId
sp_help 'Orders';

-- or
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Orders'

CREATE OR ALTER PROC proc_GetCustomerOrders(@pcustomerId NCHAR(5))
AS
BEGIN
	SELECT * FROM Orders
	WHERE CustomerID = @pcustomerId
END

GO
proc_GetCustomerOrders 'VINET'

-- 11) Write a stored procedure that inserts a new product.
-- Inputs: ProductName, SupplierID, CategoryID, UnitPrice, etc.

CREATE PROC proc_InsertProduct(@pname NVARCHAR(40), @psupplierId INT, @pcategoryId INT, @punitPrice MONEY)
AS
BEGIN
	INSERT INTO Products (ProductName, SupplierId,CategoryId, UnitPrice)
	VALUES(@pname, @psupplierId, @pcategoryId, @punitPrice)
END

GO
proc_InsertProduct 'Coffee', 1, 1, 25.00

SELECT * FROM Products

-- 12) Create a stored procedure that returns total sales per employee.
-- Join Orders, Order Details, and Employees

CREATE OR ALTER PROC proc_GetEmployeeTotalSales
AS
BEGIN
	SELECT CONCAT(e.FirstName, ' ', e.LastName) AS Employee_Name, SUM(od.Quantity * od.UnitPrice) Total_Sales_Amount
	FROM Employees e
	JOIN Orders o ON e.EmployeeID = o.EmployeeID
	JOIN [Order Details] od ON o.OrderID = od.OrderID
	GROUP BY CONCAT(e.FirstName, ' ', e.LastName)
	ORDER BY Total_Sales_Amount DESC
END

GO 
proc_GetEmployeeTotalSales

-- 13) Use a CTE to rank products by unit price within each category.
-- Use ROW_NUMBER() or RANK() with PARTITION BY CategoryID

WITH cte_RankCategory AS
(
	SELECT ProductName, UnitPrice, RANK() OVER(PARTITION BY CategoryID ORDER BY UnitPrice DESC) AS Rank 
	FROM Products
)
SELECT * FROM cte_RankCategory

-- 14) Create a CTE to calculate total revenue per product and filter products with revenue > 10,000.

WITH cte_CalculateTotalRevenue AS
(
	SELECT p.ProductID, SUM(od.Quantity * od.UnitPrice) Total_Revenue 
	FROM Products p
	JOIN [Order Details] od ON p.ProductID = od.ProductID
	GROUP BY p.ProductID
	HAVING SUM(od.Quantity * od.UnitPrice) > 10000
)
SELECT * FROM cte_CalculateTotalRevenue

-- 15) Use a CTE with recursion to display employee hierarchy.
-- Start from top-level employee (ReportsTo IS NULL) and drill down

WITH cte_EmployeeHierarchy AS
(
	SELECT EmployeeID, CONCAT(FirstName,' ',LastName) Employee_Name, ReportsTo, 1 AS Level
	FROM Employees 
	WHERE ReportsTo IS NULL

	UNION ALL

	SELECT e.EmployeeID, CONCAT(e.FirstName,' ',e.LastName) Employee_Name, e.ReportsTo, eh.Level + 1
	FROM Employees e
	INNER JOIN cte_EmployeeHierarchy eh ON e.ReportsTo = eh.EmployeeID
)
SELECT * FROM cte_EmployeeHierarchy
