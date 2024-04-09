using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;
using HospitalManagementApp.Data;

namespace HospitalManagementApp.Models
{
    public class SeedData
    {
        public static async void InitializePatientData(FirestoreDb firestoreDb)
        {
            CollectionReference colRef = firestoreDb.Collection("Patient");

            QuerySnapshot snapshots = await colRef.GetSnapshotAsync();
            if (snapshots.Documents.Any())
            {
                return;
            }

            var patients = new List<Patient>
            {
                new Patient
                {
                    Id = 1,
                    Name = "KiriL",
                    Gender = Gender.Male,
                    DateOfBirth = new DateTime(2003,8,9,0,0,0, DateTimeKind.Utc),
                    Address = "abc",
                    PhoneNum = "0769421007",
                    MedicalHistory = "abc",
                    TestResult = "abc",
                    TreatmentSchedule = "abc",
                    Status = "abc"
                },
                new Patient
                {
                    Id = 2,
                    Name = "Violet",
                    Gender = Gender.Female,
                    DateOfBirth = new DateTime(2003,3,3,0,0,0, DateTimeKind.Utc),
                    Address = "abc",
                    PhoneNum = "0969185801",
                    MedicalHistory = "abc",
                    TestResult = "abc",
                    TreatmentSchedule = "abc",
                    Status = "abc"
                }
            };

            foreach (var patient in patients)
            {
                await colRef.Document("patient_" + patient.Id.ToString()).SetAsync(patient);

            }
        }

        public static async void InitializeStaffData(FirestoreDb firestoreDb)
        {
            CollectionReference colRef = firestoreDb.Collection("Staff");

            QuerySnapshot snapshots = await colRef.GetSnapshotAsync();
            if (snapshots.Documents.Any())
            {
                return;
            }

            var staffs = new List<Staff>
            {
                new() {
                    Id = 1,
                    Name = "Doctor",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = new DateTime(2003,8,9,0,0,0, DateTimeKind.Utc),
                    Email = "doctor@gmail.com",
                    PhoneNum = "0769421007",
                    Degree = "abc",
                    Specialist = "abc",
                    Department = "abc",
                    WorkSchedule = Staff.CreateCalendar(),
                    Status = "abc"
                },
                new() {
                    Id = 2,
                    Name = "Nurse",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Nurse,
                    DateOfBirth = new DateTime(2002,10,9,0,0,0, DateTimeKind.Utc),
                    Email = "nurse@gmail.com",
                    PhoneNum = "0987654321",
                    Degree = "abc",
                    Specialist = "abc",
                    Department = "abc",
                    WorkSchedule = Staff.CreateCalendar(),
                    Status = "abc"
                },
                new() {
                    Id = 3,
                    Name = "SupportStaff",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.SupportStaff,
                    DateOfBirth = new DateTime(2004,7,4,0,0,0, DateTimeKind.Utc),
                    Email = "SupportStaff@gmail.com",
                    PhoneNum = "0987654321",
                    Degree = "abc",
                    Specialist = "abc",
                    Department = "abc",
                    WorkSchedule = Staff.CreateCalendar(),
                    Status = "abc"
                }
            };

            foreach (var staff in staffs)
            {
                await colRef.Document("staff_" + staff.Id.ToString()).SetAsync(staff);

            }
        }
    }
}
    