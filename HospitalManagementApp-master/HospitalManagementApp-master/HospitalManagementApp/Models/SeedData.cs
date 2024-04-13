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

        public static async void InitializeDrugsData(FirestoreDb firestoreDb)
        {
            CollectionReference colRef = firestoreDb.Collection("drugs");

            QuerySnapshot snapshots = await colRef.GetSnapshotAsync();
            if (snapshots.Documents.Any())
            {
                return;
            }

            var drugs = new List<Drugs>
            {
                new Drugs
                {
                    Id = 20,
                    docId = "",
                    Name = "Ketamine",
                    Expiry = 90,
                    HisUse = 27,
                    Quantity = 10,
                    ReceiptDay = new DateTime(2023,10,17,12,0,0,DateTimeKind.Utc),
                    Status = Status.Used
                },
                new Drugs
                {
                    Id = 108,
                    docId = "",
                    Name = "Halothane",
                    Expiry = 10,
                    HisUse = 18,
                    Quantity = 3,
                    ReceiptDay = new DateTime(2023,2,8,12,0,0,DateTimeKind.Utc),
                    Status = Status.Old
                },
                new Drugs
                {
                    Id = 1,
                    docId = "",
                    Name = "Lidocaine",
                    Expiry = 60,
                    HisUse = 0,
                    Quantity = 2,
                    ReceiptDay = new DateTime(2023,12,10,12,0,0,DateTimeKind.Utc),
                    Status = Status.New
                }
            };

            foreach (var drug in drugs)
            {
                await colRef.AddAsync(drug);
            }
        }

        public static async void InitializePrescriptionData(FirestoreDb firestoreDb)
        {
            CollectionReference colRef = firestoreDb.Collection("Prescription");

            QuerySnapshot snapshots = await colRef.GetSnapshotAsync();
            if (snapshots.Documents.Any())
            {
                return;
            }

            var prescriptions = new List<Prescription>
            {
               
                
            };

            foreach (var prescription in prescriptions)
            {
                await colRef.AddAsync(prescription);
            }
        }

    }
}
    