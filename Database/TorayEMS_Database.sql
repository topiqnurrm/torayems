/* ============================================================
   TorayEMS Database Script
   Employee Management System - Demo Project
   Berisi: Database, Tabel, Foreign Key, Index, Stored Procedures, Seed Data
   Jalankan script ini di SQL Server Management Studio (SSMS)
   atau Azure Data Studio yang terhubung ke LocalDB / SQL Server.
   ============================================================ */

IF DB_ID('TorayEMSDb') IS NOT NULL
BEGIN
    ALTER DATABASE TorayEMSDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TorayEMSDb;
END
GO

CREATE DATABASE TorayEMSDb;
GO

USE TorayEMSDb;
GO

/* ============================================================
   1. TABEL
   ============================================================ */

CREATE TABLE Departments
(
    DepartmentId    INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentName  NVARCHAR(100)   NOT NULL,
    Description     NVARCHAR(255)   NULL,
    CONSTRAINT UQ_Departments_DepartmentName UNIQUE (DepartmentName)
);
GO

CREATE TABLE Employees
(
    EmployeeId      INT IDENTITY(1,1) PRIMARY KEY,
    FullName        NVARCHAR(150)   NOT NULL,
    Email           NVARCHAR(150)   NOT NULL,
    Position        NVARCHAR(100)   NOT NULL,
    DepartmentId    INT             NOT NULL,
    HireDate        DATE            NOT NULL,
    Salary          DECIMAL(18,2)   NOT NULL DEFAULT 0,
    PhoneNumber     NVARCHAR(20)    NULL,
    IsActive        BIT             NOT NULL DEFAULT 1,
    CONSTRAINT UQ_Employees_Email UNIQUE (Email),
    CONSTRAINT FK_Employees_Departments FOREIGN KEY (DepartmentId)
        REFERENCES Departments (DepartmentId)
);
GO

-- Index tambahan untuk mempercepat query yang sering dipakai (optimasi)
CREATE INDEX IX_Employees_DepartmentId ON Employees (DepartmentId);
CREATE INDEX IX_Employees_IsActive ON Employees (IsActive);
GO

/* ============================================================
   2. STORED PROCEDURES - DEPARTMENTS
   ============================================================ */

CREATE PROCEDURE sp_Department_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT DepartmentId, DepartmentName, Description
    FROM Departments
    ORDER BY DepartmentName;
END
GO

CREATE PROCEDURE sp_Department_GetById
    @DepartmentId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT DepartmentId, DepartmentName, Description
    FROM Departments
    WHERE DepartmentId = @DepartmentId;
END
GO

CREATE PROCEDURE sp_Department_Insert
    @DepartmentName NVARCHAR(100),
    @Description    NVARCHAR(255) = NULL,
    @NewDepartmentId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Departments (DepartmentName, Description)
    VALUES (@DepartmentName, @Description);

    SET @NewDepartmentId = SCOPE_IDENTITY();
END
GO

CREATE PROCEDURE sp_Department_Update
    @DepartmentId   INT,
    @DepartmentName NVARCHAR(100),
    @Description    NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Departments
    SET DepartmentName = @DepartmentName,
        Description    = @Description
    WHERE DepartmentId = @DepartmentId;
END
GO

CREATE PROCEDURE sp_Department_Delete
    @DepartmentId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Cegah penghapusan jika masih ada karyawan terkait (menjaga integritas data)
    IF EXISTS (SELECT 1 FROM Employees WHERE DepartmentId = @DepartmentId)
    BEGIN
        RAISERROR('Departemen tidak dapat dihapus karena masih memiliki karyawan terkait.', 16, 1);
        RETURN;
    END

    DELETE FROM Departments WHERE DepartmentId = @DepartmentId;
END
GO

CREATE PROCEDURE sp_Dashboard_EmployeeCountByDepartment
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        d.DepartmentId,
        d.DepartmentName,
        COUNT(e.EmployeeId) AS EmployeeCount
    FROM Departments d
    LEFT JOIN Employees e ON e.DepartmentId = d.DepartmentId
    GROUP BY d.DepartmentId, d.DepartmentName
    ORDER BY EmployeeCount DESC;
END
GO

/* ============================================================
   3. STORED PROCEDURES - EMPLOYEES
   ============================================================ */

CREATE PROCEDURE sp_Employee_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        e.EmployeeId, e.FullName, e.Email, e.Position, e.DepartmentId,
        d.DepartmentName, e.HireDate, e.Salary, e.PhoneNumber, e.IsActive
    FROM Employees e
    INNER JOIN Departments d ON d.DepartmentId = e.DepartmentId
    ORDER BY e.FullName;
