# Sistema de Reservas FODUN
Sistema de reservas en línea para sedes recreativas y apartamentos del Fondo FODUN.
Desarrollado como prueba técnica para el cargo de Analista Desarrollador .NET.

## Tecnologías utilizadas
- ASP.NET Core MVC 8.0
- Entity Framework Core 8.0
- Microsoft SQL Server 2019/2022
- C# .NET 8
- Bootstrap 5.3
- Razor Pages

## Requisitos previos
- Visual Studio 2022 Community
- SQL Server 2019 o 2022 (Developer Edition)
- SQL Server Management Studio (SSMS)
- .NET 8 SDK
- Cuenta de Gmail con contraseña de aplicación

## Instalación

### 1. Clonar el repositorio
git clone https://github.com/TU_USUARIO/FODUN.Reservas.git

### 2. Configurar la base de datos
- Abrir SQL Server Management Studio
- Abrir el archivo `FODUN_DB_Script.sql`
- Ejecutar el script completo con F5
- Verificar que se creó la base de datos `FODUN_Reservas`

### 3. Configurar el proyecto
- Ir a la carpeta `FODUN.Web`
- Copiar `appsettings.Example.json` y renombrarlo a `appsettings.json`
- Actualizar la cadena de conexión con tu servidor SQL Server:
```json
"DefaultConnection": "Server=TU_SERVIDOR;Database=FODUN_Reservas;Trusted_Connection=True;TrustServerCertificate=True;"
```

### 4. Configurar el correo SMTP
- Tener una cuenta Gmail
- Activar verificación en dos pasos
- Ir a https://myaccount.google.com/apppasswords
- Generar contraseña de aplicación
- Actualizar en `appsettings.json`:
```json
"EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUser": "tucorreo@gmail.com",
    "SmtpPassword": "xxxx xxxx xxxx xxxx",
    "FromEmail": "tucorreo@gmail.com",
    "FromName": "Sistema de Reservas FODUN"
}
```

### 5. Configurar imágenes
- Ir a la carpeta `FODUN.Web/wwwroot/images/sedes`
- Agregar imágenes de cada sede con estos nombres:
  - `villeta.jpeg`
  - `fusagasuga.jpeg`
  - `chinchina.jpeg`
  - `palmira.jpeg`
  - `santafe.jpeg`
  - `bogota.jpeg`
  - `medellin.jpeg`
  - `santamartha.jpeg`
  - `default.png`
- Ejecutar en SSMS:
```sql
UPDATE Sedes SET ImagenPrincipal = 'villeta.jpeg' WHERE NombreCorto = 'Villeta';
UPDATE Sedes SET ImagenPrincipal = 'fusagasuga.jpeg' WHERE NombreCorto = 'El Placer';
UPDATE Sedes SET ImagenPrincipal = 'chinchina.jpeg' WHERE NombreCorto = 'Chinchiná';
UPDATE Sedes SET ImagenPrincipal = 'palmira.jpeg' WHERE NombreCorto = 'Palmira';
UPDATE Sedes SET ImagenPrincipal = 'santafe.jpeg' WHERE NombreCorto = 'Sta Fé Antioquia';
UPDATE Sedes SET ImagenPrincipal = 'bogota.jpeg' WHERE NombreCorto = 'Bogotá';
UPDATE Sedes SET ImagenPrincipal = 'medellin.jpeg' WHERE NombreCorto = 'Suramericana';
UPDATE Sedes SET ImagenPrincipal = 'santamartha.jpeg' WHERE NombreCorto = 'Rodadero';
```

### 6. Ejecutar el proyecto
- Abrir `FODUN.Reservas.sln` en Visual Studio 2022
- Restaurar paquetes NuGet: `Update-Package -reinstall`
- Compilar con `Ctrl + Shift + B`
- Ejecutar con `F5`
- El navegador abrirá en la pantalla de Login

## Estructura del proyecto
FODUN.Reservas
├── FODUN.Entities    → Entidades / Modelos
├── FODUN.DAL         → Acceso a datos (EF Core)
├── FODUN.BLL         → Lógica de negocio
└── FODUN.Web         → Presentación (MVC, Razor)

## Funcionalidades
- Registro e inicio de sesión de usuarios
- Recuperación de contraseña por correo SMTP
- Consulta de sedes recreativas y apartamentos
- Verificación de disponibilidad por fechas y personas
- Cálculo de tarifas por temporada
- Creación y consulta de reservas
- Envío de comprobante de pago por correo
- Actualización de datos del usuario

## Autor
**Fabian Hincapie Castañeda**
fhc-91@hotmail.com