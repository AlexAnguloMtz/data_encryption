CREATE TABLE app_user (
    id SERIAL PRIMARY KEY,
    username VARCHAR(15) UNIQUE,
    password VARCHAR(50)
);

CREATE TABLE state (
    id CHAR(3) PRIMARY KEY,
    name VARCHAR(30) UNIQUE
);

CREATE TABLE municipality (
    id SERIAL PRIMARY KEY,
    name VARCHAR(60),
    state_id CHAR(3) REFERENCES state(id)
);

CREATE TABLE business_type (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) UNIQUE
);

CREATE TABLE address (
    id SERIAL PRIMARY KEY,
    street_name VARCHAR(60),
    district_name VARCHAR(60),
    street_number VARCHAR(10),
    zip_code CHAR(5),
    municipality_id INTEGER REFERENCES municipality(id)
);

CREATE TABLE business (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50), 
    business_type_id INT REFERENCES business_type(id),
    admin_user_id INT REFERENCES app_user(id),
    address_id INT REFERENCES address(id)
);
