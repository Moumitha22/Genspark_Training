-- Table Schema:	 

-- Create Tables with Integrity Constrains: 

-- a)	EMP (empno - primary key, empname, salary, deptname - references entries in a deptname of department table with null constraint, bossno - references entries in an empno of emp table with null constraint) 
-- b)	DEPARTMENT (deptname - primary key, floor, phone, empno - references entries in an empno of emp table not null) 
-- c)	SALES (salesno - primary key, saleqty, itemname -references entries in a itemname of item table with not null constraint, deptname - references entries in a deptname of department table with not null constraint) 
-- d)	ITEM (itemname - primary key, itemtype, itemcolor) 

CREATE TABLE Department(
 dept_name VARCHAR(100) PRIMARY KEY,
 floor INT NOT NULL,
 phone VARCHAR(30) NOT NULL,
 manager_id INT NULL,
);

CREATE TABLE Employees(
 emp_no INT PRIMARY KEY,
 emp_name VARCHAR(100) NOT NULL,
 salary DECIMAL(10,2) NOT NULL CHECK (salary >= 0),
 dept_name VARCHAR(100) NULL,
 boss_no INT NULL,
 FOREIGN KEY (dept_name) REFERENCES DEPARTMENT(dept_name),
 FOREIGN KEY (boss_no) REFERENCES Employees(emp_no)
);

INSERT INTO Department (dept_name, floor, phone) VALUES
('Management', 5, '34'),
('Books', 1, '81'),
('Clothes', 2, '24'),
('Equipment', 3, '57'),
('Furniture', 4, '14'),
('Navigation', 1, '41'),
('Recreation', 2, '29'),
('Accounting', 5, '35'),
('Purchasing', 5, '36'),
('Personnel', 5, '37'),
('Marketing', 5, '38');

INSERT INTO Employees(emp_no, emp_name, salary, dept_name, boss_no) VALUES
(1, 'Alice', 75000, 'Management', NULL),  -- Top boss
(2, 'Ned', 45000, 'Marketing', 1),
(3, 'Andrew', 5000, 'Marketing', 2),
(4, 'Clare', 22000, 'Marketing', 2),
(5, 'Todd', 38000, 'Accounting', 1),
(6, 'Nancy', 22000, 'Accounting', 5),
(7, 'Brier', 43000, 'Purchasing', 1),
(8, 'Sarah', 56000, 'Purchasing', 7),
(9, 'Soptile', 35000, 'Personnel', 1),
(10, 'Sanjay', 15000, 'Navigation', 3),
(11, 'Rita', 15000, 'Books', 4),
(12, 'Gigi', 16000, 'Clothes', 4),
(13, 'Maggie', 11000, 'Clothes', 4),
(14, 'Paul', 15000, 'Equipment', 3),
(15, 'James', 15000, 'Equipment', 3),
(16, 'Pat', 15000, 'Furniture', 3),
(17, 'Mark', 15000, 'Recreation', 3);

ALTER TABLE Department
ADD CONSTRAINT fk_dept_emp FOREIGN KEY (manager_id) REFERENCES Employees(emp_no);

UPDATE DEPARTMENT SET manager_id = 1 WHERE dept_name = 'Management';
UPDATE DEPARTMENT SET manager_id = 4 WHERE dept_name IN ('Books', 'Clothes');
UPDATE DEPARTMENT SET manager_id = 3 WHERE dept_name IN ('Navigation', 'Equipment', 'Furniture', 'Recreation');
UPDATE DEPARTMENT SET manager_id = 5 WHERE dept_name = 'Accounting';
UPDATE DEPARTMENT SET manager_id = 7 WHERE dept_name = 'Purchasing';
UPDATE DEPARTMENT SET manager_id = 9 WHERE dept_name = 'Personnel';
UPDATE DEPARTMENT SET manager_id = 2 WHERE dept_name = 'Marketing';

CREATE TABLE Item(
 item_name VARCHAR(100) PRIMARY KEY,
 item_type VARCHAR(100),
 item_color VARCHAR(100),
);


CREATE TABLE Sale(
 sales_no INT PRIMARY KEY,
 sales_qty INT,
 item_name VARCHAR(100),
 dept_name VARCHAR(100),
 FOREIGN KEY (item_name) REFERENCES ITEM(item_name),
 FOREIGN KEY (dept_name) REFERENCES DEPARTMENT(dept_name)
);


INSERT INTO ITEM (item_name, item_type, item_color) VALUES
('Pocket Knife-Nile', 'E', 'Brown'),
('Pocket Knife-Avon', NULL, 'Brown'),
('Compass', 'N', NULL),
('Geo positioning system', 'N', NULL),
('Elephant Polo stick', NULL, 'Bamboo'),
('Camel Saddle', 'R', 'Brown'),
('Sextant', 'N', NULL),
('Map Measure', 'N', NULL),
('Boots-snake proof', NULL, 'Green'),
('Pith Helmet', NULL, 'Khaki'),
('Hat-polar Explorer', NULL, 'White'),
('Exploring in 10 Easy Lessons', NULL, NULL),
('Hammock', NULL, 'Khaki'),
('How to win Foreign Friends', NULL, 'Brown'),
('Safari Chair', NULL, 'Khaki'),
('Safari cooking kit', 'F', 'Khaki'),
('Stetson', 'C', 'Black');


INSERT INTO SALE (sales_no, sales_qty, item_name, dept_name) VALUES
(101, 2, 'Boots-snake proof', 'Clothes'),
(102, 1, 'Pith Helmet', 'Clothes'),
(103, 1, 'Sextant', 'Navigation'),
(104, 3, 'Hat-polar Explorer', 'Clothes'),
(105, 5, 'Pith Helmet', 'Equipment'),
(106, 2, 'Pocket Knife-Nile', 'Clothes'),
(107, 3, 'Pocket Knife-Nile', 'Recreation'),
(108, 1, 'Compass', 'Navigation'),
(109, 2, 'Geo positioning system', 'Navigation'),
(110, 1, 'Map Measure', 'Navigation'),
(111, 1, 'Geo positioning system', 'Books'),
(112, 1, 'Sextant', 'Books'),
(113, 1, 'Pocket Knife-Nile', 'Books'),
(114, 1, 'Pocket Knife-Nile', 'Navigation');