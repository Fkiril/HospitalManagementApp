﻿using Google.Cloud.Firestore;
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

        public static async void InitializeStaffData(FirestoreDb firestoreDb)
        {
            Console.WriteLine("InitializeStaffData");
            CollectionReference colRef = firestoreDb.Collection("Staff");

            QuerySnapshot snapshots = await colRef.GetSnapshotAsync();
            if (snapshots.Documents.Any())
            {
                return;
            }

            var staffs = new List<Staff>
            {
                new Staff
                {
                    Id = 1,
                    Name = "Doctor",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "09/08/2003",
                    Email = "doctor@gmail.com",
                    PhoneNum = "0769421007",
                    Degree = "abc",
                    Specialist = "abc",
                    Department = "abc",
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 2,
                    Name = "Nurse",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Nurse,
                    DateOfBirth = "04/02/2004",
                    Email = "nurse@gmail.com",
                    PhoneNum = "0987654312",
                    Degree = "abc",
                    Specialist = "abc",
                    Department = "abc",
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 3,
                    Name = "SupportStaff",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.SupportStaff,
                    DateOfBirth = "04/02/2001",
                    Email = "SupporStaff@gmail.com",
                    PhoneNum = "0231456978",
                    Degree = "abc",
                    Specialist = "abc",
                    Department = "abc",
                    WorkSchedule = null
                }
            };
        }
    }
}
    