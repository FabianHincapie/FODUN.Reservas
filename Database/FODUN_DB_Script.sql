-- =============================================
-- SISTEMA DE RESERVAS FODUN
-- Base de Datos: Microsoft SQL Server
-- Versión: 1.0
-- =============================================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'FODUN_Reservas')
    DROP DATABASE FODUN_Reservas;
GO

CREATE DATABASE FODUN_Reservas
    COLLATE Modern_Spanish_CI_AS;
GO

USE FODUN_Reservas;
GO

-- =============================================
-- 1. TABLA: TipoSede
--    Categoriza entre Sede Recreativa y Apartamento
-- =============================================
CREATE TABLE TipoSede (
    TipoSedeId   INT IDENTITY(1,1) PRIMARY KEY,
    Nombre       NVARCHAR(50)  NOT NULL,   -- 'Sede Recreativa' | 'Apartamento'
    Descripcion  NVARCHAR(200) NULL
);
GO

-- =============================================
-- 2. TABLA: Sedes
--    Todas las sedes (recreativas + edificios de aptos)
-- =============================================
CREATE TABLE Sedes (
    SedeId          INT IDENTITY(1,1) PRIMARY KEY,
    TipoSedeId      INT           NOT NULL,
    Nombre          NVARCHAR(100) NOT NULL,
    NombreCorto     NVARCHAR(50)  NOT NULL,
    Ciudad          NVARCHAR(100) NOT NULL,
    Departamento    NVARCHAR(100) NOT NULL,
    Direccion       NVARCHAR(200) NULL,
    Descripcion     NVARCHAR(MAX) NULL,
    CapacidadTotal  INT           NOT NULL DEFAULT 0,
    ImagenPrincipal NVARCHAR(200) NULL,
    Activa          BIT           NOT NULL DEFAULT 1,
    CONSTRAINT FK_Sedes_TipoSede FOREIGN KEY (TipoSedeId)
        REFERENCES TipoSede(TipoSedeId)
);
GO

-- =============================================
-- 3. TABLA: Alojamientos
--    Cada habitación o apartamento dentro de una sede
-- =============================================
CREATE TABLE Alojamientos (
    AlojamientoId    INT IDENTITY(1,1) PRIMARY KEY,
    SedeId           INT           NOT NULL,
    Numero           NVARCHAR(20)  NOT NULL,   -- '1','2','301','Hab 5', etc.
    Nombre           NVARCHAR(100) NULL,        -- Nombre descriptivo opcional
    Descripcion      NVARCHAR(MAX) NULL,
    NumHabitaciones  INT           NOT NULL DEFAULT 1,
    CapacidadMax     INT           NOT NULL,
    TieneBano        BIT           NOT NULL DEFAULT 1,
    TieneCocineta    BIT           NOT NULL DEFAULT 0,
    TieneTelevision  BIT           NOT NULL DEFAULT 0,
    TieneNevera      BIT           NOT NULL DEFAULT 0,
    TieneTerraza     BIT           NOT NULL DEFAULT 0,
    TieneSalaEstar   BIT           NOT NULL DEFAULT 0,
    TieneParqueadero BIT           NOT NULL DEFAULT 0,
    Activo           BIT           NOT NULL DEFAULT 1,
    CONSTRAINT FK_Alojamientos_Sedes FOREIGN KEY (SedeId)
        REFERENCES Sedes(SedeId)
);
GO

-- =============================================
-- 4. TABLA: Temporadas
--    Alta, Baja, Especial (Lunes-Jueves sin festivos)
-- =============================================
CREATE TABLE Temporadas (
    TemporadaId  INT IDENTITY(1,1) PRIMARY KEY,
    Nombre       NVARCHAR(50)  NOT NULL,  -- 'Alta','Baja','Especial'
    Descripcion  NVARCHAR(200) NULL,
    Activa       BIT           NOT NULL DEFAULT 1
);
GO

-- =============================================
-- 5. TABLA: FechasTemporada
--    Rangos de fechas que determinan la temporada activa
-- =============================================
CREATE TABLE FechasTemporada (
    FechaTemporadaId INT IDENTITY(1,1) PRIMARY KEY,
    TemporadaId      INT  NOT NULL,
    FechaInicio      DATE NOT NULL,
    FechaFin         DATE NOT NULL,
    Anio             INT  NOT NULL,
    CONSTRAINT FK_FechasTemporada_Temporadas FOREIGN KEY (TemporadaId)
        REFERENCES Temporadas(TemporadaId)
);
GO

-- =============================================
-- 6. TABLA: Tarifas
--    Precio según sede, temporada y número de habitaciones
-- =============================================
CREATE TABLE Tarifas (
    TarifaId         INT IDENTITY(1,1) PRIMARY KEY,
    SedeId           INT            NOT NULL,
    TemporadaId      INT            NOT NULL,
    NumHabitaciones  INT            NOT NULL DEFAULT 1,  -- 1 o 2
    PersonasMin      INT            NOT NULL DEFAULT 1,
    PersonasMax      INT            NOT NULL,
    ValorNoche       DECIMAL(12,2)  NOT NULL,
    ValorPersonaAdicional DECIMAL(12,2) NOT NULL DEFAULT 0,
    Activa           BIT            NOT NULL DEFAULT 1,
    CONSTRAINT FK_Tarifas_Sedes      FOREIGN KEY (SedeId)      REFERENCES Sedes(SedeId),
    CONSTRAINT FK_Tarifas_Temporadas FOREIGN KEY (TemporadaId) REFERENCES Temporadas(TemporadaId)
);
GO

