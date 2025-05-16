-- Phase 7: Transactions & Atomicity
-- Write a transaction block that:
-- * Enrolls a student
-- * Issues a certificate
-- * Fails if certificate generation fails (rollback)
 
CREATE OR REPLACE PROCEDURE sp_enroll_and_certify(
    p_student_id INT,
    p_course_id INT
)
LANGUAGE plpgsql
AS 
$$
DECLARE
	v_enrollment_id INT;
BEGIN
	BEGIN
		INSERT INTO enrollments (student_id, course_id, enroll_date)
		VALUES (p_student_id, p_course_id, CURRENT_DATE)
		RETURNING enrollment_id INTO v_enrollment_id;
		
		INSERT INTO certificates (enrollment_id, issue_date)
		VALUES (v_enrollment_id, CURRENT_DATE);

		RAISE NOTICE 'Student % enrolled and certified in Course % successfully.', p_student_id, p_course_id;
		
		EXCEPTION
		    WHEN OTHERS THEN
		        RAISE NOTICE 'Error: %, rolling back.', SQLERRM;
		    ROLLBACK;
	END;
END;
$$;
 

CALL sp_enroll_and_certify(5, 3);
CALL sp_enroll_and_certify(8, 3);  -- invalid student_id

CALL sp_enroll_and_certify(5, 1);


SELECT * FROM enrollments;
SELECT * FROM certificates;

