using GestionnaireOrthophonie.Data;
using GestionnaireOrthophonie.Models;
using GestionnaireOrthophonie.Models.ModelValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<DatabaseContext>();

builder.Services.AddScoped<PatientsOperationsManager>();
builder.Services.AddScoped<PlanningOperationsManager>();
builder.Services.AddTransient<PatientAddingActionFilterAttribute>();
builder.Services.AddTransient<PatientIdVerificationActionFilterAttribute>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}"
);
app.MapRazorPages();

app.Run();
