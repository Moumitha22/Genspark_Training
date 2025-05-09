-- SELECT Queries

-- List all films with their length and rental rate, sorted by length descending.
-- Columns: title, length, rental_rate

SELECT title, length, rental_rate FROM film
ORDER BY length DESC;

-- Find the top 5 customers who have rented the most films.
-- Hint: Use the rental and customer tables.

SELECT c.customer_id, concat(first_name,' ',last_name) Customer_Name, COUNT(*) AS Total_Rentals 
FROM customer c
JOIN rental r ON c.customer_id = r.customer_id
GROUP BY c.customer_id, Customer_Name
ORDER BY Total_Rentals  DESC
LIMIT 5;

-- Display all films that have never been rented.
-- Hint: Use LEFT JOIN between film and inventory → rental.

SELECT f.film_id, title, r.rental_id FROM film f
LEFT JOIN inventory i ON f.film_id = i.film_id
LEFT JOIN rental r ON i.inventory_id = r.inventory_id
WHERE r.rental_id IS NULL;

-- or
SELECT f.film_id, title FROM film f 
LEFT JOIN inventory i ON f.film_id = i.film_id 
WHERE i.inventory_id NOT IN (SELECT inventory_id FROM Rental);
 
-- JOIN Queries

-- List all actors who appeared in the film ‘Academy Dinosaur’.
-- Tables: film, film_actor, actor

SELECT a.actor_id, concat(first_name,' ',last_name) Actor_Name FROM actor a
JOIN film_actor fa ON a.actor_id = fa.actor_id
JOIN film f ON fa.film_id = f.film_id
WHERE f.title = 'Academy Dinosaur';

SELECT a.actor_id, concat(first_name,' ',last_name) Actor_Name 
FROM film_actor fa 
JOIN actor a ON a.actor_id = fa.actor_id
WHERE fa.film_id = (SELECT film_id FROM film WHERE title= 'Academy Dinosaur');
 
-- List each customer along with the total number of rentals they made and the total amount paid.
-- Tables: customer, rental, payment

SELECT c.customer_id, concat(first_name,' ',last_name) Customer_Name, SUM(amount) Total_Amount FROM customer c
JOIN rental r ON c.customer_id = r.customer_id
JOIN payment p ON r.rental_id = p.rental_id
GROUP BY c.customer_id, concat(c.first_name,' ',c.last_name);

-- CTE-Based Queries

-- Using a CTE, show the top 3 rented movies by number of rentals.
-- Columns: title, rental_count
WITH cte_MoviesRentCount AS
(
	SELECT f.film_id, title, COUNT(*) AS Total_Rentals 
	FROM film f
	JOIN inventory i ON f.film_id = i.film_id
	JOIN rental r ON i.inventory_id = r.inventory_id
	GROUP BY f.film_id
	ORDER BY COUNT(*) DESC
)
SELECT * FROM cte_MoviesRentCount
LIMIT 3;

-- Find customers who have rented more than the average number of films.
-- Use a CTE to compute the average rentals per customer, then filter.

WITH cte_AvgRentalsPerCustomer AS
(
 SELECT AVG(Rental_Count) AS Avg_Rental_Count FROM 
	( 
		SELECT customer_id, COUNT(*) AS Rental_Count FROM rental 
		GROUP BY customer_id
	) AS Avg_Rental_Count
)
SELECT c.customer_id, concat(first_name,' ',last_name) Customer_Name,COUNT(*) AS Total_Rentals 
FROM customer c
JOIN rental r ON c.customer_id = r.customer_id
GROUP BY c.customer_id
HAVING COUNT(*) > (SELECT Avg_Rental_Count FROM cte_AvgRentalsPerCustomer); 

-- 0R USE TWO CTES

WITH cte_Rental_Counts AS
(
  	SELECT c.customer_id, concat(first_name,' ',last_name) Customer_Name,COUNT(*) AS Total_Rentals 
	FROM customer c
	JOIN rental r ON c.customer_id = r.customer_id
	GROUP BY c.customer_id
),
cte_Average_Rentals AS (
	SELECT AVG(Total_Rentals) AS Avg_Rentals FROM cte_Rental_Counts
)
SELECT * FROM cte_Rental_counts rc
JOIN cte_Average_Rentals ar ON rc.Total_Rentals > ar.Avg_Rentals

-- Function Questions

-- Write a function that returns the total number of rentals for a given customer ID.
-- Function: get_total_rentals(customer_id INT)

CREATE OR REPLACE FUNCTION fn_get_total_rentals(pcustomer_id INT)
RETURNS INT
AS 
$$
DECLARE total_rentals INT;
BEGIN
 	SELECT COUNT(*) INTO total_rentals FROM rental
	WHERE customer_id = pcustomer_id;
	RETURN total_rentals;
END;
$$ 
LANGUAGE plpgsql;

SELECT fn_get_total_rentals(1) AS Total_Rentalss;

-- Stored Procedure Questions

-- Write a stored procedure that updates the rental rate of a film by film ID and new rate.
-- Procedure: update_rental_rate(film_id INT, new_rate NUMERIC)

CREATE OR REPLACE PROCEDURE proc_Update_Rental_Rate(pfilm_id INT, new_rate NUMERIC)
AS
$$
BEGIN
	UPDATE film
	SET rental_rate = new_rate
	WHERE film_id = pfilm_id;
END;
$$
LANGUAGE plpgsql;

CALL proc_Update_Rental_Rate(8,4.98);

SELECT * FROM film WHERE film_id = 8;

-- Write a procedure to list overdue rentals (return date is NULL and rental date older than 7 days).
-- Procedure: get_overdue_rentals() that selects relevant columns.

DROP FUNCTION get_overdue_rentals()
CREATE OR REPLACE FUNCTION get_overdue_rentals()
RETURNS TABLE(rental_id INT, customer_id SMALLINT, rental_date TIMESTAMP, return_date TIMESTAMP)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT r.rental_id, r.customer_id, r.rental_date, r.return_date
    FROM rental r
    WHERE 
	r.return_date IS NULL OR (CURRENT_DATE - r.rental_date) > INTERVAL '7 days';
END;
$$;

SELECT * FROM get_overdue_rentals();

