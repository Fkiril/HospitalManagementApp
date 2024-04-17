using HospitalManagementApp.Data;
using HospitalManagementApp.Models;
using HospitalManagementApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<FirestoreDbService>(provider =>
{
	var projectId = /*"hospitalmanagementapp-7aa23"*/"hospitalmanagement-6e649";


	return new FirestoreDbService(projectId);
});
builder.Services.AddSingleton<PrescriptionContext>();
builder.Services.AddSingleton<DrugsContext>();
builder.Services.AddSingleton<PatientContext>();
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var firestoreDbService = scope.ServiceProvider.GetService<FirestoreDbService>();
	if (firestoreDbService != null)
		SeedData./*InitializePatientData*/InitializePatientData(firestoreDbService.GetFirestoreDb());
	SeedData.InitializeDrugsData(firestoreDbService.GetFirestoreDb());
	SeedData.InitializePrescriptionData(firestoreDbService.GetFirestoreDb());
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
	pattern: "{controller=Drugs}/{action=Index}/{id?}");

app.Run();
