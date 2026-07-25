
## 1. Fitur

- **Dashboard** — ringkasan total karyawan, karyawan aktif, dan jumlah karyawan per departemen.
- **CRUD Karyawan** (Create, Read, Update, Delete) lewat halaman web (MVC + Razor Views).
- **CRUD Departemen** lewat halaman web.
- **REST Web API** terpisah (`/api/employees`, `/api/departments`) yang mengembalikan JSON,
  lengkap dengan dokumentasi interaktif **Swagger** di `/swagger`.
- Validasi form di sisi client (jQuery Validation) & server (Data Annotations).
- Semua query database memakai **Stored Procedure**, bukan query mentah/inline di kode C#.

---

## 2. Struktur Project

```
TorayEMS/
├── TorayEMS.sln
├── TorayEMS/                     <- Project utama ASP.NET Core
│   ├── Controllers/
│   │   ├── HomeController.cs         (Dashboard)
│   │   ├── EmployeesController.cs    (MVC CRUD Karyawan)
│   │   ├── DepartmentsController.cs  (MVC CRUD Departemen)
│   │   └── Api/
│   │       ├── EmployeesApiController.cs   (REST API)
│   │       └── DepartmentsApiController.cs (REST API)
│   ├── Data/                     <- Data Access Layer (ADO.NET + Stored Procedure)
│   │   ├── IDbConnectionFactory.cs / SqlConnectionFactory.cs
│   │   ├── IEmployeeRepository.cs / EmployeeRepository.cs
│   │   └── IDepartmentRepository.cs / DepartmentRepository.cs
│   ├── Models/                   <- Model & ViewModel
│   ├── Views/                    <- Razor Views (MVC)
│   ├── wwwroot/                  <- CSS statis
│   ├── appsettings.json          <- Connection string
│   └── Program.cs
└── Database/
    └── TorayEMS_Database.sql     <- Script SQL: buat DB, tabel, stored procedure, seed data
```

---

## 3. Cara Menjalankan (Step by Step)

### Prasyarat
1. **.NET 8 SDK** — unduh di https://dotnet.microsoft.com/download/dotnet/8.0
2. **SQL Server** — bisa pakai **SQL Server LocalDB** (biasanya sudah include kalau install
   Visual Studio dengan workload "ASP.NET and web development"), **SQL Server Express**, atau
   SQL Server versi lain / Docker.
3. **Visual Studio 2022** (disarankan, gratis Community edition) *atau* **VS Code** + ekstensi C#.

### Langkah 1 — Siapkan Database
1. Buka **SQL Server Management Studio (SSMS)** atau **Azure Data Studio**, connect ke instance
   SQL Server / LocalDB kamu.
2. Buka file `Database/TorayEMS_Database.sql`, lalu **Execute** seluruh script.
   Script ini otomatis membuat database `TorayEMSDb`, tabel, semua stored procedure, dan data contoh.

   > Alternatif lewat command line (kalau `sqlcmd` tersedia):
   > ```bash
   > sqlcmd -S "(localdb)\MSSQLLocalDB" -i Database/TorayEMS_Database.sql
   > ```

### Langkah 2 — Sesuaikan Connection String (jika perlu)
Buka `TorayEMS/appsettings.json`. Defaultnya sudah diarahkan ke LocalDB:
```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=TorayEMSDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```
Kalau kamu pakai SQL Server Express/instance lain, ganti `Server=...` sesuai nama instance kamu,
misalnya `Server=localhost\\SQLEXPRESS;...` atau tambahkan `User Id=...;Password=...;` kalau pakai SQL Authentication.

### Langkah 3 — Restore & Jalankan Aplikasi

**Lewat Visual Studio:**
1. Buka `TorayEMS.sln`.
2. Tunggu NuGet restore package (`Microsoft.Data.SqlClient`, `Swashbuckle.AspNetCore`) otomatis.
3. Tekan **F5** / klik ▶️ Run.

**Lewat CLI (dotnet):**
```bash
cd TorayEMS
dotnet restore
dotnet run
```
Aplikasi akan berjalan di `http://localhost:5100` (lihat pesan di terminal untuk port pastinya).

### Langkah 4 — Coba Aplikasinya
- Buka `http://localhost:5100/` → Dashboard
- Menu **Karyawan** → CRUD data karyawan
- Menu **Departemen** → CRUD data departemen
- Buka `http://localhost:5100/swagger` → dokumentasi & testing REST API secara interaktif

---

## 4. Contoh Endpoint API

| Method | Endpoint                 | Keterangan                     |
|--------|---------------------------|---------------------------------|
| GET    | `/api/employees`          | Ambil semua data karyawan       |
| GET    | `/api/employees/{id}`     | Ambil satu karyawan by id       |
| POST   | `/api/employees`          | Tambah karyawan baru (body JSON)|
| PUT    | `/api/employees/{id}`     | Update karyawan                 |
| DELETE | `/api/employees/{id}`     | Hapus karyawan                  |
| GET    | `/api/departments`        | Ambil semua departemen          |
| GET    | `/api/departments/summary`| Jumlah karyawan per departemen  |

Contoh body untuk `POST /api/employees`:
```json
{
  "fullName": "Hendra Wijaya",
  "email": "hendra.wijaya@torayems.co.id",
  "position": "System Analyst",
  "departmentId": 1,
  "hireDate": "2024-05-01",
  "salary": 8000000,
  "phoneNumber": "081234567890",
  "isActive": true
}
```