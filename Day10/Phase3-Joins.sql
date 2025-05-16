
-- Phase 3: SQL Joins Practice

-- Write queries to:

-- 1. List students and the courses they enrolled in
SELECT name AS student_name, course_name
FROM
    enrollments e
JOIN
    students s ON e.student_id = s.student_id
JOIN
    courses c ON e.course_id = c.course_id;
 
 
-- 2. Show students who received certificates with trainer names
SELECT
	s.name AS student_name,
    c.course_name,
    t.trainer_name,
    cert.issue_date
FROM
    certificates cert
JOIN
    enrollments e ON cert.enrollment_id = e.enrollment_id
JOIN
    students s ON e.student_id = s.student_id
JOIN
    courses c ON e.course_id = c.course_id
JOIN
    course_trainers ct ON c.course_id = ct.course_id
JOIN
    trainers t ON ct.trainer_id = t.trainer_id
ORDER BY cert.issue_date;
	
-- 3. Count number of students per course
SELECT course_name, COUNT(e.student_id) AS student_count
FROM
	courses c
LEFT JOIN    
    enrollments e ON e.course_id = c.course_id
GROUP BY course_name;
 