CREATE TABLE address (
    address_id SERIAL PRIMARY KEY,
    city VARCHAR(80) NOT NULL,
    street_name VARCHAR(80) NOT NULL,
    street_number VARCHAR(10) NOT NULL
);

CREATE TABLE employee (
    id SERIAL PRIMARY KEY,
    full_name VARCHAR(80) NOT NULL,
    email VARCHAR(255) NOT NULL,
    phone CHAR(10) NOT NULL,
    address_id INT NOT NULL,
    monthly_salary_usd INT NOT NULL,
    FOREIGN KEY (address_id) REFERENCES address (address_id),
    CONSTRAINT check_phone_length CHECK (LENGTH(phone) = 10)
);