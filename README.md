# 🛒 Ejercicio .NET - Product API

¡Bienvenido a **Product API**! 🚀

Este proyecto es una **API RESTful de productos** construida en **.NET 8** que se conecta a una base de datos **SQL Server**. Implementa **principios SOLID**, cuenta con **capas bien definidas** (DTOs, Repositories, Services) y soporta **paginación**, **seeders con datos realistas (Bogus)** y uso de **UUIDs** para mayor seguridad de los registros.

---

## 📂 Clonar el proyecto
```bash
git clone https://github.com/wallshapel/ejercicio-dotnet.git
cd ejercicio-dotnet/backend
```

---

## ⚙️ Configuración inicial
Antes de iniciar, debes crear un archivo **`appsettings.Development.json`** en la carpeta `backend/` con el siguiente contenido:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=127.0.0.1,1433;Database=<tu base de datos>;User Id=sa;Password=<tu clave>;Encrypt=True;TrustServerCertificate=True"
  }
}
```

👉 Reemplaza `<tu base de datos>` y `<tu clave>` con tus credenciales locales.

---

## 🧪 Ejecutar pruebas
El proyecto incluye un conjunto de pruebas automatizadas con **xUnit**, **Moq** y **FluentAssertions**.

Ejecuta:

```bash
dotnet test -c Release /p:CollectCoverage=true -p:VSTestUseMSBuildOutput=false
```

Esto mostrará en consola un **resumen tabular** con el porcentaje de cobertura.

---

## 📡 Endpoints principales
La API expone endpoints de productos totalmente funcionales con **paginación**.

Ejemplo:

```http
GET https://localhost:7131/api/Product?Page=1&PageSize=5
```

📌 Respuesta: lista de productos paginados con metadatos.

---

## 🏗️ Arquitectura
- **DTOs** → Manejo de transferencia de datos.  
- **Repositories** → Acceso a datos mediante EF Core.  
- **Services** → Lógica de negocio.  
- **Seeders** → Inicialización con datos **realistas** gracias a **Bogus**.  
- **UUIDs** → Seguridad extra para identificar los registros.  
- **Manejo global de excepciones** → Para el ejercicio, en caso de no haber data regresa un error controlado de manera global.  

---

## 🛢️ Requisitos de base de datos
- El servicio de **SQL Server** debe estar corriendo.  
- La base de datos `sample` (o la definida en tu `appsettings.Development.json`) **debe existir previamente**.  

---

## 🐳 Ejecución en ambiente dockerizado

Ahora la aplicación también está preparada para ejecutarse con **Docker**.  
Se incluye un `docker-compose.yml` que levanta dos servicios:  
- **ejercicio-dotnet-db** → Contenedor de SQL Server.  
- **ejercicio-dotnet-backend** → Contenedor con la API.  

Además, al iniciar la API en cualquier entorno, esta **crea automáticamente la base de datos, aplica las migraciones pendientes y ejecuta los seeders** (solo si la tabla está vacía).

### 🔧 Pasos
1. Construir e iniciar:
   ```bash
   docker compose up -d --build
   ```
2. Verificar servicios:
   ```bash
   docker compose ps
   ```

### 🌐 Accesos
- **Local (con Visual Studio/CLI)**  
  - Swagger: https://localhost:7131/swagger  
  - Postman: https://localhost:7131/api/product  

- **Dockerizado**  
  - Swagger: http://localhost:8080/swagger  
  - Postman: http://localhost:8080/api/product  

---

✅ ¡Y eso es todo! Ahora la API funciona tanto en tu entorno local como en contenedores Docker, asegurando que la base de datos y migraciones se gestionen automáticamente al iniciar.

