using NextInLine.Interfaces; 
using NextInLine.Services;
using NextInLine.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<ITurnService, TurnServiceImplementation>();

builder.Services.AddDbContext<MysqlDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysqlConnection"),
        new MySqlServerVersion(new Version(9, 0, 15))
    )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
