using Microsoft.EntityFrameworkCore;
using OrganizadorDocumentacion.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DocnizerContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("conexion")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddSession(); // Agregar servicios de sesión

var app = builder.Build();
/*builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Duración de la sesión
    options.Cookie.HttpOnly = true; // Asegura que la cookie de sesión solo sea accesible a través de HTTP
    options.Cookie.IsEssential = true; // Hacer que la cookie de sesión sea esencial para la aplicación
});*/

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Habilitar el middleware de sesión
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
