-- FUNCTION
-- SCALAR VALUE FUNCTION
CREATE FUNCTION fn_CalculateTax(@baseprice FLOAT, @tax FLOAT)
RETURNS FLOAT
AS
BEGIN
	RETURN (@baseprice + (@baseprice * @tax / 100))
END

SELECT dbo.fn_CalculateTax(1000,10)
SELECT dbo.fn_CalculateTax(1000,20)

SELECT title,dbo.fn_CalculateTax(price,12) Tax FROM titles

-- TABLE VALUE FUNCTION
CREATE FUNCTION fn_TableSample(@minprice FLOAT)
RETURNS TABLE
AS
	RETURN SELECT title, price FROM titles WHERE price >= @minprice

SELECT * FROM dbo.fn_TableSample(10)

--  Older and slower but supports more logic
CREATE FUNCTION fn_tableSampleOld(@minprice float)
  RETURNS @Result TABLE(Book_Name nvarchar(100), price float)
  AS
  BEGIN
    INSERT INTO @Result SELECT title, price FROM titles WHERE price >= @minprice
    RETURN 
END

SELECT * FROM dbo.fn_tableSampleOld(10)