END
GO

CREATE PROCEDURE sp_Employee_GetById
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        e.EmployeeId, e.FullName, e.Email, e.Position, e.DepartmentId,
        d.DepartmentName, e.HireDate, e.Salary, e.PhoneNumber, e.IsActive
    FROM Employees e
    INNER JOIN Departments d ON d.DepartmentId = e.DepartmentId
    WHERE e.EmployeeId = @EmployeeId;
END
GO

CREATE PROCEDURE sp_Employee_Insert
    @FullName       NVARCHAR(150),
    @Email          NVARCHAR(150),
    @Position       NVARCHAR(100),
    @DepartmentId   INT,
    @HireDate       DATE,
    @Salary         DECIMAL(18,2),
    @PhoneNumber    NVARCHAR(20) = NULL,
    @IsActive       BIT = 1,
    @NewEmployeeId  INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Employees WHERE Email = @Email)
    BEGIN
        RAISERROR('Email sudah digunakan oleh karyawan lain.', 16, 1);
        RETURN;
    END

    INSERT INTO Employees (FullName, Email, Position, DepartmentId, HireDate, Salary, PhoneNumber, IsActive)
    VALUES (@FullName, @Email, @Position, @DepartmentId, @HireDate, @Salary, @PhoneNumber, @IsActive);

    SET @NewEmployeeId = SCOPE_IDENTITY();
END
GO

CREATE PROCEDURE sp_Employee_Update
    @EmployeeId     INT,
    @FullName       NVARCHAR(150),
    @Email          NVARCHAR(150),
    @Position       NVARCHAR(100),
    @DepartmentId   INT,
    @HireDate       DATE,
    @Salary         DECIMAL(18,2),
    @PhoneNumber    NVARCHAR(20) = NULL,
    @IsActive       BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Employees WHERE Email = @Email AND EmployeeId <> @EmployeeId)
    BEGIN
        RAISERROR('Email sudah digunakan oleh karyawan lain.', 16, 1);
        RETURN;
    END

    UPDATE Employees
    SET FullName     = @FullName,
        Email        = @Email,
        Position     = @Position,
        DepartmentId = @DepartmentId,
        HireDate     = @HireDate,
        Salary       = @Salary,
        PhoneNumber  = @PhoneNumber,
        IsActive     = @IsActive
    WHERE EmployeeId = @EmployeeId;
END
GO

CREATE PROCEDURE sp_Employee_Delete
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Employees WHERE EmployeeId = @EmployeeId;
END
GO

/* ============================================================
   4. SEED DATA (data contoh agar aplikasi langsung bisa didemokan)
   ============================================================ */

INSERT INTO Departments (DepartmentName, Description) VALUES
('Information Technology', 'Mengelola sistem, jaringan, dan aplikasi internal perusahaan'),
('Production', 'Bertanggung jawab atas proses produksi serat sintetis'),
('Quality Control', 'Memastikan standar kualitas produk sebelum didistribusikan'),
('Human Resources', 'Mengelola rekrutmen, payroll, dan pengembangan karyawan'),
('Finance', 'Mengelola keuangan, akuntansi, dan anggaran perusahaan');
GO

INSERT INTO Employees (FullName, Email, Position, DepartmentId, HireDate, Salary, PhoneNumber, IsActive) VALUES
('Andi Pratama',        'andi.pratama@torayems.co.id',   'IT Supervisor',        1, '2022-03-01', 9500000, '081234567801', 1),
('Budi Santoso',        'budi.santoso@torayems.co.id',   'Web Developer',        1, '2023-06-15', 7500000, '081234567802', 1),
('Citra Ayu Lestari',   'citra.lestari@torayems.co.id',  'Production Staff',     2, '2021-01-10', 6000000, '081234567803', 1),
('Dedi Kurniawan',      'dedi.kurniawan@torayems.co.id', 'Production Supervisor',2, '2019-08-20', 8500000, '081234567804', 1),
('Eka Wulandari',       'eka.wulandari@torayems.co.id',  'QC Analyst',           3, '2022-11-05', 6800000, '081234567805', 1),
('Fajar Nugroho',       'fajar.nugroho@torayems.co.id',  'HR Officer',           4, '2020-02-17', 7000000, '081234567806', 1),
('Gita Permatasari',    'gita.permatasari@torayems.co.id','Finance Staff',       5, '2023-01-09', 6500000, '081234567807', 0);
GO

PRINT 'Database TorayEMSDb berhasil dibuat lengkap dengan tabel, stored procedure, dan data contoh.';
