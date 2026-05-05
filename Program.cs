using NextInLine.Interfaces;
using NextInLine.Services;
using NextInLine.Data;
using Microsoft.EntityFrameworkCore;
using NextInLine.Settings;
using NextInLine.TurnsHub;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<ITurnService, TurnServiceImplementation>();
builder.Services.AddSingleton<PrinterService>();

builder.Services.AddDbContext<MysqlDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysqlConnection"),
        new MySqlServerVersion(new Version(9, 0, 15))
    )
);

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings")
);
builder.Services.AddSignalR();

var app = builder.Build();
/*This class to render on live page turns */
app.MapHub<TurnHub>("/turnHub");

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