-- =============================================
-- 7. TABLA: Usuarios
--    Asociados / usuarios del sistema
-- =============================================
CREATE TABLE Usuarios (
    UsuarioId             INT IDENTITY(1,1) PRIMARY KEY,
    NroDocumento          NVARCHAR(20)  NOT NULL UNIQUE,
    NombreCompleto        NVARCHAR(150) NOT NULL,
    FechaNacimiento       DATE          NULL,
    Celular               NVARCHAR(20)  NULL,
    Email                 NVARCHAR(150) NOT NULL UNIQUE,
    Departamento          NVARCHAR(100) NULL,
    Municipio             NVARCHAR(100) NULL,
    Barrio                NVARCHAR(100) NULL,
    DireccionResidencia   NVARCHAR(200) NULL,
    TelefonoResidencia    NVARCHAR(20)  NULL,
    PreguntaSecreta       NVARCHAR(200) NULL,
    RespuestaSecreta      NVARCHAR(200) NULL,  -- Se guarda con hash
    AutorizaCorreo        BIT           NOT NULL DEFAULT 1,
    AutorizaCelular       BIT           NOT NULL DEFAULT 1,
    PasswordHash          NVARCHAR(MAX) NOT NULL,
    PasswordSalt          NVARCHAR(MAX) NULL,
    TokenRecuperacion     NVARCHAR(MAX) NULL,
    TokenExpiracion       DATETIME      NULL,
    FechaRegistro         DATETIME      NOT NULL DEFAULT GETDATE(),
    Activo                BIT           NOT NULL DEFAULT 1
);
GO

-- =============================================
-- 8. TABLA: Reservas
--    Cabecera de cada reserva
-- =============================================
CREATE TABLE Reservas (
    ReservaId          INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId          INT           NOT NULL,
    SedeId             INT           NOT NULL,
    FechaReserva       DATETIME      NOT NULL DEFAULT GETDATE(),
    FechaLlegada       DATE          NOT NULL,
    FechaSalida        DATE          NOT NULL,
    NumPersonas        INT           NOT NULL,
    NumHabitaciones    INT           NOT NULL DEFAULT 1,
    ServicioLavanderia BIT           NOT NULL DEFAULT 0,
    TemporadaId        INT           NOT NULL,
    ValorTotal         DECIMAL(12,2) NOT NULL DEFAULT 0,
    Estado             NVARCHAR(20)  NOT NULL DEFAULT 'Pendiente',
    -- Estado: Pendiente | Confirmada | Cancelada | Completada
    Observaciones      NVARCHAR(500) NULL,
	ComprobanteEnviado BIT           NOT NULL DEFAULT 0,
    CONSTRAINT FK_Reservas_Usuarios   FOREIGN KEY (UsuarioId)   REFERENCES Usuarios(UsuarioId),
    CONSTRAINT FK_Reservas_Sedes      FOREIGN KEY (SedeId)      REFERENCES Sedes(SedeId),
    CONSTRAINT FK_Reservas_Temporadas FOREIGN KEY (TemporadaId) REFERENCES Temporadas(TemporadaId)
);
GO

-- =============================================
-- 9. TABLA: DetalleReserva
--    Qué alojamientos/habitaciones incluye cada reserva
-- =============================================
CREATE TABLE DetalleReserva (
    DetalleId     INT IDENTITY(1,1) PRIMARY KEY,
    ReservaId     INT           NOT NULL,
    AlojamientoId INT           NOT NULL,
    ValorNoche    DECIMAL(12,2) NOT NULL,
    NumNoches     INT           NOT NULL,
    SubTotal      DECIMAL(12,2) NOT NULL,
    CONSTRAINT FK_Detalle_Reservas     FOREIGN KEY (ReservaId)     REFERENCES Reservas(ReservaId),
    CONSTRAINT FK_Detalle_Alojamientos FOREIGN KEY (AlojamientoId) REFERENCES Alojamientos(AlojamientoId)
);
GO

-- =============================================
-- ÍNDICES para optimizar consultas de disponibilidad
-- =============================================
CREATE INDEX IX_Reservas_SedeId_Fechas
    ON Reservas(SedeId, FechaLlegada, FechaSalida, Estado);

CREATE INDEX IX_DetalleReserva_Alojamiento
    ON DetalleReserva(AlojamientoId);

CREATE INDEX IX_Reservas_UsuarioId
    ON Reservas(UsuarioId);

CREATE INDEX IX_FechasTemporada_Anio
    ON FechasTemporada(Anio, FechaInicio, FechaFin);
GO

-- =============================================
-- =============================================
-- DATOS INICIALES (SEED DATA)
-- =============================================
-- =============================================

-- Tipos de Sede
INSERT INTO TipoSede (Nombre, Descripcion) VALUES
('Sede Recreativa', 'Sedes recreativas con múltiples alojamientos y servicios deportivos'),
('Apartamento',     'Edificios de apartamentos para uso de los asociados');
GO

-- Sedes Recreativas
INSERT INTO Sedes (TipoSedeId, Nombre, NombreCorto, Ciudad, Departamento, Descripcion, CapacidadTotal) VALUES
(1, 'Sede Recreativa Villeta',           'Villeta',       'Villeta',               'Cundinamarca',
 'Sede recreativa ubicada en el barrio San Jorge, a poca distancia de la plaza central de Villeta en la Provincia del Gualivá, Cundinamarca. Distante de Bogotá 90 kilómetros, aproximadamente hora y media por la autopista Bogotá-Medellín.', 32),

(1, 'Sede Recreativa El Placer',         'El Placer',     'Fusagasugá',            'Cundinamarca',
 'Sede recreativa ubicada en la vereda El Placer del municipio de Fusagasugá, a unos 10 minutos del casco urbano.', 34),

(1, 'Sede Recreativa Gonzalo Morante',   'Chinchiná',     'Chinchiná',             'Caldas',
 'Sede recreativa Gonzalo Morante ubicada en Chinchiná, Caldas.', 30),

