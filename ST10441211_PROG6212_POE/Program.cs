using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ST10441211_PROG6212_POE;
using ST10441211_PROG6212_POE.Data;

var builder = WebApplication.CreateBuilder(args);

// ----------------- Connection String -----------------
string connectionString = "Server=(localdb)\\mssqllocaldb;Database=ClaimsManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true";

// ----------------- Services -----------------
builder.Services.AddControllersWithViews();

// Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Enable session and distributed cache
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register IHttpContextAccessor (needed for SessionManager)
builder.Services.AddHttpContextAccessor();

// Register SessionManager as singleton
builder.Services.AddSingleton<SessionManager>();

// ----------------- Build App -----------------
var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

// ----------------- Middleware Pipeline -----------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable session
app.UseSession();

app.UseAuthorization();

// ----------------- Default Route -----------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
