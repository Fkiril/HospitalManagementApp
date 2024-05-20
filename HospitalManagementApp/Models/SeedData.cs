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
                    Email = string.Empty,
                    MedicalHistory = null,
                    TestResult = new TestResult
                    {
                        Disease = "abc",
                        Type = SpecialList.TieuHoa,
                        StartDate = "23/04/2024"
                    },
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
                    Email = string.Empty,
                    MedicalHistory = null,
                    TestResult = new TestResult
                    {
                        Disease = "abc",
                        Type = SpecialList.TimMach,
                        StartDate = "23/04/2024"
                    },
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

            Console.WriteLine("InitializeApplicationUserData");
            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = "0",
                    UserName = "Admin",
                    Email = "admin@gmail.com",
                    Password = "admin123",
                    Role = "Admin",
                    DataId = 0
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
                    Name = "Philippe Macaire",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "09/08/2003",
                    Email = "doctor1@gmail.com",
                    PhoneNum = "0769421007",
                    Degree = "Bachelor of Medicine",
                    SpecialList = SpecialList.TimMach,
                    Department = Department.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 2,
                    Name = "Reinel Martin Alvarez Plasencia",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "02/04/2001",
                    Email = "doctor2@gmail.com",
                    PhoneNum = "0987651234",
                    Degree = "Bachelor of Medicine",
                    SpecialList = SpecialList.TimMach,
                    Department = Department.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 3,
                    Name = "Nguyễn Thanh Liêm",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "10/10/2002",
                    Email = "doctor3@gmail.com",
                    PhoneNum = "0123459876",
                    Degree = "Bachelor of Medicine",
                    SpecialList = SpecialList.TimMach,
                    Department = Department.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 4,
                    Name = "Võ Thành Nhân",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "01/01/2001",
                    Email = "doctor4@gmail.com",
                    PhoneNum = "0234561798",
                    Degree = "Bachelor of Medicine",
                    SpecialList = SpecialList.NoiTiet,
                    Department = Department.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 5,
                    Name = "Phạm Nhật An",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "09/08/2003",
                    Email = "doctor5@gmail.com",
                    PhoneNum = "0769421007",
                    Degree = "Bachelor of Medicine",
                    SpecialList = SpecialList.NoiTiet,
                    Department = Department.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 6,
                    Name = "Phan Quỳnh Lan",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "02/04/2001",
                    Email = "doctor6@gmail.com",
                    PhoneNum = "0987651234",
                    Degree = "Bachelor of Medicine",
                    SpecialList = SpecialList.NoiTiet,
                    Department = Department.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 7,
                    Name = "Nguyễn Thị Tân Sinh",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "10/10/2002",
                    Email = "doctor7@gmail.com",
                    PhoneNum = "0123459876",
                    Degree = "Bachelor of Medicine",
                    SpecialList = SpecialList.TieuHoa,
                    Department = Department.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 8,
                    Name = "Tôn Thất Trí Dũng",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "01/01/2001",
                    Email = "doctor8@gmail.com",
                    PhoneNum = "0234561798",
                    Degree = "Bachelor of Medicine",
                    SpecialList = SpecialList.TieuHoa,
                    Department = Department.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 9,
                    Name = "Quách Thanh Dung",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "09/08/2003",
                    Email = "doctor9@gmail.com",
                    PhoneNum = "0769421007",
                    Degree = "Bachelor of Medicine",
                    SpecialList = SpecialList.TieuHoa,
                    Department = Department.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 10,
                    Name = "Ngô Văn Đoan",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "02/04/2001",
                    Email = "doctoc10@gmail.com",
                    PhoneNum = "0987651234",
                    Degree = "Bachelor of Medicine",
                    SpecialList = SpecialList.HoHap,
                    Department = Department.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 11,
                    Name = "Nguyễn Tất Bình",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "10/10/2002",
                    Email = "doctor11@gmail.com",
                    PhoneNum = "0123459876",
                    Degree = "Bachelor of Medicine",
                    SpecialList = SpecialList.HoHap,
                    Department = Department.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 12,
                    Name = "Nguyễn Văn Quyết",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "01/01/2001",
                    Email = "doctor12@gmail.com",
                    PhoneNum = "0234561798",
                    Degree = "Bachelor of Medicine",
                    SpecialList = SpecialList.HoHap,
                    Department = Department.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 13,
                    Name = "Đoàn Thị Hồng Hạnh",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "02/04/2001",
                    Email = "doctoc13@gmail.com",
                    PhoneNum = "0987651234",
                    Degree = "Bachelor of Medicine",
                    SpecialList = SpecialList.Mat,
                    Department = Department.NgoaiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 14,
                    Name = "Thái Bằng",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "10/10/2002",
                    Email = "doctor14@gmail.com",
                    PhoneNum = "0123459876",
                    Degree = "Bachelor of Medicine",
                    SpecialList = SpecialList.Mat,
                    Department = Department.NgoaiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 15,
                    Name = "Nguyễn Thái Trí",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Nurse,
                    DateOfBirth = "01/01/2001",
                    Email = "doctor15@gmail.com",
                    PhoneNum = "0234561798",
                    Degree = "Bachelor of Medicine",
                    SpecialList = SpecialList.Mat,
                    Department = Department.NgoaiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 16,
                    Name = "Lê Trọng Bình",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Nurse,
                    DateOfBirth = "02/04/2001",
                    Email = "doctoc16@gmail.com",
                    PhoneNum = "0987651234",
                    Degree = "Bachelor of Medicine",
                    SpecialList = SpecialList.TaiMuiHong,
                    Department = Department.NgoaiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 17,
                    Name = "Đoàn Trung Hiệp",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "10/10/2002",
                    Email = "doctor17@gmail.com",
                    PhoneNum = "0123459876",
                    Degree = "Bachelor of Medicine",
                    SpecialList = SpecialList.TaiMuiHong,
                    Department = Department.NgoaiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 18,
                    Name = "Bùi Tiến Đạt",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "01/01/2001",
                    Email = "doctor18@gmail.com",
                    PhoneNum = "0234561798",
                    Degree = "Bachelor of Medicine",
                    SpecialList = SpecialList.TaiMuiHong,
                    Department = Department.NgoaiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 19,
                    Name = "Phạm Thị Thùy Nhung",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Nurse,
                    DateOfBirth = "02/04/1980",
                    Email = "doctoc19@gmail.com",
                    PhoneNum = "0987651234",
                    Degree = "Associate Professor",
                    SpecialList = SpecialList.NgoaiChinhHinh,
                    Department = Department.NgoaiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 20,
                    Name = "Bùi Minh Đức",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "10/10/1979",
                    Email = "doctor14@gmail.com",
                    PhoneNum = "0123459876",
                    Degree = "Associate Professor",
                    SpecialList = SpecialList.NgoaiChinhHinh,
                    Department = Department.NgoaiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 21,
                    Name = "Nguyễn Thanh Hưng",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "01/01/1988",
                    Email = "doctor21@gmail.com",
                    PhoneNum = "0234561798",
                    Degree = "Associate Professor",
                    SpecialList = SpecialList.NgoaiChinhHinh,
                    Department = Department.NgoaiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 22,
                    Name = "Huỳnh Thoại Loan",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "02/04/1981",
                    Email = "doctoc22@gmail.com",
                    PhoneNum = "0987651234",
                    Degree = "Associate Professor",
                    SpecialList = SpecialList.XQuang,
                    Department = Department.ChanDoan,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 23,
                    Name = "Đoàn Xuân Sinh",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "10/10/1979",
                    Email = "doctor23@gmail.com",
                    PhoneNum = "0123459876",
                    Degree = "Associate Professor",
                    SpecialList = SpecialList.XQuang,
                    Department = Department.ChanDoan,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 24,
                    Name = "Nguyễn Việt Anh",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "01/01/1986",
                    Email = "doctor24@gmail.com",
                    PhoneNum = "0234561798",
                    Degree = "Associate Professor",
                    SpecialList = SpecialList.XQuang,
                    Department = Department.ChanDoan,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 25,
                    Name = "Nguyễn Thị Hoàn",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Nurse,
                    DateOfBirth = "02/04/1981",
                    Email = "doctoc25@gmail.com",
                    PhoneNum = "0987651234",
                    Degree = "Associate Professor",
                    SpecialList = SpecialList.PhuKhoa,
                    Department = Department.SanPhu,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 26,
                    Name = "Nguyễn Đăng Tuân",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "10/10/1970",
                    Email = "doctor26@gmail.com",
                    PhoneNum = "0123459876",
                    Degree = "Professor",
                    SpecialList = SpecialList.PhuKhoa,
                    Department = Department.SanPhu,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 27,
                    Name = "Trần Như Tú",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "01/01/1985",
                    Email = "doctor27@gmail.com",
                    PhoneNum = "0234561798",
                    Degree = "Associate Professor",
                    SpecialList = SpecialList.PhuKhoa,
                    Department = Department.SanPhu,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 28,
                    Name = "Trần Liên Anh",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Nurse,
                    DateOfBirth = "10/10/1990",
                    Email = "doctor28@gmail.com",
                    PhoneNum = "0123459876",
                    Degree = "Doctor of Medicine",
                    SpecialList = SpecialList.ThaiNhi,
                    Department = Department.SanPhu,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 29,
                    Name = "Khổng Tiến Đạt",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "01/01/1991",
                    Email = "doctor29@gmail.com",
                    PhoneNum = "0234561798",
                    Degree = "Doctor of Medicine",
                    SpecialList = SpecialList.ThaiNhi,
                    Department = Department.SanPhu,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 30,
                    Name = "Trần Thị Linh Chi",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "02/04/1992",
                    Email = "doctoc30@gmail.com",
                    PhoneNum = "0987651234",
                    Degree = "Doctor of Medicine",
                    SpecialList = SpecialList.ThaiNhi,
                    Department = Department.SanPhu,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 31,
                    Name = "Lê Hữu Đồng",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Nurse,
                    DateOfBirth = "10/10/1990",
                    Email = "doctor31@gmail.com",
                    PhoneNum = "0123459876",
                    Degree = "Doctor of Medicine",
                    SpecialList = SpecialList.SinhDuc,
                    Department = Department.SanPhu,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 32,
                    Name = "Lê Viết Cường",
                    Gender = Gender.Male,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "01/01/1991",
                    Email = "doctor32@gmail.com",
                    PhoneNum = "0234561798",
                    Degree = "Doctor of Medicine",
                    SpecialList = SpecialList.SinhDuc,
                    Department = Department.SanPhu,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 33,
                    Name = "Trần Hữu Tuấn",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Doctor,
                    DateOfBirth = "02/04/1992",
                    Email = "doctoc33@gmail.com",
                    PhoneNum = "0987651234",
                    Degree = "Doctor of Medicine",
                    SpecialList = SpecialList.SinhDuc,
                    Department = Department.SanPhu,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 35,
                    Name = "Phùng Tuyết Lan",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.Nurse,
                    DateOfBirth = "04/02/2004",
                    Email = "nurse@gmail.com",
                    PhoneNum = "0987654312",
                    Degree = "Bachelor of Medicine",
                    SpecialList = SpecialList.TieuHoa,
                    Department = Department.NoiKhoa,
                    WorkSchedule = null
                },
                new Staff
                {
                    Id = 36,
                    Name = " Bùi Thu Hương",
                    Gender = Gender.Female,
                    HealthCareStaff = HealthCareStaff.SupportStaff,
                    DateOfBirth = "04/02/2001",
                    Email = "SupporStaff@gmail.com",
                    PhoneNum = "0231456978",
                    Degree = "Bachelor of Medicine",
                    SpecialList = null,
                    Department = Department.QuanLy,
                    WorkSchedule = null
                }
            };
            foreach (var staff in staffs) {
                await colRef.Document("staff_" + staff.Id).SetAsync(staff);

            }
        }
    }
}
    