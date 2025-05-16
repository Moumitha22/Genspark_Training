-- Phase 6: Security & Roles

-- 1. Create a `readonly_user` role:
-- * Can run `SELECT` on `students`, `courses`, and `certificates`
-- * Cannot `INSERT`, `UPDATE`, or `DELETE`

CREATE ROLE readonly_user NOINHERIT LOGIN PASSWORD 'readonly';
 
GRANT USAGE ON SCHEMA public TO readonly_user;
 
GRANT SELECT ON students, courses, certificates TO readonly_user;

-- 2. Create a `data_entry_user` role:
-- * Can `INSERT` into `students`, `enrollments`
-- * Cannot modify certificates directly
 
CREATE ROLE dataentry_user NOINHERIT LOGIN PASSWORD 'dataentry';

GRANT USAGE ON SCHEMA public TO dataentry_user;

GRANT INSERT ON students, enrollments TO dataentry_user;
 
REVOKE ALL ON certificates FROM dataentry_user;
 
GRANT USAGE, SELECT ON SEQUENCE students_student_id_seq TO dataentry_user;

GRANT USAGE, SELECT ON SEQUENCE enrollments_enrollment_id_seq TO dataentry_user;

-- Test
SELECT * FROM Students;
SELECT * FROM courses;
SELECT * FROM trainers;
SELECT * FROM course_trainers;
SELECT * FROM enrollments;
SELECT * FROM certificates;
 
INSERT INTO students (name, email, phone)
VALUES('Ria', 'ria@email.com', '9876543298');
 
INSERT INTO enrollments (student_id, course_id)
VALUES(8, 1);
 
INSERT INTO courses (course_name, category, duration_days)
VALUES('DevOps ', 'Cloud/DevOps', 60);
 
INSERT INTO certificates (enrollment_id, issue_date)
VALUES(11, '2025-01-10');
