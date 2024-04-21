using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;
using HospitalManagementApp.Data;

namespace HospitalManagementApp.Models
{
    public class SeedData
    {
        public static async void InitializePatientData(FirestoreDb firestoreDb)
        {
            Console.WriteLine("InitializePatientData");
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
                    DateOfBirth = "09/08/2003",
                    Address = "abc",
                    PhoneNum = "0769421007",
                    MedicalHistory = null,
                    TestResult = null,
                    StaffIds = [1, 2],
                    TreatmentSchedule =
                    [
                        new TreatmentScheduleEle
                        {
                            Id = 1,
                            Date = "09/04/2024",
                            StartTime = "10:00",
                            EndTime = "12:00",
                        }
                    ],
                    Status = PatientStatus.Ill,
                    PrescriptionId = null
                },
                new Patient
                {
                    Id = 2,
                    Name = "Violet",
                    Gender = Gender.Female,
                    DateOfBirth = "03/03/2003",
                    Address = "abc",
                    PhoneNum = "0969185801",
                    MedicalHistory = null,
                    TestResult = null,
                    StaffIds = [3, 4],
                    TreatmentSchedule =
                    [
                        new TreatmentScheduleEle
                        {
                            Id = 1,
                            Date = "10/04/2024",
                            StartTime = "08:00",
                            EndTime = "12:00",
                        }
                    ],
                    Status = PatientStatus.Ill,
                    PrescriptionId = null
                }
            };

            foreach (var patient in patients)
            {
                await colRef.Document("patient_" + patient.Id).SetAsync(patient);
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
                    Role = "Admin"
                }
            };

            foreach (var user in users)
            {
                await colRef.Document("applicationuser_" + user.Id).SetAsync(user);
            }
        }
    }
}
    