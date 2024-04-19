using HospitalManagementApp.Data;
using HospitalManagementApp.Services;
using HospitalManagementApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<FirestoreDbService>(provider =>
{
    var projectId = "hospitalmanagementapp-7aa23";
    string rootPath = provider.GetRequiredService<IWebHostEnvironment>().ContentRootPath;
    var jsonFilePath = Path.Combine(rootPath,"" ,"hospitalmanagementapp-7aa23-firebase-adminsdk-gw6ir-355e13d856.json");

    Google.Apis.Auth.OAuth2.GoogleCredential credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(jsonFilePath);
    return new FirestoreDbService(projectId, credential);
});
builder.Services.AddSingleton<PatientContext>(sp =>
{
    var firestoreDbService = sp.GetRequiredService<FirestoreDbService>();
    return new PatientContext(firestoreDbService, null);
});

builder.Services.AddSingleton<ApplicationUserContext>();

builder.Services.AddSingleton<DrugsContext>();

builder.Services.AddSingleton<PrescriptionContext>();

builder.Services.AddSingleton<StaffContext>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Authentication/Login";
            options.Cookie.Name = "MyCookie";
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            options.SlidingExpiration = true;
            //options.Events = new CookieAuthenticationEvents
            //{
            //    OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
            //};
        });

builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var firestoreDbService = scope.ServiceProvider.GetService<FirestoreDbService>();
    if (firestoreDbService != null)
    {
        SeedData.InitializePatientData(firestoreDbService.GetFirestoreDb());
        //SeedData.InitializeApplicationUserData(firestoreDbService.GetFirestoreDb());
    }
}

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
app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Equipment}/{action=Index}/{id?}");

app.Run();
