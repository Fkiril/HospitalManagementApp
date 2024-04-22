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
                    Name = "Doctor5",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "09/08/2003",
                    Email = "doctor5@gmail.com",
                    PhoneNum = "0769421007",
                    Degree = "abc",
                    SpecialList = SpecialList.NoiTiet,
                    Department = Deparment.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 6,
                    Name = "Doctor6",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "02/04/2001",
                    Email = "doctor6@gmail.com",
                    PhoneNum = "0987651234",
                    Degree = "abc",
                    SpecialList = SpecialList.NoiTiet,
                    Department = Deparment.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 7,
                    Name = "Doctor7",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "10/10/2002",
                    Email = "doctor7@gmail.com",
                    PhoneNum = "0123459876",
                    Degree = "abc",
                    SpecialList = SpecialList.TieuHoa,
                    Department = Deparment.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 8,
                    Name = "Doctor8",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "01/01/2001",
                    Email = "doctor8@gmail.com",
                    PhoneNum = "0234561798",
                    Degree = "abc",
                    SpecialList = SpecialList.TieuHoa,
                    Department = Deparment.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 9,
                    Name = "Doctor9",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "09/08/2003",
                    Email = "doctor9@gmail.com",
                    PhoneNum = "0769421007",
                    Degree = "abc",
                    SpecialList = SpecialList.TieuHoa,
                    Department = Deparment.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 10,
                    Name = "Doctor10",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "02/04/2001",
                    Email = "doctoc10@gmail.com",
                    PhoneNum = "0987651234",
                    Degree = "abc",
                    SpecialList = SpecialList.HoHap,
                    Department = Deparment.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 11,
                    Name = "Doctor11",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "10/10/2002",
                    Email = "doctor11@gmail.com",
                    PhoneNum = "0123459876",
                    Degree = "abc",
                    SpecialList = SpecialList.HoHap,
                    Department = Deparment.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 12,
                    Name = "Doctor12",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "01/01/2001",
                    Email = "doctor12@gmail.com",
                    PhoneNum = "0234561798",
                    Degree = "abc",
                    SpecialList = SpecialList.HoHap,
                    Department = Deparment.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 13,
                    Name = "Doctor13",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "02/04/2001",
                    Email = "doctoc13@gmail.com",
                    PhoneNum = "0987651234",
                    Degree = "abc",
                    SpecialList = SpecialList.Mat,
                    Department = Deparment.NgoaiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 14,
                    Name = "Doctor14",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "10/10/2002",
                    Email = "doctor14@gmail.com",
                    PhoneNum = "0123459876",
                    Degree = "abc",
                    SpecialList = SpecialList.Mat,
                    Department = Deparment.NgoaiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 15,
                    Name = "Doctor15",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Nurse,
                    DateOfBirth = "01/01/2001",
                    Email = "doctor15@gmail.com",
                    PhoneNum = "0234561798",
                    Degree = "abc",
                    SpecialList = SpecialList.Mat,
                    Department = Deparment.NgoaiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 16,
                    Name = "Doctor16",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Nurse,
                    DateOfBirth = "02/04/2001",
                    Email = "doctoc16@gmail.com",
                    PhoneNum = "0987651234",
                    Degree = "abc",
                    SpecialList = SpecialList.TaiMuiHong,
                    Department = Deparment.NgoaiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 17,
                    Name = "Doctor17",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "10/10/2002",
                    Email = "doctor17@gmail.com",
                    PhoneNum = "0123459876",
                    Degree = "abc",
                    SpecialList = SpecialList.TaiMuiHong,
                    Department = Deparment.NgoaiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 18,
                    Name = "Doctor18",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "01/01/2001",
                    Email = "doctor18@gmail.com",
                    PhoneNum = "0234561798",
                    Degree = "abc",
                    SpecialList = SpecialList.TaiMuiHong,
                    Department = Deparment.NgoaiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 19,
                    Name = "Doctor19",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Nurse,
                    DateOfBirth = "02/04/1980",
                    Email = "doctoc19@gmail.com",
                    PhoneNum = "0987651234",
                    Degree = "abc",
                    SpecialList = SpecialList.NgoaiChinhHinh,
                    Department = Deparment.NgoaiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 20,
                    Name = "Doctor14",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "10/10/1979",
                    Email = "doctor14@gmail.com",
                    PhoneNum = "0123459876",
                    Degree = "abc",
                    SpecialList = SpecialList.NgoaiChinhHinh,
                    Department = Deparment.NgoaiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 21,
                    Name = "Doctor21",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "01/01/1988",
                    Email = "doctor21@gmail.com",
                    PhoneNum = "0234561798",
                    Degree = "abc",
                    SpecialList = SpecialList.NgoaiChinhHinh,
                    Department = Deparment.NgoaiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 22,
                    Name = "Doctor22",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "02/04/1981",
                    Email = "doctoc22@gmail.com",
                    PhoneNum = "0987651234",
                    Degree = "abc",
                    SpecialList = SpecialList.XQuang,
                    Department = Deparment.ChanDoan,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 23,
                    Name = "Doctor23",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "10/10/1979",
                    Email = "doctor23@gmail.com",
                    PhoneNum = "0123459876",
                    Degree = "abc",
                    SpecialList = SpecialList.XQuang,
                    Department = Deparment.ChanDoan,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 24,
                    Name = "Doctor24",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "01/01/1986",
                    Email = "doctor24@gmail.com",
                    PhoneNum = "0234561798",
                    Degree = "abc",
                    SpecialList = SpecialList.XQuang,
                    Department = Deparment.ChanDoan,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 25,
                    Name = "Doctor25",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Nurse,
                    DateOfBirth = "02/04/1981",
                    Email = "doctoc25@gmail.com",
                    PhoneNum = "0987651234",
                    Degree = "abc",
                    SpecialList = SpecialList.PhuKhoa,
                    Department = Deparment.SanPhu,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 26,
                    Name = "Doctor26",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "10/10/1970",
                    Email = "doctor26@gmail.com",
                    PhoneNum = "0123459876",
                    Degree = "abc",
                    SpecialList = SpecialList.PhuKhoa,
                    Department = Deparment.SanPhu,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 27,
                    Name = "Doctor27",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "01/01/1985",
                    Email = "doctor27@gmail.com",
                    PhoneNum = "0234561798",
                    Degree = "abc",
                    SpecialList = SpecialList.PhuKhoa,
                    Department = Deparment.SanPhu,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 28,
                    Name = "Doctor28",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Nurse,
                    DateOfBirth = "10/10/1990",
                    Email = "doctor28@gmail.com",
                    PhoneNum = "0123459876",
                    Degree = "abc",
                    SpecialList = SpecialList.ThaiNhi,
                    Department = Deparment.SanPhu,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 29,
                    Name = "Doctor29",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "01/01/1991",
                    Email = "doctor29@gmail.com",
                    PhoneNum = "0234561798",
                    Degree = "abc",
                    SpecialList = SpecialList.ThaiNhi,
                    Department = Deparment.SanPhu,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 30,
                    Name = "Doctor30",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "02/04/1992",
                    Email = "doctoc30@gmail.com",
                    PhoneNum = "0987651234",
                    Degree = "abc",
                    SpecialList = SpecialList.ThaiNhi,
                    Department = Deparment.SanPhu,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 31,
                    Name = "Doctor31",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Nurse,
                    DateOfBirth = "10/10/1990",
                    Email = "doctor31@gmail.com",
                    PhoneNum = "0123459876",
                    Degree = "abc",
                    SpecialList = SpecialList.SinhDuc,
                    Department = Deparment.SanPhu,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 32,
                    Name = "Doctor32",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "01/01/1991",
                    Email = "doctor32@gmail.com",
                    PhoneNum = "0234561798",
                    Degree = "abc",
                    SpecialList = SpecialList.SinhDuc,
                    Department = Deparment.SanPhu,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 33,
                    Name = "Doctor33",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "02/04/1992",
                    Email = "doctoc33@gmail.com",
                    PhoneNum = "0987651234",
                    Degree = "abc",
                    SpecialList = SpecialList.SinhDuc,
                    Department = Deparment.SanPhu,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 35,
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
                    Id = 36,
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
    