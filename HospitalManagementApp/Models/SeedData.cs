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
                    docId = "",
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
                    docId = "",
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
                await colRef.AddAsync(patient);
            }
        }

        public static async void InitializeApplicationUserData(FirestoreDb firestoreDb)
        {
            CollectionReference colRef = firestoreDb.Collection("ApplicationUser");
            QuerySnapshot snapshots = await colRef.GetSnapshotAsync();
            if (snapshots.Documents.Any())
            {
                return;
            }

            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = "1",
                    UserName = "KiriL",
                    Email = "abc.gmail.com",
                    Password = "abcdef123",
                    Role = "admin"
                }
            };

            foreach (var user in users)
            {
                await colRef.Document("applicationuser_" + user.Id).SetAsync(user);
            }
        }
    }
}
    