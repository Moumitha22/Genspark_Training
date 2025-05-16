-- Phase 2: DDL & DML
-- Create all tables with appropriate constraints (PK, FK, UNIQUE, NOT NULL)

-- Students
CREATE TABLE students (
    student_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    phone VARCHAR(20)
);

 
-- Courses
CREATE TABLE courses (
    course_id SERIAL PRIMARY KEY,
    course_name VARCHAR(150) NOT NULL,
    category VARCHAR(100),
    duration_days INT
);
 
-- Trainers
CREATE TABLE trainers (
    trainer_id SERIAL PRIMARY KEY,
    trainer_name VARCHAR(100) NOT NULL,
    expertise VARCHAR(150)
);
 
 
-- Enrollments
CREATE TABLE enrollments (
    enrollment_id SERIAL PRIMARY KEY,
    student_id INT NOT NULL,
    course_id INT NOT NULL,
    enroll_date DATE DEFAULT CURRENT_DATE,
    FOREIGN KEY (student_id) REFERENCES students(student_id) ON DELETE CASCADE,
    FOREIGN KEY (course_id) REFERENCES courses(course_id) ON DELETE CASCADE,
    UNIQUE (student_id, course_id)
);
 
 
-- Certificates
CREATE TABLE certificates (
    certificate_id SERIAL PRIMARY KEY,
    enrollment_id INT UNIQUE NOT NULL,
    issue_date DATE,
    serial_no UUID DEFAULT gen_random_uuid(),
    FOREIGN KEY (enrollment_id) REFERENCES enrollments(enrollment_id) ON DELETE CASCADE
);
 
 
-- Course-Trainers
CREATE TABLE course_trainers (
    course_id INT NOT NULL,
    trainer_id INT NOT NULL,
    PRIMARY KEY (course_id, trainer_id),
    FOREIGN KEY (course_id) REFERENCES courses(course_id) ON DELETE CASCADE,
    FOREIGN KEY (trainer_id) REFERENCES trainers(trainer_id) ON DELETE CASCADE
);


-- Insert sample data using `INSERT` statements

-- Students
INSERT INTO students (name, email, phone) VALUES
('Moumi', 'moumi@email.com', '9876543210'),
('Megha', 'megha@email.com', '9876543211'),
('Dhanu', 'dhanu@email.com', '9876543212');

SELECT * FROM students;

-- Courses
INSERT INTO courses (course_name, category, duration_days) VALUES
('SQL Basics', 'Database', 15),
('Angular', 'Programming', 15),
('C# Fundamentals', 'Programming', 30);

INSERT INTO courses (course_name, category, duration_days) VALUES
('React', 'Programming', 15),
('Java Fundamentals', 'Programming', 30),
('Ethical Hacking', 'Cybersecurity', 45);

SELECT * FROM courses;

-- Trainers
INSERT INTO trainers (trainer_name, expertise) VALUES
('Priya', 'SQL, PostgreSQL'),
('Gokul', 'Angular, React'),
('Oswalt', 'C#, .NET');

INSERT INTO trainers (trainer_name, expertise) VALUES
('Mano', 'Java, SpringBoot');

SELECT * FROM trainers;

-- Enrollments
INSERT INTO enrollments (student_id, course_id) VALUES
(1, 1),
(2, 2),
(3, 1),
(1, 3);
 
SELECT * FROM enrollments;

-- Certificates
INSERT INTO certificates (enrollment_id, issue_date) VALUES
(1, '2025-06-01'),
(2, '2025-06-01'),
(3, '2025-06-01');
 
SELECT * FROM certificates;

-- Course-Trainers
INSERT INTO course_trainers (course_id, trainer_id) VALUES
(1, 1),
(2, 2),
(3, 3);

INSERT INTO course_trainers (course_id, trainer_id) VALUES
(4, 2),
(5, 4);
 
-- Create indexes on `student_id`, `email`, and `course_id`

CREATE INDEX idx_students_student_id ON students(student_id);
CREATE INDEX idx_students_email ON students(email);
CREATE INDEX idx_courses_course_id ON courses(course_id);

-----------------------------------------------------------
