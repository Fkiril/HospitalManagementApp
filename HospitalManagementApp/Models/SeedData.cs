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

            Console.WriteLine("InitializePatientData");

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
            CollectionReference colRef = firestoreDb.Collection("Staff");

            QuerySnapshot snapshots = await colRef.GetSnapshotAsync();
            if (snapshots.Documents.Any())
            {
                return;
            }

            Console.WriteLine("InitializeStaffData");
            var staffs = new List<Staff>
            {
                new Staff
                {
                    Id = 1,
                    Name = "Doctor1",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "09/08/2003",
                    Email = "doctor1@gmail.com",
                    PhoneNum = "0769421007",
                    Degree = "abc",
                    SpecialList = SpecialList.TimMach,
                    Department = Deparment.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 2,
                    Name = "Doctor2",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "02/04/2001",
                    Email = "doctor2@gmail.com",
                    PhoneNum = "0987651234",
                    Degree = "abc",
                    SpecialList = SpecialList.TimMach,
                    Department = Deparment.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 3,
                    Name = "Doctor3",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "10/10/2002",
                    Email = "doctor3@gmail.com",
                    PhoneNum = "0123459876",
                    Degree = "abc",
                    SpecialList = SpecialList.TimMach,
                    Department = Deparment.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 4,
                    Name = "Doctor4",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "01/01/2001",
                    Email = "doctor4@gmail.com",
                    PhoneNum = "0234561798",
                    Degree = "abc",
                    SpecialList = SpecialList.NoiTiet,
                    Department = Deparment.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 5,
                    Name = "Nurse",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Nurse,
                    DateOfBirth = "04/02/2004",
                    Email = "nurse@gmail.com",
                    PhoneNum = "0987654312",
                    Degree = "abc",
                    SpecialList = SpecialList.TieuHoa,
                    Department = Deparment.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 6,
                    Name = "SupportStaff",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.SupportStaff,
                    DateOfBirth = "04/02/2001",
                    Email = "SupporStaff@gmail.com",
                    PhoneNum = "0231456978",
                    Degree = "abc",
                    SpecialList = null,
                    Department = Deparment.QuanLy,
                    WorkSchedule = null
                }
            };
            foreach (var staff in staffs) {
                await colRef.Document("staff_" + staff.Id).SetAsync(staff);
            }
        }
    }
}
    