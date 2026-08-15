using AdedonhaMVC.Common.Data;
using AdedonhaMVC.Common.Data.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<Contexto>
    (options => options.UseSqlServer("Data Source=DESKTOP-JTHDJFE\\SQLSERVER;Initial Catalog=AdedonhaMVC2;Integrated Security=True;TrustServerCertificate=True"));

var app = builder.Build();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var seedScope = app.Services.CreateScope())
{
    var context = seedScope.ServiceProvider.GetRequiredService<Contexto>();
    var csvPath = Path.Combine(AppContext.BaseDirectory, "Common", "Data", "Seed", "adedonha_palavras.csv");

    if (File.Exists(csvPath))
    {
        await CsvPalavraSeeder.SeedAsync(context, csvPath);
    }
    else
    {
        app.Logger.LogWarning("Arquivo de seed não encontrado em {CsvPath}; seed ignorado.", csvPath);
    }
}

app.Run();
