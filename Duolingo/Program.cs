using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Duolingo.Data;
using Duolingo.Areas.Identity.Data;
using Duolingo.Services;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DuolingoContextConnection") ?? throw new InvalidOperationException("Connection string 'DuolingoContextConnection' not found.");;

builder.Services.AddDbContext<DuolingoContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<DuolingoUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<DuolingoContext>();
builder.Services.AddScoped<AiService>();
builder.Services.AddScoped<GamificationService>();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
var app = builder.Build();
app.UseAuthentication(); // Ensure this is added before authorization
app.MapRazorPages(); // Dodaje obs�ug� stron Identity
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

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();

    app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
