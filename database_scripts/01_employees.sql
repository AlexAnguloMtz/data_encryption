INSERT INTO address(city, street_name, street_number)
VALUES
    ('Ciudad de México', 'Avenida Juárez', '123'),
    ('Guadalajara', 'Calle Morelos', '456-B'), 
    ('Monterrey', 'Avenida Hidalgo', '789'),
    ('Puebla', 'Calle Reforma', '101-C'), 
    ('Tijuana', 'Avenida Revolución', '202');

INSERT INTO employee(full_name, email, phone, monthly_salary_usd, address_id)
VALUES
    ('Juan Pérez López', 'juan.perez@gmail.com', '6621234567', 1000, 1),
    ('María González Rodríguez', 'maria.gonzalez@hotmail.com', '6622345678', 2000, 2),
    ('José Rodríguez Martínez', 'jose.rodriguez@outlook.com', '6623456789', 1750, 3),
    ('Ana López García', 'ana.lopez@yandex.com', '6624567890', 4000, 4),
    ('Carlos Martínez Hernández', 'carlos.martinez@yahoo.com', '6625678901', 1300, 5);
