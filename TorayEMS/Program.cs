using TorayEMS.Data;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. Register services ke dalam DI Container
// ============================================================

// MVC (Controller + Views)
builder.Services.AddControllersWithViews();

// Web API (Controller only, menghasilkan JSON)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger untuk dokumentasi & testing API secara interaktif
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Toray EMS API",
        Version = "v1",
        Description = "REST API untuk Employee Management System (demo skill C#, .NET, MVC, Web API, SQL Server Stored Procedures)"
    });
});

// Registrasi Connection Factory & Repository (Dependency Injection)
// Semua akses data dilakukan lewat ADO.NET + Stored Procedure, bukan Entity Framework,
// supaya sesuai requirement "Good knowledge of SQL Server and Stored Procedures".
builder.Services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

var app = builder.Build();

// ============================================================
// 2. Konfigurasi HTTP request pipeline
// ============================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // Swagger hanya diaktifkan di mode Development
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Toray EMS API v1"));
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Route default untuk MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Route untuk Web API Controllers (attribute routing: [Route("api/[controller]")])
app.MapControllers();

app.Run();
