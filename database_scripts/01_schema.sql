CREATE TABLE address (
    address_id SERIAL PRIMARY KEY,
    city VARCHAR(255) NOT NULL,
    street_name VARCHAR(255) NOT NULL,
    street_number VARCHAR(255) NOT NULL
);

CREATE TABLE employee (
    id SERIAL PRIMARY KEY,
    full_name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    phone CHAR(255) NOT NULL,
    address_id INT NOT NULL,
    monthly_salary_usd INT NOT NULL,
    FOREIGN KEY (address_id) REFERENCES address (address_id)
);