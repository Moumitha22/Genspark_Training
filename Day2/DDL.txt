CREATE TABLE SupplierStatus(
 supplier_status_id INT PRIMARY KEY,
 status_message VARCHAR(100) NOT NULL,
);

CREATE TABLE CustomerStatus(
 customer_status_id INT PRIMARY KEY,
 status_message VARCHAR(100) NOT NULL,
);

CREATE TABLE OrderStatus(
 order_status_id INT PRIMARY KEY,
 status_message VARCHAR(100) NOT NULL,
);

CREATE TABLE Country(
 country_id INT PRIMARY KEY,
 country_name VARCHAR(100) NOT NULL UNIQUE 
);

CREATE TABLE State(
 state_id INT PRIMARY KEY,
 state_name VARCHAR(100) NOT NULL,
 country_id INT NOT NULL,
 FOREIGN KEY (country_id) REFERENCES Country(country_id)
);

CREATE TABLE City(
 city_id INT PRIMARY KEY,
 city_name VARCHAR(100) NOT NULL,
 state_id INT NOT NULL,
 FOREIGN KEY (state_id) REFERENCES State(state_id)
);

CREATE TABLE Area(
 zipcode VARCHAR(10) PRIMARY KEY,
 area_name VARCHAR(100) NOT NULL,
 city_id INT NOT NULL,
 FOREIGN KEY (city_id) REFERENCES City(city_id)
);

CREATE TABLE Address( 
 address_id INT PRIMARY KEY,
 door_number VARCHAR(10) NOT NULL,
 address_line1 VARCHAR(100) NOT NULL,
 zipcode VARCHAR(10) NOT NULL,
 FOREIGN KEY (zipcode) REFERENCES Area(zipcode)
);

CREATE TABLE Supplier(
 supplier_id INT PRIMARY KEY,
 supplier_name VARCHAR(100) NOT NULL,
 contact_person VARCHAR(100) NOT NULL,
 phone VARCHAR(10) NOT NULL UNIQUE CHECK (LEN(phone) >= 10),
 email VARCHAR(100) NOT NULL UNIQUE,
 address_id INT NOT NULL,
 supplier_status_id INT NOT NULL,
 FOREIGN KEY (address_id) REFERENCES Address(address_id),
 FOREIGN KEY (supplier_status_id) REFERENCES SupplierStatus(supplier_status_id)
);

CREATE TABLE Product(
 product_id INT PRIMARY KEY,
 product_name VARCHAR(100) NOT NULL,
 unit_price DECIMAL(10,2) NOT NULL CHECK (unit_price >= 0),
 quantity INT NOT NULL CHECK (quantity >= 0),
 description TEXT,
 image VARCHAR(255)
)

CREATE TABLE ProductSupplier(
 transaction_id INT PRIMARY KEY,
 product_id INT NOT NULL,
 supplier_id INT NOT NULL,
 date_of_supply DATE NOT NULL,
 quantity INT NOT NULL CHECK (quantity > 0),
 FOREIGN KEY (product_id) REFERENCES Product(product_id),
 FOREIGN KEY (supplier_id) REFERENCES Supplier(supplier_id)
);

CREATE TABLE Customer(
 customer_id INT PRIMARY KEY,
 customer_name VARCHAR(50) NOT NULL,
 phone VARCHAR(10) NOT NULL UNIQUE CHECK (LEN(phone) >= 10),
 email VARCHAR(50) NOT NULL UNIQUE,
 age INT NOT NULL CHECK (age >= 18),
 address_id INT NOT NULL,
 customer_status_id INT NOT NULL,
 FOREIGN KEY (address_id) REFERENCES Address(address_id),
 FOREIGN KEY (customer_status_id) REFERENCES CustomerStatus(customer_status_id)
);

CREATE TABLE Orders(
 order_number INT PRIMARY KEY,
 customer_id INT NOT NULL,
 order_date DATE NOT NULL,
 amount DECIMAL(10,2) NOT NULL CHECK (amount >= 0),
 order_status_id INT NOT NULL,
 FOREIGN KEY (customer_id) REFERENCES Customer(customer_id),
 FOREIGN KEY (order_status_id) REFERENCES OrderStatus(order_status_id)
);

CREATE TABLE OrderDetails(
 order_details_id INT PRIMARY KEY,
 order_number INT NOT NULL,
 product_id INT NOT NULL,
 quantity INT NOT NULL CHECK (quantity > 0),
 unit_price DECIMAL(10,2)  NOT NULL CHECK (unit_price >= 0),
 FOREIGN KEY (order_number) REFERENCES Orders(order_number),
 FOREIGN KEY (product_id) REFERENCES Product(product_id)
);

