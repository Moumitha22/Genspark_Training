-- Cursor-Based Questions (5)

-- Write a cursor that loops through all films and prints titles longer than 120 minutes.
DO 
$$
DECLARE
    film_cursor CURSOR FOR SELECT title, length FROM film;
    film_title TEXT;
    film_length INT;
BEGIN
    OPEN film_cursor;
 
    LOOP
        FETCH film_cursor INTO film_title, film_length;
        EXIT WHEN NOT FOUND;
 
        IF film_length > 120 THEN
            RAISE NOTICE 'Title: %', film_title;
        END IF;
    END LOOP;
 
    CLOSE film_cursor;
END;
$$
LANGUAGE plpgsql;
 
 
-- Create a cursor that iterates through all customers and counts how many rentals each made.
DO 
$$
DECLARE
    customer_cursor CURSOR FOR SELECT customer_id, first_name, last_name FROM customer;
    cust_id INT;
    f_name TEXT;
    l_name TEXT;
    rental_count INT;
BEGIN
    OPEN customer_cursor;
 
    LOOP
        FETCH customer_cursor INTO cust_id, f_name, l_name;
        EXIT WHEN NOT FOUND;
 
        SELECT COUNT(*) INTO rental_count
        FROM rental
        WHERE rental.customer_id = cust_id;
 
        RAISE NOTICE 'Customer: %, % | Rentals: %', l_name, f_name, rental_count;
    END LOOP;
 
    CLOSE customer_cursor;
END;
$$
LANGUAGE plpgsql;
 
-- Using a cursor, update rental rates: Increase rental rate by $1 for films with less than 5 rentals.
DO 
$$
DECLARE
    film_rec RECORD;
    rental_count INT;
BEGIN
    FOR film_rec IN
        SELECT film_id, title, rental_rate FROM film
    LOOP
        SELECT COUNT(*) INTO rental_count
        FROM inventory i
        JOIN rental r ON r.inventory_id = i.inventory_id
        WHERE i.film_id = film_rec.film_id;
 
        IF rental_count < 5 THEN
            UPDATE film
            SET rental_rate = rental_rate + 1
            WHERE film_id = film_rec.film_id;
 
            RAISE NOTICE 'Updated: %, Rentals: %, New Rate: %', 
                film_rec.title, rental_count, film_rec.rental_rate + 1;
        END IF;
    END LOOP;
END;
$$ 
LANGUAGE plpgsql;
 
 
-- Create a function using a cursor that collects titles of all films from a particular category.
CREATE OR REPLACE FUNCTION get_films_by_category(p_category_name TEXT)
RETURNS VOID AS $$
DECLARE
    film_cursor CURSOR FOR
        SELECT f.title
        FROM film f
        JOIN film_category fc ON f.film_id = fc.film_id
        JOIN category c ON fc.category_id = c.category_id
WHERE c.name ILIKE p_category_name;
    film_title TEXT;
BEGIN
    OPEN film_cursor;
 
    LOOP
        FETCH film_cursor INTO film_title;
        EXIT WHEN NOT FOUND;
        RAISE NOTICE 'Film Title: %', film_title;  -- Debug output for each film title
    END LOOP;
 
    CLOSE film_cursor;
 
    IF NOT FOUND THEN
        RAISE NOTICE 'No films found in category: %', p_category_name;  -- Notify if no films are found
    END IF;
END;
$$ 
LANGUAGE plpgsql;
 
SELECT get_films_by_category('Animation');
 
-- Loop through all stores and count how many distinct films are available in each store using a cursor.
DO 
$$
DECLARE
    store_rec RECORD;
    film_count INT;
BEGIN
    FOR store_rec IN SELECT store_id FROM store LOOP
        SELECT COUNT(DISTINCT film_id)
        INTO film_count
        FROM inventory
        WHERE store_id = store_rec.store_id;
		
        RAISE NOTICE 'Store ID: %, Distinct Films: %', store_rec.store_id, film_count;
    END LOOP;
END 
$$;

----------------------------------------------------------------------------------
-- Trigger-Based Questions (5)
 
-- Write a trigger that logs whenever a new customer is inserted
CREATE TABLE customer_log (
	log_id SERIAL PRIMARY KEY,
	customer_id INT,
	customer_name TEXT,
	log_time TIMESTAMP DEFAULT CURRENT_TIMESTAMP
)
 
CREATE OR REPLACE FUNCTION log_new_customer()
RETURNS TRIGGER AS 
$$
BEGIN
    INSERT INTO customer_log (customer_id, customer_name)
    VALUES (NEW.customer_id, NEW.first_name || ' ' || NEW.last_name);
    RETURN NEW;
END;
$$
LANGUAGE plpgsql;
 
CREATE TRIGGER trg_log_customer_insert
AFTER INSERT ON customer
FOR EACH ROW
EXECUTE FUNCTION log_new_customer()
 
INSERT INTO customer (first_name, last_name, email, store_id, address_id)
VALUES 
('Nila', 'Krishnan', 'nilakrishnan@example.com', 1, 2);
SELECT * FROM customer_log
 
 
-- Create a trigger that prevents inserting a payment of amount 0.
CREATE TRIGGER try_prevents_payment 
BEFORE INSERT ON payment
FOR EACH ROW
EXECUTE FUNCTION fn_verify_payment()
 
-- Create the function that verifies the payment amount
CREATE OR REPLACE FUNCTION fn_verify_payment()
RETURNS TRIGGER AS 
$$
BEGIN
    IF NEW.amount = 0 THEN
        RAISE EXCEPTION 'Payment amount cannot be 0';
    END IF;
 
    RETURN NEW;
