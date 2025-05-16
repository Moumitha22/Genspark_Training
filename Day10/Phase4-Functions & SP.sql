-- Phase 4: Functions & Stored Procedures

-- Function:
-- Create `get_certified_students(course_id INT)`
-- → Returns a list of students who completed the given course and received certificates.

CREATE OR REPLACE FUNCTION get_certified_students(p_course_id INT)
RETURNS TABLE(student_id INT, student_name VARCHAR, email VARCHAR, course_name VARCHAR, issue_date DATE) 
AS
$$
BEGIN
	RETURN QUERY
		SELECT 
			s.student_id,
			s.name,
			s.email,
			c.course_name,
			cert.issue_date
		FROM 
			certificates cert
		JOIN
			enrollments e ON cert.enrollment_id = e.enrollment_id
		JOIN 
			students s ON e.student_id = s.student_id
		JOIN 
			courses c ON e.course_id = c.course_id
		WHERE e.course_id = p_course_id
		ORDER BY e.student_id;
END;
$$
LANGUAGE plpgsql;

SELECT * FROM get_certified_students(2);

-- Stored Procedure:
-- Create `sp_enroll_student(p_student_id, p_course_id)`
-- → Inserts into `enrollments` and conditionally adds a certificate if completed (simulate with status flag).

CREATE OR REPLACE PROCEDURE sp_enroll_student(p_student_id INT, p_course_id INT, p_completed BOOLEAN)
LANGUAGE plpgsql
AS 
$$
DECLARE 
	v_enrollment_id INT;
BEGIN
	INSERT INTO enrollments(student_id, course_id)
	VALUES(p_student_id, p_course_id)
	RETURNING enrollment_id INTO v_enrollment_id;
	
	RAISE NOTICE 'Student % enrolled in course %.', p_student_id, p_course_id;

	IF p_completed THEN
        INSERT INTO certificates (enrollment_id, issue_date)
        VALUES (v_enrollment_id, CURRENT_DATE);
		
		RAISE NOTICE 'Student % obtained certificate in course %', p_student_id, p_course_id;
    END IF;
END;
$$;

CALL sp_enroll_student(1, 2, 't');
CALL sp_enroll_student(2, 1, 't');

SELECT * FROM enrollments;
SELECT * FROM certificates;
