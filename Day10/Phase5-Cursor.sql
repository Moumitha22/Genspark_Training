-- Phase 5: Cursor
-- Use a cursor to:
-- Loop through all students in a course
-- Print name and email of those who do not yet have certificates

CREATE OR REPLACE PROCEDURE sp_get_students_without_certificates(p_course_id INT)
LANGUAGE plpgsql
AS 
$$
DECLARE 
	student_rec RECORD;
	course_student_cur CURSOR FOR
		SELECT
            s.student_id,
			s.name,
			s.email,
			c.course_name,
            e.enrollment_id
        FROM 
			students s
        JOIN 
			enrollments e ON s.student_id = e.student_id
        JOIN 
			courses c ON e.course_id = c.course_id
        WHERE 
			e.course_id = p_course_id;
BEGIN
	OPEN course_student_cur;
	
	LOOP
		FETCH course_student_cur INTO student_rec;
		EXIT WHEN NOT FOUND;

		IF NOT EXISTS (SELECT 1 FROM certificates c WHERE c.enrollment_id = student_rec.enrollment_id) THEN
			RAISE NOTICE 'Student_Name : %, Email : %, Course: %', student_rec.name, student_rec.email, student_rec.course_name;
		END IF;
	END LOOP;

	CLOSE course_student_cur;
END;
$$;


CALL sp_get_students_without_certificates(3);
