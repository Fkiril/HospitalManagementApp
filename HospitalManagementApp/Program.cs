using Google.Cloud.Firestore;
using Microsoft.Extensions.DependencyInjection;
using HospitalManagementApp.Data;
using HospitalManagementApp.Services;
using Google.Api;
using HospitalManagementApp.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<FirestoreDbService>(provider =>
{
    var projectId = "hospitalmanagementapp-7aa23";
    var jsonFilePath = "D:\\HCMUT\\232\\Advanced programming\\Assignment\\HospitalManagementApp\\HospitalManagementApp\\hospitalmanagementapp-7aa23-firebase-adminsdk-gw6ir-355e13d856.json";
    var jsonCreadentials = File.ReadAllText(jsonFilePath);
    return new FirestoreDbService(projectId, jsonCreadentials);
});
builder.Services.AddSingleton<PatientContext>();
builder.Services.AddSingleton<StaffContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var firestoreDbService = scope.ServiceProvider.GetService<FirestoreDbService>();
    if (firestoreDbService != null )
    {
        SeedData.InitializePatientData(firestoreDbService.GetFirestoreDb());
        SeedData.InitializeStaffData(firestoreDbService.GetFirestoreDb());
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Staff}/{action=Index}/{id?}");

app.Run();