(1, 'Sede Recreativa Tablones',          'Palmira',       'Palmira',               'Valle del Cauca',
 'Sede recreativa Tablones en Palmira, Valle del Cauca.', 24),

(1, 'Sede Recreativa Manguruma',         'Sta Fé Antioquia', 'Santa Fe de Antioquia', 'Antioquia',
 'Sede recreativa Manguruma en Santa Fe de Antioquia.', 46),

(1, 'Sede Recreativa Federman',          'Bogotá',        'Bogotá',                'Cundinamarca',
 'Sede recreativa Federman en Bogotá. Cuenta con zona húmeda, gimnasio, sala de masajes, billar y juegos de mesa.', 0),

-- Apartamentos
(2, 'Edificio Suramericana - Medellín',  'Suramericana',  'Medellín',              'Antioquia',
 'Ubicado en la Calle 49B N° 64B-15 en el edificio Suramericana N° 6 Apartamento 1204. Cerca del campus de la Universidad Nacional de Colombia.', 0),

(2, 'Edificio Reina 1 - Santa Marta',   'Rodadero',      'Santa Marta',           'Magdalena',
 'Ubicados en el edificio REINA 1 de la Carrera 3 número 7-85 centro urbano y turístico El Rodadero, a tres cuadras de la playa.', 0);
GO

-- =============================================
-- Alojamientos - Villeta (8 habitaciones)
-- =============================================
INSERT INTO Alojamientos (SedeId, Numero, Nombre, NumHabitaciones, CapacidadMax, TieneBano, TieneTelevision, TieneNevera, TieneTerraza, Descripcion)
SELECT
    s.SedeId, CAST(n.Numero AS NVARCHAR(20)),
    'Habitación ' + CAST(n.Numero AS NVARCHAR(20)),
    1, 4, 1, 1, 1, 1,
    'Habitación con cama doble y camarote, baño privado, nevera, televisor y terraza cubierta.'
FROM Sedes s
CROSS JOIN (VALUES (1),(2),(3),(4),(5),(6),(7),(8)) AS n(Numero)
WHERE s.NombreCorto = 'Villeta';
GO

-- =============================================
-- Alojamientos - El Placer, Fusagasugá
-- =============================================
DECLARE @SedeElPlacer INT = (SELECT SedeId FROM Sedes WHERE NombreCorto = 'El Placer');

INSERT INTO Alojamientos (SedeId, Numero, Nombre, NumHabitaciones, CapacidadMax, TieneBano, TieneTelevision, TieneCocineta, TieneSalaEstar, Descripcion) VALUES
(@SedeElPlacer,'1','Alojamiento 1', 2, 4, 1, 1, 0, 0, 'Dos habitaciones, baño y TV. Una con cama doble y sencilla, otra con una cama sencilla.'),
(@SedeElPlacer,'2','Alojamiento 2', 2, 6, 1, 1, 0, 0, 'Dos habitaciones, baño y TV. Una con cama doble, otra con 4 camas sencillas.'),
(@SedeElPlacer,'3','Alojamiento 3', 1, 4, 1, 1, 0, 0, 'Una habitación con cama doble y 2 camas sencillas, baño y TV.'),
(@SedeElPlacer,'4','Alojamiento 4', 2, 4, 1, 1, 0, 0, 'Dos habitaciones, baño y TV. Una con cama doble y sencilla, otra con una cama sencilla.'),
-- Cabañas 5-8 (bloque nuevo)
(@SedeElPlacer,'5','Cabaña 5', 1, 4, 1, 1, 1, 1, 'Cabaña: sala de estar con sofá-cama y TV, baño, habitación con cama doble y sencilla, cocineta equipada, terraza comedor.'),
(@SedeElPlacer,'6','Cabaña 6', 1, 4, 1, 1, 1, 1, 'Cabaña: sala de estar con sofá-cama y TV, baño, habitación con cama doble y sencilla, cocineta equipada, terraza comedor.'),
(@SedeElPlacer,'7','Cabaña 7', 1, 4, 1, 1, 1, 1, 'Cabaña: sala de estar con sofá-cama y TV, baño, habitación con cama doble y sencilla, cocineta equipada, terraza comedor.'),
(@SedeElPlacer,'8','Cabaña 8', 1, 4, 1, 1, 1, 1, 'Cabaña: sala de estar con sofá-cama y TV, baño, habitación con cama doble y sencilla, cocineta equipada, terraza comedor.');
GO

-- =============================================
-- Alojamientos - Gonzalo Morante, Chinchiná
-- =============================================
DECLARE @SedeChinchina INT = (SELECT SedeId FROM Sedes WHERE NombreCorto = 'Chinchiná');

INSERT INTO Alojamientos (SedeId, Numero, Nombre, NumHabitaciones, CapacidadMax, TieneBano, TieneTelevision, TieneCocineta, Descripcion) VALUES
(@SedeChinchina,'1','Alojamiento 1', 2, 6, 1, 1, 1, 'Cocineta, baño, TV y 2 habitaciones. Hab1: 2 camas sencillas + 2 adicionales. Hab2: cama doble y sencilla.'),
(@SedeChinchina,'2','Alojamiento 2', 2, 6, 1, 1, 1, 'Cocineta, baño, TV y 2 habitaciones. Hab1: cama doble + auxiliar doble. Hab2: 2 camas sencillas + 2 auxiliares.'),
(@SedeChinchina,'3','Alojamiento 3 (Tipo A)', 2, 6, 1, 1, 1, 'Cocineta, 2 baños, sala comedor, TV y 2 habitaciones. Hab1: cama doble. Hab2: 2 camas sencillas + 2 auxiliares.'),
(@SedeChinchina,'4','Alojamiento 4', 1, 3, 1, 1, 1, 'Cocineta, baño, TV y una habitación con cama doble y sencilla.'),
(@SedeChinchina,'5','Alojamiento 5 (Tipo B)', 1, 3, 1, 1, 1, 'Cocineta, baño, sala con sofá, TV, habitación con cama doble y sencilla.'),
(@SedeChinchina,'6','Alojamiento 6 (Tipo B)', 1, 3, 1, 1, 1, 'Cocineta, baño, sala con sofá, TV, habitación con cama doble y sencilla.');
GO

