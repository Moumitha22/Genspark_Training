-- Cursors 

-- Write a cursor to list all customers and how many rentals each made. Insert these into a summary table.
CREATE TABLE customer_rental_log(
	customer_id INT,
	customer_name VARCHAR(100),
	total_rents INT
)

DO 
$$
DECLARE 
	customer_record RECORD;
	customer_cursor CURSOR FOR 
		SELECT r.customer_id, CONCAT(first_name, ' ', last_name) AS customer_name, COUNT(*) AS total_rents
		FROM rental r
		JOIN customer c ON r.customer_id = c.customer_id
		GROUP BY r.customer_id,customer_name;
BEGIN
	OPEN customer_cursor;

	LOOP
		FETCH customer_cursor INTO customer_record;
		EXIT WHEN NOT FOUND;

		RAISE NOTICE 'Customer_ID: % | Customer_Name: % | Rentals: %',customer_record.customer_id,customer_record.customer_name,customer_record.total_rents;
		
		INSERT INTO customer_rental_log(customer_id,customer_name,total_rents)
		VALUES(customer_record.customer_id,customer_record.customer_name,customer_record.total_rents);
	END LOOP;

	CLOSE customer_cursor;
END;
$$
LANGUAGE plpgsql;

SELECT * FROM customer_rental_log ORDER BY customer_id;


-- Using a cursor, print the titles of films in the 'Comedy' category rented more than 10 times.
DO
$$
DECLARE
	film_record RECORD;
	film_cursor CURSOR FOR 
		SELECT title, COUNT(*) AS rental_count
		FROM film f
		JOIN film_category fc ON f.film_id = fc.film_id
		JOIN category c ON fc.category_id = c.category_id
		JOIN inventory i ON f.film_id = i.film_id
		JOIN rental r ON i.inventory_id = r.inventory_id
		WHERE c.name = 'Comedy'
		GROUP BY f.title;
BEGIN
	OPEN film_cursor;

	LOOP
		FETCH film_cursor INTO  film_record;
		EXIT WHEN NOT FOUND;

		IF  film_record.rental_count > 10 THEN
			RAISE NOTICE '% | %',film_record.title,film_record.rental_count;
		END IF;
	END LOOP;

	CLOSE film_cursor;
END;
$$
LANGUAGE plpgsql;

	
-- Create a cursor to go through each store and count the number of distinct films available, and insert results into a report table.
CREATE TABLE store_film_log(
	store_id INT,
	films_count INT
)

DO
$$
DECLARE
	store_record RECORD;
	store_cursor CURSOR FOR
		SELECT store_id, COUNT(DISTINCT film_id) AS films_count
		FROM inventory 
		GROUP BY store_id;
BEGIN
	OPEN store_cursor;

	LOOP
		FETCH store_cursor INTO store_record;
		EXIT WHEN NOT FOUND;

		INSERT INTO store_film_log(store_id,films_count)
		VALUES(store_record.store_id,store_record.films_count);
	END LOOP;

	CLOSE store_cursor;
END;
$$
LANGUAGE plpgsql;

SELECT * FROM store_film_log;


-- Loop through all customers who haven't rented in the last 6 months and insert their details into an inactive_customers table.
CREATE TABLE inactive_customers(
	customer_id INT,
	customer_name TEXT,
	last_rental_date TIMESTAMP
)
DROP TABLE inactive_customers;

DO 
$$
DECLARE 
	customer_record RECORD;
	customer_cursor CURSOR FOR 
		SELECT c.customer_id, CONCAT(first_name, ' ', last_name) AS customer_name, MAX(rental_date) AS last_rental_date
		FROM customer c 
		LEFT JOIN rental r ON c.customer_id = r.customer_id
		GROUP BY c.customer_id,customer_name
		HAVING MAX(r.rental_date) IS NULL OR (CURRENT_TIMESTAMP - MAX(r.rental_date) ) > INTERVAL '6 MONTHS';
BEGIN
	OPEN customer_cursor;

	LOOP
		FETCH customer_cursor INTO customer_record;
		EXIT WHEN NOT FOUND;

		RAISE NOTICE 'Customer_ID: % | Customer_Name: % | Last_Rental_Date: %',customer_record.customer_id,customer_record.customer_name,customer_record.last_rental_date;

		INSERT INTO inactive_customers(customer_id,customer_name,last_rental_date)
		VALUES(customer_record.customer_id,customer_record.customer_name,customer_record.last_rental_date);
	END LOOP;

	CLOSE customer_cursor;
END;
$$
LANGUAGE plpgsql;

 
SELECT * FROM inactive_customers;


-------------------------------------------------------------------------------------------------------------
-- Transactions 

-- Write a transaction that inserts a new customer, adds their rental, and logs the payment – all atomically.

BEGIN TRANSACTION;
	DO 
	$$
	DECLARE
		p_customer_id INT;
		p_rental_id INT;
	BEGIN
		INSERT INTO customer(store_id, first_name, last_name, email, address_id, create_date, active)
		VALUES(1, 'Moumitha', 'R', 'moumitha@gmail.com', 1, CURRENT_DATE, 1)
		RETURNING customer_id INTO p_customer_id ;
	
		INSERT INTO rental(rental_date, inventory_id, customer_id, staff_id)
		VALUES(CURRENT_TIMESTAMP, 1, p_customer_id, 1)
		RETURNING rental_id INTO p_rental_id;
	
		INSERT INTO payment(customer_id, staff_id, rental_id, amount, payment_date)
		VALUES(p_customer_id, 1, p_rental_id, 450.00, CURRENT_TIMESTAMP);
		
	END 
	$$;
