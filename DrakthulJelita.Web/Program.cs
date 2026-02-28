using DrakthulJelita.Web.Configuration;
using DrakthulJelita.Web.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DbPath") ??
                       throw new InvalidOperationException("`DbPath` connection string not found");

builder.Services
    .AddDbContext<AppDbContext>(options =>
        options.UseSqlite(connectionString)
    );

builder.Services
    .AddControllersWithViews();

builder.Services
    .AddOptions<CdnOptions>()
    .Bind(builder.Configuration.GetSection(CdnOptions.SectionName))
    .Validate(o => !string.IsNullOrWhiteSpace(o.BaseUrl), "Cdn:BaseUrl must be set")
    .ValidateOnStart();

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

app.MapStaticAssets();

app.MapControllerRoute(
        "default",
        "{controller=Screenshots}/{action=Index}/{id?}"
    )
    .WithStaticAssets();


app.Run();