-- =============================================
-- Alojamientos - Tablones, Palmira
-- =============================================
DECLARE @SedePalmira INT = (SELECT SedeId FROM Sedes WHERE NombreCorto = 'Palmira');

INSERT INTO Alojamientos (SedeId, Numero, Nombre, NumHabitaciones, CapacidadMax, TieneBano, TieneTelevision, TieneCocineta, TieneSalaEstar, Descripcion) VALUES
(@SedePalmira,'1','Alojamiento 1', 1, 4, 1, 1, 1, 0, 'Habitación con cama doble y camarote. TV, baño, cocineta con nevera, comedor.'),
(@SedePalmira,'2','Alojamiento 2', 1, 4, 1, 1, 1, 0, 'Habitación con cama doble y camarote. TV, baño, cocineta con nevera, comedor.'),
(@SedePalmira,'3','Alojamiento 3', 2, 8, 1, 1, 1, 1, 'Dos habitaciones. Hab1: cama doble y camarote. Hab2: dos camarotes. Sala de estar con TV, baño y cocineta.'),
(@SedePalmira,'4','Alojamiento 4', 2, 8, 1, 1, 1, 1, 'Dos habitaciones. Hab1: cama doble y camarote. Hab2: dos camarotes. Sala de estar con TV, baño y cocineta.');
GO

-- =============================================
-- Alojamientos - Manguruma, Santa Fe de Antioquia
-- =============================================
DECLARE @SedeManguruma INT = (SELECT SedeId FROM Sedes WHERE NombreCorto = 'Sta Fé Antioquia');

INSERT INTO Alojamientos (SedeId, Numero, Nombre, NumHabitaciones, CapacidadMax, TieneBano, TieneTelevision, TieneTerraza, Descripcion) VALUES
(@SedeManguruma,'1','Alojamiento 1', 1, 3, 1, 1, 1, 'Cama doble, camarote. Baño y terraza. TV.'),
(@SedeManguruma,'2','Alojamiento 2', 1, 5, 1, 1, 1, 'Cama doble, camarote y sofá-cama. Baño y terraza. TV.'),
(@SedeManguruma,'3','Alojamiento 3', 1, 5, 1, 1, 1, 'Cama doble, camarote y sofá-cama. Baño y terraza. TV.');

-- Bloque Nuevo (8 alojamientos: N4 a N11)
INSERT INTO Alojamientos (SedeId, Numero, Nombre, NumHabitaciones, CapacidadMax, TieneBano, TieneTelevision, TieneTerraza, TieneCocineta, TieneNevera, Descripcion)
SELECT
    @SedeManguruma, 'N' + CAST(n.Num AS NVARCHAR(3)),
    'Bloque Nuevo - Alojamiento ' + CAST(n.Num AS NVARCHAR(3)),
    1, 4, 1, 1, 1, 1, 1,
    'Habitación con dos camas gemelas y camarote; baño, terraza-comedor y cocina. Nevera y TV.'
FROM (VALUES (1),(2),(3),(4),(5),(6),(7),(8)) AS n(Num);
GO

-- =============================================
-- Alojamientos - Suramericana, Medellín
-- =============================================
DECLARE @SedeSuramericana INT = (SELECT SedeId FROM Sedes WHERE NombreCorto = 'Suramericana');

INSERT INTO Alojamientos (SedeId, Numero, Nombre, NumHabitaciones, CapacidadMax, TieneBano, Descripcion) VALUES
(@SedeSuramericana,'Hab1','Habitación 1', 1, 2, 1, '2 camas sencillas y baño privado.'),
(@SedeSuramericana,'Hab2','Habitación 2', 1, 2, 0, '2 camas sencillas.'),
(@SedeSuramericana,'Hab3','Habitación 3', 1, 2, 0, '2 camas sencillas.'),
(@SedeSuramericana,'Hab4','Habitación 4', 1, 2, 0, '2 camas sencillas.'),
(@SedeSuramericana,'Hab5','Habitación 5', 1, 1, 1, '1 cama sencilla y baño privado.');
GO

-- =============================================
-- Alojamientos - El Rodadero, Santa Marta
-- =============================================
DECLARE @SedeRodadero INT = (SELECT SedeId FROM Sedes WHERE NombreCorto = 'Rodadero');

INSERT INTO Alojamientos (SedeId, Numero, Nombre, NumHabitaciones, CapacidadMax, TieneBano, TieneParqueadero, TieneSalaEstar, TieneCocineta, Descripcion) VALUES
(@SedeRodadero,'202','Apartamento 202', 3, 8, 1, 1, 1, 1, 'Sala comedor, cocina, 2 baños, 3 habitaciones y parqueadero. Capacidad máxima: 8 personas.'),
(@SedeRodadero,'301','Apartamento 301', 2, 6, 1, 1, 1, 1, 'Sala comedor, cocina, 1 baño, 2 habitaciones y parqueadero. Capacidad máxima: 6 personas.'),
(@SedeRodadero,'401','Apartamento 401', 2, 6, 1, 1, 1, 1, 'Sala comedor, cocina, 1 baño, 2 habitaciones y parqueadero. Capacidad máxima: 6 personas.');
GO

-- =============================================
-- Temporadas
-- =============================================
INSERT INTO Temporadas (Nombre, Descripcion) VALUES
('Baja',     'Temporada baja: días ordinarios (aplica tarifa ordinaria).'),
('Alta',     'Temporada alta: festivos, semana escolar, vacaciones de junio-julio y diciembre-enero.'),
('Especial', 'Tarifa especial: lunes a jueves, excepto festivos, semana escolar y alta temporada.');
GO

