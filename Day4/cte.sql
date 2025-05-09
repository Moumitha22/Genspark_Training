--Procedure to filter Products with OUT parameter
  CREATE PROC proc_FilterProducts(@pcpu NVARCHAR(20), @pcount INT OUT)
  AS
  BEGIN 
	SET @pcount = (SELECT COUNT(*) FROM Products WHERE
	TRY_CAST(JSON_VALUE(details, '$.spec.cpu') AS NVARCHAR(20)) = @pcpu)
  END

BEGIN
	DECLARE @cnt INT
	EXEC proc_FilterProducts 'i5', @cnt OUT
	PRINT CONCAT ('The number of computers is ',@cnt)
END

sp_help authors

CREATE TABLE People(
 id INT PRIMARY KEY,
 name NVARCHAR(20),
 age INT
 );

 -- Procedure to bulk insert from a file
 CREATE OR ALTER PROC proc_BulkInsertPeople(@filepath NVARCHAR(500))
 AS
 BEGIN
	DECLARE @insertquery NVARCHAR(MAX)

	SET @insertquery = 'BULK INSERT people FROM '''+ @filepath +'''
	WITH(
	FIRSTROW =2,
    FIELDTERMINATOR='','',
    ROWTERMINATOR = ''\n'')'

	EXEC sp_executesql @insertQuery

 END


proc_BulkInsertPeople 'C:\Users\moumithar\Documents\Training\Day4\Data.csv'

SELECT * FROM PEOPLE;

CREATE TABLE BulkInsertLog(
	LogId INT IDENTITY(1,1) PRIMARY KEY,
	FilePath NVARCHAR(1000),
	status nvarchar(50) CONSTRAINT chk_status CHECK(status in('Success','Failed')),
	Message NVARCHAR(1000),
	InsertedOn DateTime DEFAULT GetDate()
)

-- Procedure to bulk insert from a file with logging
CREATE OR ALTER PROC proc_BulkInsertPeople(@filepath NVARCHAR(500))
 AS
 BEGIN
	BEGIN TRY
		DECLARE @insertquery NVARCHAR(MAX)

		SET @insertquery = 'BULK INSERT people FROM '''+ @filepath +'''
		WITH(
		FIRSTROW =2,
		FIELDTERMINATOR='','',
		ROWTERMINATOR = ''\n'')'

		EXEC sp_executesql @insertQuery

		INSERT INTO BulkInsertLog(filepath,status,message)
	    VALUES(@filepath,'Success','Bulk insert completed')
	 END TRY
	 BEGIN CATCH
		 INSERT INTO BulkInsertLog(filepath,status,message)
		 VALUES(@filepath,'Failed',Error_Message())
	END Catch
 END

proc_BulkInsertPeople 'C:\Users\moumithar\Documents\Training\Day4\Data.csv'

SELECT * FROM BulkInsertLog

TRUNCATE TABLE people

DROP TABLE People
DROP TABLE BulkInsertLog


-- CTE - Common Table Expression
WITH cteAuthors
AS
(SELECT au_id, concat(au_fname,' ',au_lname) author_name FROM authors)

SELECT * FROM cteAuthors

SELECT * FROM titles

DECLARE @page INT=2, @pageSize INT=10;
WITH PaginatedBooks AS
( SELECT title_id, title, price, ROW_Number() OVER (ORDER BY price DESC) as RowNum
  FROM titles
)
SELECT * FROM PaginatedBooks WHERE RowNum between((@page-1)*@pageSize) and (@page*@pageSize)
-- SELECT * FROM PaginatedBooks;

-- create a sp that will take the page number and size as param and print the books
CREATE OR ALTER PROC proc_PaginateTitles(@page INT, @pageSize INT)
AS
BEGIN
	WITH PaginatedBooks AS
	( SELECT title_id, title, price, ROW_Number() OVER (ORDER BY price DESC) as RowNum
	  FROM titles
	)
	SELECT * FROM PaginatedBooks WHERE RowNum between((@page-1)*@pageSize) and (@page*@pageSize)
END

proc_PaginateTitles 2,5

-- Offset
SELECT title_id, title, price 
FROM titles
ORDER BY PRICE DESC
OFFSET 10 ROWS;

-- FETCH NEXT ONLY
SELECT title_id, title, price 
FROM titles
ORDER BY PRICE DESC
OFFSET 10 ROWS FETCH NEXT 5 ROWS ONLY;

