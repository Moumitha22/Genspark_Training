-- 1. Try Two Concurrent Updates to the Same Row (See Lock in Action)
-- Session 1
BEGIN;
UPDATE rental
SET rental_date = CURRENT_TIMESTAMP
WHERE rental_id = 1;
 
-- Session 2 (runs concurrently with Session 1)
BEGIN;
UPDATE rental
SET rental_date = CURRENT_TIMESTAMP
WHERE rental_id = 1;
-- Session 2 update is blocked and waiting for Session 1 to commit or roll back
 
-- 2. Write a Query Using SELECT...FOR UPDATE and Check How It Locks Row
BEGIN;
SELECT *
FROM rental
WHERE rental_id = 1
FOR UPDATE;
-- Row is locked, no other transaction can update it until this transaction is committed or rolled back

BEGIN;
UPDATE rental
SET rental_date = CURRENT_TIMESTAMP
WHERE rental_id = 1;
 
-- 3. Intentionally Create a Deadlock and Observe PostgreSQL Cancel One Transaction
 
BEGIN;
-- Lock row 1
SELECT * FROM rental WHERE rental_id = 1 FOR UPDATE;
UPDATE rental SET rental_date = CURRENT_TIMESTAMP WHERE rental_id = 2;
 
BEGIN;
-- Lock row 2
SELECT * FROM rental WHERE rental_id = 2 FOR UPDATE;
UPDATE rental SET rental_date = CURRENT_TIMESTAMP WHERE rental_id = 1;
 
-- This will cause a deadlock because Session 1 holds a lock on rental_id = 1 and is waiting for a lock on rental_id = 2, while Session 2 holds a lock on rental_id = 2 and is waiting for a lock on rental_id = 1. PostgreSQL will cancel one of the transactions to resolve the deadlock.
 
 
-- 4. Use pg_locks Query to Monitor Active Locks
-- Query to check all active locks in the system
SELECT *
FROM pg_locks
JOIN pg_stat_activity ON pg_locks.pid = pg_stat_activity.pid
WHERE NOT pg_locks.granted;