-- Fechas de temporada (ejemplo año 2025-2026)
-- Temporada Alta 2025
DECLARE @TempAlta INT     = (SELECT TemporadaId FROM Temporadas WHERE Nombre = 'Alta');
DECLARE @TempBaja INT     = (SELECT TemporadaId FROM Temporadas WHERE Nombre = 'Baja');
DECLARE @TempEspecial INT = (SELECT TemporadaId FROM Temporadas WHERE Nombre = 'Especial');

INSERT INTO FechasTemporada (TemporadaId, FechaInicio, FechaFin, Anio) VALUES
-- Temporada Alta 2025
(@TempAlta, '2025-01-01', '2025-01-13', 2025),  -- Año Nuevo
(@TempAlta, '2025-06-15', '2025-07-31', 2025),  -- Vacaciones junio-julio
(@TempAlta, '2025-12-20', '2025-12-31', 2025),  -- Navidad
-- Temporada Alta 2026
(@TempAlta, '2026-01-01', '2026-01-12', 2026),
(@TempAlta, '2026-06-20', '2026-07-31', 2026),
(@TempAlta, '2026-12-20', '2026-12-31', 2026);
GO

-- =============================================
-- Tarifas
-- =============================================
DECLARE @SedeVilleta     INT = (SELECT SedeId FROM Sedes WHERE NombreCorto = 'Villeta');
DECLARE @SedeElPlacer2   INT = (SELECT SedeId FROM Sedes WHERE NombreCorto = 'El Placer');
DECLARE @SedeChinchina2  INT = (SELECT SedeId FROM Sedes WHERE NombreCorto = 'Chinchiná');
DECLARE @SedePalmira2    INT = (SELECT SedeId FROM Sedes WHERE NombreCorto = 'Palmira');
DECLARE @SedeManguruma2  INT = (SELECT SedeId FROM Sedes WHERE NombreCorto = 'Sta Fé Antioquia');
DECLARE @SedeSuramer2    INT = (SELECT SedeId FROM Sedes WHERE NombreCorto = 'Suramericana');
DECLARE @SedeRodadero2   INT = (SELECT SedeId FROM Sedes WHERE NombreCorto = 'Rodadero');

DECLARE @TB INT = (SELECT TemporadaId FROM Temporadas WHERE Nombre = 'Baja');
DECLARE @TA INT = (SELECT TemporadaId FROM Temporadas WHERE Nombre = 'Alta');
DECLARE @TE INT = (SELECT TemporadaId FROM Temporadas WHERE Nombre = 'Especial');

-- ---- Sedes Recreativas (Villeta, El Placer, Chinchiná, Palmira, Manguruma) ----
-- Temporada Baja/Alta: 1 habitación -> $70.000, 2 habitaciones -> $90.000
-- Persona adicional (>4): $16.000
-- Temporada Especial: 1 hab -> $27.000, 2 habs -> $37.000, adicional $11.000

-- Se insertan para cada sede recreativa
-- Usamos un cursor simplificado con VALUES
;WITH SedesRecreativas AS (
    SELECT SedeId FROM Sedes
    WHERE NombreCorto IN ('Villeta','El Placer','Chinchiná','Palmira','Sta Fé Antioquia')
)
INSERT INTO Tarifas (SedeId, TemporadaId, NumHabitaciones, PersonasMin, PersonasMax, ValorNoche, ValorPersonaAdicional)
SELECT s.SedeId, t.TemporadaId, t.NumHabs, 1, 4, t.Valor, t.Adicional
FROM SedesRecreativas s
CROSS JOIN (VALUES
    (@TB, 1, 70000.00, 16000.00),
    (@TB, 2, 90000.00, 16000.00),
    (@TA, 1, 70000.00, 16000.00),
    (@TA, 2, 90000.00, 16000.00),
    (@TE, 1, 27000.00, 11000.00),
    (@TE, 2, 37000.00, 11000.00)
) AS t(TemporadaId, NumHabs, Valor, Adicional);
GO

-- ---- Apartamentos Suramericana Medellín ----
DECLARE @SurameId INT = (SELECT SedeId FROM Sedes WHERE NombreCorto = 'Suramericana');
DECLARE @TBId INT     = (SELECT TemporadaId FROM Temporadas WHERE Nombre = 'Baja');
DECLARE @TAId INT     = (SELECT TemporadaId FROM Temporadas WHERE Nombre = 'Alta');

INSERT INTO Tarifas (SedeId, TemporadaId, NumHabitaciones, PersonasMin, PersonasMax, ValorNoche, ValorPersonaAdicional) VALUES
(@SurameId, @TBId, 1, 1, 1, 63000.00, 0),
(@SurameId, @TBId, 1, 2, 2, 75000.00, 0),
(@SurameId, @TAId, 1, 1, 1, 63000.00, 0),
(@SurameId, @TAId, 1, 2, 2, 75000.00, 0);
GO

-- ---- Apartamentos El Rodadero Santa Marta ----
DECLARE @RodaderoId INT = (SELECT SedeId FROM Sedes WHERE NombreCorto = 'Rodadero');
DECLARE @TBaja INT      = (SELECT TemporadaId FROM Temporadas WHERE Nombre = 'Baja');
DECLARE @TAlta INT      = (SELECT TemporadaId FROM Temporadas WHERE Nombre = 'Alta');

