IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'WorkManagementDB')
BEGIN
    CREATE DATABASE WorkManagementDB;
END
GO

USE WorkManagementDB;
GO


-- Tabla de Usuarios (Microservicio UserManagement)
-- =============================================
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
GO

CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) -- Opcional, pero útil
);
GO


-- Tabla de Ítems de Trabajo (Microservicio WorkItems)
-- =============================================
IF OBJECT_ID('dbo.WorkItems', 'U') IS NOT NULL DROP TABLE dbo.WorkItems;
GO

CREATE TABLE WorkItems (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    
    Relevance NVARCHAR(10) NOT NULL CHECK (Relevance IN ('High', 'Low')),
    
    DueDate DATETIME NOT NULL,
    IsCompleted BIT DEFAULT 0, 
    
    UserId UNIQUEIDENTIFIER NULL, 
    
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
GO

-- usuarios de prueba para el algoritmo
INSERT INTO Users (Name, Email) VALUES 
('Usuario Alfredo', 'alfredo@test.com'), 
('Usuario Bertha', 'bertha@test.com'),
('Usuario Carmen', 'carmen@test.com');
GO