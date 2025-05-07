USE pubs;
go

SELECT * FROM titles;

SELECT * FROM publishers;

-- JOINS

-- INNER JOIN
SELECT title, pub_name
FROM titles t
JOIN publishers p
ON t.pub_id = p.pub_id;

-- Print the details of publisher who never published
SELECT *
FROM publishers
WHERE pub_id NOT IN (SELECT DISTINCT pub_id FROM titles);

-- OUTER JOIN
SELECT title, pub_name
FROM titles t
RIGHT OUTER JOIN publishers p
ON t.pub_id = p.pub_id;

-- Print author_id and book name
SELECT au_id, title 
FROM titleauthor 
JOIN titles
ON titleauthor.title_id = titles.title_id
ORDER by title;

--or
SELECT au_id, title 
FROM titleauthor 
JOIN titles
ON titleauthor.title_id = titles.title_id
ORDER by 2; -- use cardinal

-- Print author name and title_id
SELECT CONCAT(au_fname,' ', au_lname) Author_Name, title Book_Name FROM authors a
JOIN titleauthor ta ON a.au_id = ta.au_id
JOIN titles t ON ta.title_id = t.title_id;

-- Print the publisher's name, book name and the order date of the  books
SELECT pub_name Publisher_Name, title Book_Name, ord_date Order_Date FROM publishers p
JOIN titles t ON p.pub_id = t.pub_id
JOIN sales s ON t.title_id = s.title_id
ORDER BY 3 DESC;

-- Print the publisher name and the first book sale date for all the publishers
SELECT pub_name Publisher_Name, MIN(ord_date) First_Order_Date FROM publishers p
LEFT OUTER JOIN titles t ON p.pub_id = t.pub_id
LEFT OUTER JOIN sales s ON t.title_id = s.title_id 
GROUP BY p.pub_name
ORDER BY 2 DESC;

-- Print the bookname and the store address of the sale
SELECT title Book_Name, stor_address Store_Address FROM titles t
JOIN sales s ON t.title_id = s.title_id
JOIN stores st ON s.stor_id = st.stor_id
ORDER BY 1;

SELECT title Book_Name, CONCAT(st.stor_address,',',st.city,',',st.state,',',st.zip) Store_Address FROM titles t
JOIN sales s ON t.title_id = s.title_id
JOIN stores st ON s.stor_id = st.stor_id
ORDER BY 1;