INSERT INTO Tarifas (SedeId, TemporadaId, NumHabitaciones, PersonasMin, PersonasMax, ValorNoche, ValorPersonaAdicional) VALUES
-- Apto 301 y 401 (hasta 6 personas) - aplica tarifa por sede, se diferencia en el alojamiento
(@RodaderoId, @TBaja, 2,  1, 6,  89000.00, 0),
(@RodaderoId, @TAlta, 2,  1, 6, 124000.00, 0),
-- Apto 202 (hasta 8 personas)
(@RodaderoId, @TBaja, 3,  1, 8, 103000.00, 0),
(@RodaderoId, @TAlta, 3,  1, 8, 143000.00, 0);
GO

-- =============================================
-- =============================================
-- PROCEDIMIENTOS ALMACENADOS
-- =============================================
-- =============================================

-- =============================================
-- SP 1: Buscar habitaciones disponibles por rango de fechas
-- =============================================
CREATE OR ALTER PROCEDURE sp_BuscarDisponibilidadFechas
    @SedeId      INT,
    @FechaInicio DATE,
    @FechaFin    DATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Retorna alojamientos de la sede que NO tienen reserva
    -- confirmada/pendiente que se cruce con el rango solicitado
    SELECT
        a.AlojamientoId,
        a.Numero,
        a.Nombre,
        a.NumHabitaciones,
        a.CapacidadMax,
        a.TieneBano,
        a.TieneCocineta,
        a.TieneTelevision,
        a.TieneNevera,
        a.TieneTerraza,
        a.TieneSalaEstar,
        a.TieneParqueadero,
        a.Descripcion,
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM Reservas r
                INNER JOIN DetalleReserva dr ON r.ReservaId = dr.ReservaId
                WHERE dr.AlojamientoId = a.AlojamientoId
                  AND r.Estado IN ('Pendiente','Confirmada')
                  AND r.FechaLlegada < @FechaFin
                  AND r.FechaSalida  > @FechaInicio
            ) THEN 0
            ELSE 1
        END AS Disponible
    FROM Alojamientos a
    WHERE a.SedeId = @SedeId
      AND a.Activo = 1
    ORDER BY a.Numero;
END
GO

-- =============================================
-- SP 2: Disponibilidad por fechas Y número de personas
-- =============================================
CREATE OR ALTER PROCEDURE sp_BuscarDisponibilidadFechasPersonas
    @SedeId       INT,
    @FechaInicio  DATE,
    @FechaFin     DATE,
    @NumPersonas  INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        a.AlojamientoId,
        a.Numero,
        a.Nombre,
        a.NumHabitaciones,
        a.CapacidadMax,
        a.Descripcion,
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM Reservas r
                INNER JOIN DetalleReserva dr ON r.ReservaId = dr.ReservaId
                WHERE dr.AlojamientoId = a.AlojamientoId
                  AND r.Estado IN ('Pendiente','Confirmada')
                  AND r.FechaLlegada < @FechaFin
                  AND r.FechaSalida  > @FechaInicio
            ) THEN 0
            ELSE 1
        END AS Disponible
    FROM Alojamientos a
    WHERE a.SedeId      = @SedeId
      AND a.Activo      = 1
      AND a.CapacidadMax >= @NumPersonas   -- Solo los que tienen capacidad suficiente
    ORDER BY a.CapacidadMax, a.Numero;
END
GO

-- =============================================
-- SP 3: Consultar tarifas según sede, temporada, personas y alojamiento
-- =============================================
CREATE OR ALTER PROCEDURE sp_ConsultarTarifas
    @SedeId        INT,
    @FechaInicio   DATE,
    @NumPersonas   INT,
    @AlojamientoId INT = NULL   -- Opcional; si se provee, filtra por ese alojamiento
AS
BEGIN
    SET NOCOUNT ON;

    -- Determinar la temporada aplicable a la fecha de inicio
    DECLARE @TemporadaId INT;

    -- Primero verificar si cae en Alta Temporada por rango de fechas
    SELECT TOP 1 @TemporadaId = ft.TemporadaId
    FROM FechasTemporada ft
    WHERE @FechaInicio BETWEEN ft.FechaInicio AND ft.FechaFin
      AND ft.TemporadaId = (SELECT TemporadaId FROM Temporadas WHERE Nombre = 'Alta')
    ORDER BY ft.FechaInicio;

    -- Si no es Alta, verificar si aplica tarifa Especial (Lun-Jue, no festivo, no escolar)
    IF @TemporadaId IS NULL
    BEGIN
        IF DATEPART(WEEKDAY, @FechaInicio) BETWEEN 2 AND 5  -- Lunes=2 .. Jueves=5
        BEGIN
            SELECT @TemporadaId = TemporadaId FROM Temporadas WHERE Nombre = 'Especial';
        END
        ELSE
        BEGIN
            SELECT @TemporadaId = TemporadaId FROM Temporadas WHERE Nombre = 'Baja';
        END
    END

    -- Retornar tarifas
    SELECT
        t.TarifaId,
        s.Nombre         AS NombreSede,
        tp.Nombre        AS Temporada,
        t.NumHabitaciones,
        t.PersonasMin,
        t.PersonasMax,
        t.ValorNoche,
        t.ValorPersonaAdicional,
        @TemporadaId     AS TemporadaAplicada,
        -- Cálculo de persona adicional
        CASE
            WHEN @NumPersonas > t.PersonasMax
            THEN (@NumPersonas - t.PersonasMax) * t.ValorPersonaAdicional
            ELSE 0
        END AS AdicionalPorPersonas,
        t.ValorNoche +
        CASE
            WHEN @NumPersonas > t.PersonasMax
            THEN (@NumPersonas - t.PersonasMax) * t.ValorPersonaAdicional
            ELSE 0
        END AS ValorNocheTotal,
        -- Info del alojamiento si se especificó
        a.AlojamientoId,
        a.Numero         AS NumeroAlojamiento,
        a.Nombre         AS NombreAlojamiento,
        a.CapacidadMax
    FROM Tarifas t
    INNER JOIN Sedes     s  ON t.SedeId      = s.SedeId
    INNER JOIN Temporadas tp ON t.TemporadaId = tp.TemporadaId
    LEFT  JOIN Alojamientos a ON a.SedeId    = t.SedeId
        AND (@AlojamientoId IS NULL OR a.AlojamientoId = @AlojamientoId)
    WHERE t.SedeId      = @SedeId
      AND t.TemporadaId = @TemporadaId
      AND t.Activa      = 1
      AND @NumPersonas  <= (t.PersonasMax + 6)  -- margen para adicionales
    ORDER BY t.NumHabitaciones;
