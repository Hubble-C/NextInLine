using NextInLine.Interfaces;
using NextInLine.Services;
using NextInLine.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ITurnService, TurnServiceImplementation>();
builder.Services.AddSingleton<PrinterService>(); // ← nuevo

builder.Services.AddDbContext<MysqlDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysqlConnection"),
        new MySqlServerVersion(new Version(9, 0, 15))
    )
);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();