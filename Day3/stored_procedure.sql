-- PROCEDURES

CREATE PROCEDURE proc_FirstProcedure
AS
BEGIN
	print 'Hello World!'
END

EXEC proc_FirstProcedure


CREATE TABLE Products(
 id INT identity(1,1) CONSTRAINT pk_producId PRIMARY KEY,  -- identity(seed,increment)
 name NVARCHAR(100) NOT NULL,  -- NVARCHAR -> Unicode characters for multilingual data
 details NVARCHAR(MAX)
 );

-- Procedure to insert products
CREATE OR ALTER PROC proc_InsertProduct(@pname NVARCHAR(100), @pdetails NVARCHAR(Max))
AS
BEGIN
	INSERT INTO Products(name,details) VALUES (@pname, @pdetails)
END

proc_InsertProduct 'Laptop','{"brand":"Dell","spec":{"ram":"16GB","cpu":"i5"}}'
go
select * from Products

-- Procedure to update products
CREATE PROC proc_UpdateProductSpec (@pid INT, @newvalue VARCHAR(100))
AS
BEGIN
	UPDATE Products
	SET details = JSON_MODIFY(details, '$.spec.ram' , @newvalue) WHERE id = @pid
END

proc_UpdateProductSpec 1, '24GB'

-- To get scalar values from Json -> JSON_VALUE
SELECT id, name, JSON_VALUE(details, '$.brand') Brand_Name
FROM Products

-- To get complex values from Json like array,object -> JSON_QUERY
SELECT id, name, JSON_QUERY(details, '$.spec') Brand_Name
FROM Products

SELECT JSON_QUERY(details, '$.spec') Product_Specification 
FROM Products;

CREATE TABLE Posts(
    id INT PRIMARY KEY,
    title NVARCHAR(100),
    user_id INT,
    body NVARCHAR(MAX)
);

-- Declare json data
DECLARE @jsondata NVARCHAR(MAX) = '[
  {
    "userId": 1,
    "id": 1,
    "title": "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
    "body": "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"
  },
  {
    "userId": 1,
    "id": 2,
    "title": "qui est esse",
    "body": "est rerum tempore vitae\nsequi sint nihil reprehenderit dolor beatae ea dolores neque\nfugiat blanditiis voluptate porro vel nihil molestiae ut reiciendis\nqui aperiam non debitis possimus qui neque nisi nulla"
  }]';

-- Use openjson to insert jsoon data
  INSERT INTO Posts (user_id, id, title, body)
  SELECT userId, id, title, body FROM openjson(@jsondata)
  WITH (userId INT, id INT, title NVARCHAR(100), body NVARCHAR(MAX));

  SELECT * FROM Posts;

  DELETE FROM Posts;

-- PROECEDURE to Insert Bulk Data
  CREATE PROC proc_BulkInsertPosts(@jsondata NVARCHAR(MAX))
  AS
  BEGIN
	INSERT INTO Posts(user_id, id, title, body)
	SELECT userId, id, title, body FROM openjson(@jsondata)
	WITH (userId INT, id INT , title NVARCHAR(100), body NVARCHAR(MAX))
  END;


  proc_BulkInsertPosts '[
  {
    "userId": 1,
    "id": 1,
    "title": "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
    "body": "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"
  },
  {
    "userId": 1,
    "id": 2,
    "title": "qui est esse",
    "body": "est rerum tempore vitae\nsequi sint nihil reprehenderit dolor beatae ea dolores neque\nfugiat blanditiis voluptate porro vel nihil molestiae ut reiciendis\nqui aperiam non debitis possimus qui neque nisi nulla"
  }]'


  select * from products where 
  json_value(details,'$.spec.cpu') ='i5';

-- Type cast json values to compare
  select * from products where 
  try_cast(json_value(details,'$.spec.cpu') as nvarchar(20)) ='i5'

-- Create a Procedure that brings post by taking the user_id as parameter
  CREATE OR ALTER PROC proc_GetUserPost(@puser_id INT)
  AS
  BEGIN
	SELECT * FROM Posts
	WHERE user_id = @puser_id
  END

  GO
  proc_GetUserPost 1;