END
GO

-- =============================================
-- SP 4: Calcular tarifa total de la reserva
-- =============================================
CREATE OR ALTER PROCEDURE sp_CalcularTarifaReserva
    @SedeId            INT,
    @AlojamientoId     INT,
    @FechaInicio       DATE,
    @FechaFin          DATE,
    @NumPersonas       INT,
    @NumHabitaciones   INT,
    @ServicioLavanderia BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NumNoches         INT;
    DECLARE @TemporadaId       INT;
    DECLARE @TemporadaNombre   NVARCHAR(50);
    DECLARE @ValorNoche        DECIMAL(12,2);
    DECLARE @ValorAdicional    DECIMAL(12,2);
    DECLARE @PersonasBase      INT;
    DECLARE @TotalAdicionales  DECIMAL(12,2) = 0;
    DECLARE @TotalLavanderia   DECIMAL(12,2) = 0;
    DECLARE @TotalReserva      DECIMAL(12,2) = 0;

    SET @NumNoches = DATEDIFF(DAY, @FechaInicio, @FechaFin);

    IF @NumNoches <= 0
    BEGIN
        RAISERROR('La fecha de salida debe ser posterior a la fecha de llegada.', 16, 1);
        RETURN;
    END

    -- Determinar temporada
    SELECT TOP 1 @TemporadaId = ft.TemporadaId
    FROM FechasTemporada ft
    WHERE @FechaInicio BETWEEN ft.FechaInicio AND ft.FechaFin
      AND ft.TemporadaId = (SELECT TemporadaId FROM Temporadas WHERE Nombre = 'Alta')
    ORDER BY ft.FechaInicio;

    IF @TemporadaId IS NULL
    BEGIN
        IF DATEPART(WEEKDAY, @FechaInicio) BETWEEN 2 AND 5
            SELECT @TemporadaId = TemporadaId FROM Temporadas WHERE Nombre = 'Especial';
        ELSE
            SELECT @TemporadaId = TemporadaId FROM Temporadas WHERE Nombre = 'Baja';
    END

    SELECT @TemporadaNombre = Nombre FROM Temporadas WHERE TemporadaId = @TemporadaId;

    -- Obtener tarifa
    SELECT TOP 1
        @ValorNoche     = t.ValorNoche,
        @ValorAdicional = t.ValorPersonaAdicional,
        @PersonasBase   = t.PersonasMax
    FROM Tarifas t
    WHERE t.SedeId         = @SedeId
      AND t.TemporadaId    = @TemporadaId
      AND t.NumHabitaciones = @NumHabitaciones
      AND t.Activa         = 1
    ORDER BY t.PersonasMax DESC;

    IF @ValorNoche IS NULL
    BEGIN
        RAISERROR('No se encontró tarifa para los parámetros indicados.', 16, 1);
        RETURN;
    END

    -- Calcular personas adicionales
    IF @NumPersonas > @PersonasBase
        SET @TotalAdicionales = (@NumPersonas - @PersonasBase) * @ValorAdicional * @NumNoches;

    -- Lavandería Santa Marta: $18.000 por estadía
    IF @ServicioLavanderia = 1
    BEGIN
        DECLARE @CiudadSede NVARCHAR(100);
        SELECT @CiudadSede = Ciudad FROM Sedes WHERE SedeId = @SedeId;
        IF @CiudadSede = 'Santa Marta'
            SET @TotalLavanderia = 18000.00;
    END

    SET @TotalReserva = (@ValorNoche * @NumNoches) + @TotalAdicionales + @TotalLavanderia;

    -- Resultado
    SELECT
        @SedeId           AS SedeId,
        @AlojamientoId    AS AlojamientoId,
        @FechaInicio      AS FechaInicio,
        @FechaFin         AS FechaFin,
        @NumNoches        AS NumNoches,
        @NumPersonas      AS NumPersonas,
        @NumHabitaciones  AS NumHabitaciones,
        @TemporadaNombre  AS Temporada,
        @ValorNoche       AS ValorNocheSinAdicional,
        @NumNoches * @ValorNoche AS SubtotalHabitaciones,
        @TotalAdicionales AS TotalPersonasAdicionales,
        @TotalLavanderia  AS TotalLavanderia,
        @TotalReserva     AS ValorTotalReserva;
END
GO

