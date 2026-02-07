# Sistema de Gestión de Ítems de Trabajo (Microservicios)

Este proyecto implementa una arquitectura de microservicios utilizando **.NET 10** y **SQL Server**. El objetivo principal es la distribución inteligente de tareas basada en algoritmos que consideran fechas de vencimiento, relevancia y saturación de los usuarios.

## 🚀 Arquitectura

El sistema está dividido en dos microservicios independientes que se comunican vía HTTP:

1.  **UserManagementService:** Gestiona la información de los usuarios.
2.  **WorkItemsService:** Gestiona las tareas y contiene el **Algoritmo de Asignación Inteligente**.

## 📋 Requisitos Previos

Para ejecutar este proyecto necesitas:

* [.NET 10 SDK]
* [SQL Server] (Local o Remoto)
* Un editor de código (Visual Studio o VS Code)

## ⚙️ Configuración de la Base de Datos

### 1. Crear la Base de Datos
En la raíz del proyecto encontrarás una carpeta `DataBase` con el script `script_db.sql`. Ejecuta este script en tu servidor SQL para crear la base de datos `WorkManagementDB` y las tablas necesarias.

### 2. Configurar la Cadena de Conexión (Importante)
Cada microservicio se conecta a la base de datos de manera independiente. Debes configurar tu servidor local en **ambos** proyectos.

1.  Navega a `backend/UserManagementService/appsettings.json`
2.  Navega a `backend/WorkItemsService/appsettings.json`

En **ambos archivos**, localiza la sección `ConnectionStrings` y modifica los valores de `Server`, `User Id` y `Password` según tu configuración local:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=TU_SERVIDOR_SQL; Database=WorkManagementDB; User Id=TU_USUARIO; Password=TU_PASSWORD; TrustServerCertificate=True;"
}
```
## 🛠️ Ejecución del Proyecto
Para levantar el sistema, debe abrir dos terminales y ejecutar cada microservicio por separado:
### 1.   Iniciar Microservicio de Usuarios (UserManagementService)
En la primera terminal, ejecute:

```json
cd backend/UserManagementService
dotnet run 
```
#### Swagger UI: Una vez iniciado, acceda a la documentación en: http://localhost:5231/swagger/index.html
![Swagger UserManagementService](assets/microservice1.png)

### 2. Iniciar Microservicio de Ítems (WorkItemsService)
En la segunda terminal, ejecute:
```json
cd backend/WorkItemsService
dotnet run
```
#### Swagger UI: Una vez iniciado, acceda a la documentación en: http://localhost:5121/swagger/index.html
![Swagger WorkItemsService](assets/microservice2.png)

## ✅ Verificación de Resultados
### Ve al endpoint POST /api/WorkItems.

#### Prueba 1: El Balanceo de Carga (Prueba Normal)
Objetivo: Verificar que si las fechas son lejanas, el sistema reparte las tareas equitativamente entre los usuarios disponibles.  
Acción: Vamos a crear 3 tareas Normales (Fecha lejana, Relevancia Baja).  
```json
{
  "title": "Tarea Normal",
  "description": "Prueba de balanceo",
  "relevance": "Low",
  "dueDate": "2026-12-31T23:59:59" 
}
```
Resultado Esperado: Como tienes 3 usuarios  creados por el script incial
- La 1ra tarea se asigna a cualquiera.
- La 2da tarea debería ir a otro usuario porque A ya tiene 1.
- La 3ra tarea debería ir al último usuario.

#### Prueba 2: Modo Pánico (Urgencia < 3 días)
Objetivo: Verificar que si la fecha es para mañana, el sistema ignora la relevancia y busca desesperadamente al más libre.  
Acción: Vamos a simular una urgencia. Cambia la fecha para mañana (o pasado mañana). 
```json
{
  "title": "URGENCIA PÁNICO",
  "description": "Esta tarea vence en 2 días",
  "relevance": "High", 
  "dueDate": "2026-02-08T23:59:59"
}
```
Resultado Esperado: El sistema debe asignarlo al usuario con menos carga TOTAL en ese instante.

#### Prueba 3: La Regla de Saturación 
Objetivo: Llenar a un usuario de tareas "High" y ver si el sistema deja de asignarle tareas nuevas.  
Acción: Vamos a "saturar" manualmente al sistema. Ejecuta este JSON 4 veces (esto simula que entraron 4 proyectos críticos). Para que caigan en el mismo usuario, el algoritmo debería ir balanceando, pero al final todos tendrán carga alta.
```json
{
  "title": "Proyecto Crítico",
  "description": "Saturando usuarios",
  "relevance": "High",
  "dueDate": "2026-10-01T00:00:00"
}
```


Resultado Esperado: Ahora, lanza una tarea Normal con Fecha lejana y con Relevancia Low.
- El sistema debería mirar quién tiene MÁS de 3 tareas High y saltárselo, asignando la tarea a alguien que no esté saturado.

#### Para comprobar que el algoritmo de distribución está asignando las tareas a los usuarios correctos según la lógica de negocio, ejecute la siguiente consulta en su SQL Server:
```json
SELECT
    WorkItems.Title AS Tarea,
    Users.Name AS Asignado_A,
    WorkItems.UserId AS ID_Usuario
FROM WorkItems
JOIN Users ON WorkItems.UserId = Users.Id;
```
![DataBase Results](assets/result_db.png)