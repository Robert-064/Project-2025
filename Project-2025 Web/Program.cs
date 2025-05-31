using Microsoft.EntityFrameworkCore;
using Project_2025_Web.Data;
using Project_2025_Web.Services;
using AutoMapper;
using Project_2025_Web.Data.Entities;
using static Project_2025_Web.Services.AuthorizePermissionAttribute;

var builder = WebApplication.CreateBuilder(args);

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Servicios
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();


// DbContext
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Autenticación
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

builder.Services.AddAuthorization();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Añadir datos de ejemplo
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();

    if (!context.Plans.Any())
    {
        var plan1 = new Plan { Name = "Aventura Andina", Description = "Tour en montaña", Basic_Price = 100, Type_Difficulty = 3, Max_Persons = 10, Distance = 8, ImageUrl1 = "/images/perro (1).jpeg", ImageUrl2 = "/images/perro (2).jpeg" };
        var plan2 = new Plan { Name = "Relax en la Playa", Description = "Día completo de playa", Basic_Price = 150, Type_Difficulty = 1, Max_Persons = 20, Distance = 2, ImageUrl1 = "/images/velo (1).jpeg", ImageUrl2 = "/images/velo (2).jpeg" };
        var plan3 = new Plan { Name = "Selva Explorada", Description = "Caminata guiada en la selva", Basic_Price = 120, Type_Difficulty = 4, Max_Persons = 8, Distance = 10, ImageUrl1 = "/images/bote (1).jpeg", ImageUrl2 = "/images/bote (2).jpeg" };

        context.Plans.AddRange(plan1, plan2, plan3);
        await context.SaveChangesAsync();

        var reservation1 = new Reservation { Id_Plan = plan1.Id, Id_User = 1, Date = DateTime.Now.AddDays(1), Status = "Activa", Person_Number = 2 };
        var reservation2 = new Reservation { Id_Plan = plan2.Id, Id_User = 2, Date = DateTime.Now.AddDays(2), Status = "Activa", Person_Number = 4 };

        context.Reservations.AddRange(reservation1, reservation2);
        await context.SaveChangesAsync();
    }
}

app.Run();