-- =============================================
-- SP 5: Crear una reserva
-- =============================================
CREATE OR ALTER PROCEDURE sp_CrearReserva
    @UsuarioId          INT,
    @SedeId             INT,
    @AlojamientoId      INT,
    @FechaLlegada       DATE,
    @FechaSalida        DATE,
    @NumPersonas        INT,
    @NumHabitaciones    INT,
    @ServicioLavanderia BIT = 0,
    @Observaciones      NVARCHAR(500) = NULL,
    @NuevaReservaId     INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Verificar disponibilidad
        IF EXISTS (
            SELECT 1
            FROM Reservas r
            INNER JOIN DetalleReserva dr ON r.ReservaId = dr.ReservaId
            WHERE dr.AlojamientoId = @AlojamientoId
              AND r.Estado IN ('Pendiente','Confirmada')
              AND r.FechaLlegada < @FechaSalida
              AND r.FechaSalida  > @FechaLlegada
        )
        BEGIN
            RAISERROR('El alojamiento no está disponible para las fechas seleccionadas.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Calcular tarifa
        DECLARE @ValorTotal DECIMAL(12,2);
        DECLARE @TemporadaId INT;

        SELECT TOP 1 @TemporadaId = ft.TemporadaId
        FROM FechasTemporada ft
        WHERE @FechaLlegada BETWEEN ft.FechaInicio AND ft.FechaFin
          AND ft.TemporadaId = (SELECT TemporadaId FROM Temporadas WHERE Nombre = 'Alta')
        ORDER BY ft.FechaInicio;

        IF @TemporadaId IS NULL
        BEGIN
            IF DATEPART(WEEKDAY, @FechaLlegada) BETWEEN 2 AND 5
                SELECT @TemporadaId = TemporadaId FROM Temporadas WHERE Nombre = 'Especial';
            ELSE
                SELECT @TemporadaId = TemporadaId FROM Temporadas WHERE Nombre = 'Baja';
        END

        DECLARE @NumNoches INT = DATEDIFF(DAY, @FechaLlegada, @FechaSalida);
        DECLARE @ValorNoche DECIMAL(12,2);
        DECLARE @ValAdicional DECIMAL(12,2);
        DECLARE @PersBase INT;

        SELECT TOP 1
            @ValorNoche   = t.ValorNoche,
            @ValAdicional = t.ValorPersonaAdicional,
            @PersBase     = t.PersonasMax
        FROM Tarifas t
        WHERE t.SedeId          = @SedeId
          AND t.TemporadaId     = @TemporadaId
          AND t.NumHabitaciones = @NumHabitaciones
          AND t.Activa          = 1
        ORDER BY t.PersonasMax DESC;

        DECLARE @Adicionales DECIMAL(12,2) = 0;
        IF @NumPersonas > @PersBase
            SET @Adicionales = (@NumPersonas - @PersBase) * @ValAdicional * @NumNoches;

        DECLARE @Lavanderia DECIMAL(12,2) = 0;
        IF @ServicioLavanderia = 1
        BEGIN
            DECLARE @Ciudad NVARCHAR(100);
            SELECT @Ciudad = Ciudad FROM Sedes WHERE SedeId = @SedeId;
            IF @Ciudad = 'Santa Marta' SET @Lavanderia = 18000.00;
        END

        SET @ValorTotal = (@ValorNoche * @NumNoches) + @Adicionales + @Lavanderia;

        -- Insertar reserva
        INSERT INTO Reservas (UsuarioId, SedeId, FechaLlegada, FechaSalida, NumPersonas,
                              NumHabitaciones, ServicioLavanderia, TemporadaId, ValorTotal,
                              Estado, Observaciones)
        VALUES (@UsuarioId, @SedeId, @FechaLlegada, @FechaSalida, @NumPersonas,
                @NumHabitaciones, @ServicioLavanderia, @TemporadaId, @ValorTotal,
                'Pendiente', @Observaciones);

        SET @NuevaReservaId = SCOPE_IDENTITY();

        -- Insertar detalle
        INSERT INTO DetalleReserva (ReservaId, AlojamientoId, ValorNoche, NumNoches, SubTotal)
        VALUES (@NuevaReservaId, @AlojamientoId, @ValorNoche, @NumNoches,
                @ValorNoche * @NumNoches);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- =============================================
-- SP 6: Consultar reservas del usuario
-- =============================================
CREATE OR ALTER PROCEDURE sp_ConsultarReservasUsuario
    @UsuarioId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        r.ReservaId,
        s.Nombre          AS Sede,
        s.Ciudad,
        ts.Nombre         AS TipoSede,
        r.FechaReserva,
        r.FechaLlegada,
        r.FechaSalida,
        DATEDIFF(DAY, r.FechaLlegada, r.FechaSalida) AS NumNoches,
        r.NumPersonas,
        r.NumHabitaciones,
        r.ServicioLavanderia,
        tp.Nombre         AS Temporada,
        r.ValorTotal,
        r.Estado,
        a.Numero          AS NumeroAlojamiento,
        a.Nombre          AS NombreAlojamiento
    FROM Reservas r
    INNER JOIN Sedes         s  ON r.SedeId      = s.SedeId
    INNER JOIN TipoSede      ts ON s.TipoSedeId  = ts.TipoSedeId
    INNER JOIN Temporadas    tp ON r.TemporadaId = tp.TemporadaId
    INNER JOIN DetalleReserva dr ON r.ReservaId  = dr.ReservaId
    INNER JOIN Alojamientos   a  ON dr.AlojamientoId = a.AlojamientoId
    WHERE r.UsuarioId = @UsuarioId
    ORDER BY r.FechaReserva DESC;
END
GO

/*

PARA ACTUALIZAR LAS IMÁGENES 

UPDATE Sedes SET ImagenPrincipal = 'villeta.jpeg'     WHERE NombreCorto = 'Villeta';
UPDATE Sedes SET ImagenPrincipal = 'fusagasuga.jpeg'  WHERE NombreCorto = 'El Placer';
UPDATE Sedes SET ImagenPrincipal = 'chinchina.jpeg'   WHERE NombreCorto = 'Chinchiná';
UPDATE Sedes SET ImagenPrincipal = 'palmira.jpeg'     WHERE NombreCorto = 'Palmira';
UPDATE Sedes SET ImagenPrincipal = 'santafe.jpeg'     WHERE NombreCorto = 'Sta Fé Antioquia';
UPDATE Sedes SET ImagenPrincipal = 'bogota.jpeg'      WHERE NombreCorto = 'Bogotá';
UPDATE Sedes SET ImagenPrincipal = 'medellin.jpeg'    WHERE NombreCorto = 'Suramericana';
UPDATE Sedes SET ImagenPrincipal = 'santamartha.jpeg' WHERE NombreCorto = 'Rodadero';
*/