END;
$$ 
LANGUAGE plpgsql;
 
INSERT INTO payment (payment_id, customer_id, staff_id, rental_id, amount, payment_date)
VALUES 
(1, 1, 1, 1, 5.99, '2025-05-09');
 
 INSERT INTO payment (payment_id, customer_id, staff_id, rental_id, amount, payment_date)
VALUES 
(2, 1, 1, 1, 0, '2025-05-09');

-- Set up a trigger to automatically set last_update on the film table before update.
CREATE OR REPLACE FUNCTION update_last_update()
RETURNS TRIGGER AS 
$$
BEGIN
    NEW.last_update := CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ 
LANGUAGE plpgsql;
 
CREATE TRIGGER trigger_update_last_update
BEFORE UPDATE ON film
FOR EACH ROW
EXECUTE FUNCTION update_last_update();
 
-- Create a trigger to log changes in the inventory table (insert/delete).
CREATE TABLE inventory_log (
    log_id SERIAL PRIMARY KEY,
    action_type VARCHAR(10),  -- 'INSERT' or 'DELETE'
    inventory_id INT,
    log_time TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
 
CREATE OR REPLACE FUNCTION log_inventory_changes()
RETURNS TRIGGER AS 
$$
BEGIN
    IF (TG_OP = 'INSERT') THEN
        INSERT INTO inventory_log(action_type, inventory_id)
        VALUES ('INSERT', NEW.inventory_id);
    ELSIF (TG_OP = 'DELETE') THEN
        INSERT INTO inventory_log(action_type, inventory_id)
        VALUES ('DELETE', OLD.inventory_id);
    END IF;
    RETURN NULL;
END;
$$ 
LANGUAGE plpgsql;
 
CREATE TRIGGER trigger_log_inventory_changes
AFTER INSERT OR DELETE ON inventory
FOR EACH ROW
EXECUTE FUNCTION log_inventory_changes();
 
-- Write a trigger that ensures a rental can’t be made for a customer who owes more than $50.
CREATE OR REPLACE FUNCTION check_outstanding_balance()
RETURNS TRIGGER AS 
$$
DECLARE
    outstanding_balance DECIMAL;
BEGIN
    -- Calculate the total outstanding balance for the customer
    SELECT SUM(amount) INTO outstanding_balance
    FROM payment
    WHERE customer_id = NEW.customer_id AND payment_date > CURRENT_DATE - INTERVAL '30 days';
 
    -- If the outstanding balance is greater than $50, raise an exception
    IF outstanding_balance > 50 THEN
        RAISE EXCEPTION 'Customer owes more than $50, rental cannot be made.';
    END IF;
 
    RETURN NEW;
END;
$$ 
LANGUAGE plpgsql;
 
CREATE TRIGGER trigger_check_outstanding_balance
BEFORE INSERT ON rental
FOR EACH ROW
EXECUTE FUNCTION check_outstanding_balance();

-- Transaction-Based Questions (5)

-- Write a transaction that inserts a customer and an initial rental in one atomic operation.
BEGIN TRANSACTION;
DO 
$$ 
DECLARE
    cust_id INT;
BEGIN
    INSERT INTO customer (first_name, last_name, email, address_id, active, create_date, store_id)
    VALUES ('Dia', 'Singh', 'diasinghe@example.com', 1, 1, NOW(), 1)
    RETURNING customer_id INTO cust_id;
 
    INSERT INTO rental (rental_date, inventory_id, customer_id, return_date, staff_id)
    VALUES (NOW(), 1, cust_id, NULL, 1);
END 
$$;
COMMIT;
 
ROLLBACK
 
-- Simulate a failure in a multi-step transaction (update film + insert into inventory) and roll back.
BEGIN TRANSACTION;
 
UPDATE film
SET rental_duration = 5
WHERE title = 'ACADEMY DINOSAUR';
 
INSERT INTO inventory (film_id, store_id, last_update)
VALUES (9990, 1, NOW());
 
COMMIT;
ROLLBACK
 
-- Create a transaction that transfers an inventory item from one store to another.
BEGIN TRANSACTION;
DO $$
DECLARE
    v_store_id INTEGER;
BEGIN
    SELECT store_id INTO v_store_id
    FROM inventory
    WHERE inventory_id = 98;
 
    IF v_store_id = 1 THEN
        UPDATE inventory SET store_id = 2 WHERE inventory_id = 98;
    ELSIF v_store_id = 2 THEN
        UPDATE inventory SET store_id = 1 WHERE inventory_id = 98;
    END IF;
END $$;
COMMIT;
 
-- Demonstrate SAVEPOINT and ROLLBACK TO SAVEPOINT by updating payment amounts, then undoing one.
BEGIN TRANSACTION;

UPDATE payment 
SET amount = amount + 1 
WHERE payment_id = 1;

SAVEPOINT after_first_update;

UPDATE payment 
SET amount = amount + 2 
WHERE payment_id = 1;

ROLLBACK TO SAVEPOINT after_first_update;

UPDATE payment 
SET amount = amount + 3 
WHERE payment_id = 1;

COMMIT;
 
-- Write a transaction that deletes a customer and all associated rentals and payments, ensuring atomicity.
BEGIN TRANSACTION;

DELETE FROM payment WHERE customer_id = 123;
DELETE FROM rental WHERE customer_id = 123;
DELETE FROM customer WHERE customer_id = 123;

COMMIT;