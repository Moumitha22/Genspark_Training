-- TRANSACTION INSIDE SP

CREATE OR REPLACE PROCEDURE proc_create_customer_rental_payment(
p_first_name text,p_last_name text, p_email text,p_address_id int, 
p_inventory_id int, p_store_id int,
p_staff_id int,p_amount numeric
)
Language plpgsql
as $$
DECLARE
    v_customer_id INT;
    v_rental_id INT;
BEGIN
  Begin
    INSERT INTO customer (store_id, first_name, last_name, email, address_id, active, create_date)
    VALUES (p_store_id,p_first_name,p_last_name,p_email,p_address_id, 1, CURRENT_DATE)
    RETURNING customer_id INTO v_customer_id;
 
    INSERT INTO rental (rental_date, inventory_id, customer_id, staff_id)
    VALUES (CURRENT_TIMESTAMP, p_inventory_id, v_customer_id, p_staff_id)
    RETURNING rental_id INTO v_rental_id;
    
    INSERT INTO payment (customer_id, staff_id, rental_id, amount, payment_date)
    VALUES (v_customer_id, p_staff_id, v_rental_id, p_amount, CURRENT_TIMESTAMP);
	
  Exception when others then
    raise notice 'Transaction failed %',sqlerrm;
	
  End;
END; 
$$;

select * from customer order by customer_id  desc

call proc_create_customer_rental_payment ('Ram','Som','ram_som@gmail.com',1,1,1,1,10)
call proc_create_customer_rental_payment ('Ram','Som','ram_som@gmail.com',1,1,1,1,-10)

---------------------------------------------------------------------------------------------

-- CURSOR INSIDE SP
--loop through all the films and update the rental rate by +1 for the films when rental count < 5


CREATE OR REPLACE PROCEDURE proc_update_rental_rate()
language plpgsql
as $$
declare
  rec record;
  cur_film_rent_count cursor for
	  select f.film_id, f.rental_rate, count(r.rental_id) as rental_count 
	  from film f left join inventory i on f.film_id = i.film_id
	  left join rental r on i.inventory_id = r.inventory_id
	  group by f.film_id, f.rental_rate;
Begin
  open cur_film_rent_count;

  Loop
  	Fetch cur_film_rent_count into rec;
	exit when not found;

	if rec.rental_count < 5 then
	   update film set rental_rate= rental_rate +1
	   where film_id =  rec.film_id;

	   raise notice 'updated file  with id % . The new rental rate is %',rec.film_id,rec.rental_rate+1;
	end if;
end loop;
close cur_film_rent_count;
end;
$$;

call proc_update_rental_rate();
----------------------------------------------------------------------------------------------
