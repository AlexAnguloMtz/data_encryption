CREATE TABLE app_user (
    id SERIAL PRIMARY KEY,
    username VARCHAR(15) UNIQUE,
    password VARCHAR(50)
);

CREATE TABLE state (
    id SERIAL PRIMARY KEY,
    name VARCHAR(30) UNIQUE
);

CREATE TABLE business_type (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) UNIQUE
);

CREATE TABLE address (
    id SERIAL PRIMARY KEY,
    state_id INT REFERENCES state(id),
    street_name VARCHAR(60),
    district_name VARCHAR(60),
    street_number VARCHAR(10),
    zip_code SMALLINT
);

CREATE TABLE business (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50), 
    business_type_id INT REFERENCES business_type(id),
    admin_user_id INT REFERENCES app_user(id),
    address_id INT REFERENCES address(id)
);