COMMIT;
 
SELECT * FROM customer ORDER BY customer_id DESC;
SELECT * FROM rental WHERE customer_id = 605;
SELECT * FROM payment WHERE customer_id = 605;


-- Simulate a transaction where one update fails (e.g., invalid rental ID), and ensure the entire transaction rolls back.
BEGIN TRANSACTION;	
	UPDATE rental
	SET rental_date = CURRENT_TIMESTAMP
	WHERE rental_id = 16051;
	
	UPDATE rental
	SET rental_date = CURRENT_TIMESTAMP
	WHERE rent_id = 123456789;  -- invalid column name rent_id
	 
ROLLBACK;

 
-- Use SAVEPOINT to update multiple payment amounts. Roll back only one payment update using ROLLBACK TO SAVEPOINT.
BEGIN TRANSACTION;

	UPDATE payment SET amount = amount + 10 WHERE customer_id = 605;
	SAVEPOINT S1;
	
	UPDATE payment SET amount = amount + 10 WHERE customer_id = 605;
	SAVEPOINT S2;
	
	UPDATE payment SET amount = amount + 10 WHERE customer_id = 605;
	SAVEPOINT S3;
	
	UPDATE payment SET amount = amount + 10 WHERE customer_id = 605;
	
	ROLLBACK TO SAVEPOINT S2;
	
COMMIT;

 
-- Perform a transaction that transfers inventory from one store to another (delete + insert) safely.
BEGIN TRANSACTION;
	DO 
	$$
	DECLARE
	    p_store_id INTEGER;
	BEGIN
	    SELECT store_id INTO p_store_id
	    FROM inventory
	    WHERE inventory_id = 1;
	 
	    IF p_store_id = 1 THEN
	        UPDATE inventory SET store_id = 2 WHERE inventory_id = 1;
	    ELSIF v_store_id = 2 THEN
	        UPDATE inventory SET store_id = 1 WHERE inventory_id = 1;
	    END IF;
	END
	$$;
COMMIT;
 
 
-- Create a transaction that deletes a customer and all associated records (rental, payment), ensuring referential integrity.
BEGIN TRANSACTION;

	DELETE FROM payment WHERE customer_id = 605;
	DELETE FROM rental WHERE customer_id = 605;
	DELETE FROM customer WHERE customer_id = 605;
	
COMMIT;

------------------------------------------------------------------------------------------------------------------
-- Triggers

-- Create a trigger to prevent inserting payments of zero or negative amount.
CREATE OR REPLACE FUNCTION prevent_invalid_payment()
RETURNS TRIGGER AS 
$$
BEGIN
    IF NEW.amount <= 0 THEN
        RAISE EXCEPTION 'Invalid amount: %. Amount should be greater than 0.00', NEW.amount;
    END IF;
    RETURN NEW;  
END;
$$ 
LANGUAGE plpgsql;
 
CREATE TRIGGER trg_check_amount
BEFORE INSERT ON payment
FOR EACH ROW
EXECUTE FUNCTION prevent_invalid_payment();
 
INSERT INTO payment (customer_id, staff_id, rental_id, amount, payment_date)
VALUES (1, 1, 1185, 0, CURRENT_TIMESTAMP);

 
-- Set up a trigger that automatically updates last_update on the film table when the title or rental rate is changed.
CREATE OR REPLACE FUNCTION update_last_update()
RETURNS TRIGGER AS
$$
BEGIN
    IF NEW.title <> OLD.title OR NEW.rental_rate <> OLD.rental_rate THEN
        NEW.last_update := CURRENT_TIMESTAMP;
	END IF;
    RETURN NEW;
END;
$$ 
LANGUAGE plpgsql;
 
CREATE TRIGGER trg_update_last_update
BEFORE UPDATE ON film
FOR EACH ROW
EXECUTE FUNCTION update_last_update();
 
UPDATE film SET title = 'Academy Dinosaurs' WHERE film_id = 1;
UPDATE film SET title = 'Academy Dinosaur' WHERE film_id = 1;


-- Write a trigger that inserts a log into rental_log whenever a film is rented more than 3 times in a week.
CREATE TABLE rental_log (
    log_id SERIAL PRIMARY KEY,
    film_id INT,
    rental_count INT,
    log_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
 
CREATE OR REPLACE FUNCTION check_rental_frequency()
RETURNS TRIGGER AS $$
DECLARE
    rental_count INT;
	filmid INT;
BEGIN
	SELECT film_id INTO filmid
    FROM inventory
    WHERE inventory_id = NEW.inventory_id;
 
    SELECT COUNT(*)
    INTO rental_count
    FROM rental
    WHERE inventory_id = NEW.inventory_id
    AND rental_date >= CURRENT_DATE - INTERVAL '7 days';
 
    IF rental_count > 3 THEN
        INSERT INTO rental_log (film_id, rental_count)
        VALUES (filmid, rental_count);
    END IF;
 
    RETURN NEW;
END;
$$ 
LANGUAGE plpgsql;
 
CREATE TRIGGER trg_check_rental_frequency
AFTER INSERT ON rental
FOR EACH ROW
EXECUTE FUNCTION check_rental_frequency();
 
INSERT INTO rental (rental_date, inventory_id, customer_id, staff_id)
VALUES (CURRENT_TIMESTAMP, 4, 1, 1);
 
SELECT * FROM rental_log;