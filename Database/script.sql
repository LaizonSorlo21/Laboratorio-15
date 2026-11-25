-- Script de creación de base de datos: VeterinariaDb
-- Dos entidades con relación 1:N (Propietario 1 -- N Mascota), clave foránea + propiedad de navegación.

IF DB_ID('VeterinariaDb') IS NULL
BEGIN
    CREATE DATABASE VeterinariaDb;
END
GO

USE VeterinariaDb;
GO

IF OBJECT_ID('dbo.Usuarios', 'U') IS NOT NULL DROP TABLE dbo.Usuarios;
IF OBJECT_ID('dbo.Mascotas', 'U') IS NOT NULL DROP TABLE dbo.Mascotas;
IF OBJECT_ID('dbo.Propietarios', 'U') IS NOT NULL DROP TABLE dbo.Propietarios;
GO

CREATE TABLE dbo.Propietarios
(
    Id          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nombre      NVARCHAR(60)   NOT NULL,
    Apellido    NVARCHAR(60)   NOT NULL,
    Email       NVARCHAR(120)  NOT NULL,
    Telefono    NVARCHAR(20)   NULL,
    Direccion   NVARCHAR(150)  NULL,
    CONSTRAINT UQ_Propietarios_Email UNIQUE (Email)
);
GO

CREATE TABLE dbo.Mascotas
(
    Id               INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nombre           NVARCHAR(60)    NOT NULL,
    Especie          NVARCHAR(40)    NOT NULL,
    Raza             NVARCHAR(40)    NULL,
    FechaNacimiento  DATETIME2       NOT NULL,
    Peso             DECIMAL(6,2)    NOT NULL,
    PropietarioId    INT             NOT NULL,
    CONSTRAINT FK_Mascotas_Propietarios
        FOREIGN KEY (PropietarioId) REFERENCES dbo.Propietarios(Id)
);
GO

-- Datos de ejemplo
INSERT INTO dbo.Propietarios (Nombre, Apellido, Email, Telefono, Direccion) VALUES
    ('Ana',    'Gomez',    'ana.gomez@example.com',    '987654321', 'Av. Los Alamos 123'),
    ('Luis',   'Perez',    'luis.perez@example.com',   '976543210', 'Jr. Las Flores 456'),
    ('Maria',  'Torres',   'maria.torres@example.com', '965432109', 'Calle Las Palmeras 789');
GO

INSERT INTO dbo.Mascotas (Nombre, Especie, Raza, FechaNacimiento, Peso, PropietarioId) VALUES
    ('Rocky',   'Perro', 'Labrador',        '2021-03-15', 28.50, 1),
    ('Michi',   'Gato',  'Siames',          '2022-07-02', 4.20,  1),
    ('Firulais','Perro', 'Mestizo',         '2020-11-20', 15.00, 2),
    ('Pelusa',  'Gato',  'Persa',           '2023-01-10', 3.80,  2),
    ('Nemo',    'Pez',   'Betta',           '2023-05-05', 0.10,  3);
GO

-- ============================================================
-- Semana 14: autenticación JWT. Tabla Usuarios (Correo único, ClaveHash BCrypt, Rol).
-- Roles propios de la veterinaria: Veterinario y Recepcionista.
-- Las claves de prueba NO están en texto plano: solo el hash BCrypt (generado con Database/HashGenerator).
--   veterinario@veterinaria.com  /  Vet#2026!Clave
--   recepcion@veterinaria.com    /  Recep#2026!Clave
-- (Este bloque puede ejecutarse solo, sin tocar Propietarios ni Mascotas.)
-- ============================================================
IF OBJECT_ID('dbo.Usuarios', 'U') IS NOT NULL DROP TABLE dbo.Usuarios;
GO

CREATE TABLE dbo.Usuarios
(
    Id         INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Correo     NVARCHAR(120)  NOT NULL,
    ClaveHash  NVARCHAR(100)  NOT NULL,
    Rol        NVARCHAR(30)   NOT NULL,
    CONSTRAINT UQ_Usuarios_Correo UNIQUE (Correo),
    CONSTRAINT CK_Usuarios_Rol CHECK (Rol IN ('Veterinario', 'Recepcionista'))
);
GO

INSERT INTO dbo.Usuarios (Correo, ClaveHash, Rol) VALUES
    ('veterinario@veterinaria.com', '$2a$11$M6N4t48n1pQ4VL6RCCxe/uN7KMzX.NXpw.PkzkrzFCoaTXYT4H4U2', 'Veterinario'),
    ('recepcion@veterinaria.com',   '$2a$11$LbYQe3Y0P1IwF07jPQDMOepwtqvBjGtgEO9ebhONPPyQkr7UtzuiS', 'Recepcionista');
GO
