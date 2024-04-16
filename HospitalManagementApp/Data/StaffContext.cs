using Google.Api;
using Google.Api.Gax;
using Google.Cloud.Firestore;
using HospitalManagementApp.Models;
using HospitalManagementApp.Services;
using System;

namespace HospitalManagementApp.Data
{
    public class StaffContext
    {
        public readonly FirestoreDb _firestoreDb;
        public static ICollection<Staff> StaffList { get; private set; } = default!;
        public StaffContext(FirestoreDbService firestoreDbService, ICollection<Staff>? staffList = null)
        {
            _firestoreDb = firestoreDbService.GetFirestoreDb();
            StaffList = (staffList != null) ? staffList : [];
        }
        private string docId = "staff_";
        public async Task InitializeStaffListFromFirestore()
        {
            Console.WriteLine("InitializeStaffListFromFirestore");
            if (StaffList.Count != 0)
            {
                return;
            }
            QuerySnapshot snapshotQuery = await _firestoreDb.Collection("Staff").GetSnapshotAsync();

            foreach (DocumentSnapshot docSnapshot in snapshotQuery.Documents)
            {
                Staff staff = docSnapshot.ConvertTo<Staff>();
                StaffList.Add(staff);
            }
        }
        public async Task SaveChangesAsync()
        {
            Console.WriteLine("SaveChangesAsync");
            CollectionReference colRef = _firestoreDb.Collection("Staff");
            QuerySnapshot snapshot = await colRef.GetSnapshotAsync();

            List<string> idsInCloud = [];
            foreach (DocumentSnapshot docSnapshot in snapshot.Documents)
            {
                idsInCloud.Add(docSnapshot.Id);
            }
            List<string> idsInList = [];
            foreach (Staff staff in StaffList)
            {
                if (staff != null)
                    idsInList.Add("staff_" + staff.Id);
            }
            IEnumerable<string> idsToDelete = idsInCloud.Except(idsInList);

            foreach (string id in idsToDelete)
            {
                DocumentReference docRef = colRef.Document(id);
                await docRef.DeleteAsync();
            }

            foreach (Staff staff in StaffList)
            {
                if (staff.changed == true)
                {
                    await colRef.Document("staff_" + staff.Id).SetAsync(staff);
                    staff.changed = false;
                }
            }
        }

        public void Add(Staff staff)
        {
            staff.changed = true;
            StaffList.Add(staff);
        }
        public void Update(Staff staff)
        {
            foreach (var s in StaffList)
            {
                if (s.Id == staff.Id)
                {
                    s.Id = staff.Id;
                    s.Name = staff.Name;
                    s.Gender = staff.Gender;
                    s.HealthCareStaff = staff.HealthCareStaff;
                    s.DateOfBirth = staff.DateOfBirth;
                    s.Email = staff.Email;
                    s.PhoneNum = staff.PhoneNum;
                    s.Degree = staff.Degree;
                    s.Department = staff.Department;
                    s.Specialist = staff.Specialist;
                    s.WorkSchedule = staff.WorkSchedule;
               

                    s.changed = true;
                    break;
                }
            }
        }

        public void DateInWeek(Staff staff)
        {
            //add 7 day in week
            DateTime today = DateTime.Today;
            int currentDayOfWeek = (int)today.DayOfWeek;
            DateTime sunday = today.AddDays(-currentDayOfWeek);
            DateTime monday = sunday.AddDays(1);

            // If we started on Sunday, we should actually have gone *back*
            // 6 days instead of forward 1...
            if (currentDayOfWeek == 0)
            {
                monday = monday.AddDays(-7);
            }

            var dates = Enumerable.Range(0, 7).Select(days => monday.AddDays(days)).ToList();

            if (staff.WorkSchedule == null) staff.WorkSchedule = new Calendar();
            for(int i = 0; i < 7; i++) 
            {
                staff.WorkSchedule.Date.Add(dates[i].ToString("dd/mm/yyyy"));
            }
        }

        public void CreateCalendar()
        {
            Random random = new Random();
            int n = 7;

            foreach (Staff staff in StaffList)
            {
                if (staff.WorkSchedule == null)
                {
                    staff.WorkSchedule = new Calendar();
                    DateInWeek(staff);

                    for (int i = 0; i < n; i++)
                    {
                        //add shift
                        int randomNumber = random.Next(0, 2);
                        staff.WorkSchedule.DayofWeek[i] = (Shift)randomNumber;
                        int other = 1 - randomNumber;
                        var specialList = StaffList.Where(s => s.Specialist == staff.Specialist && s != staff).ToList();

                        Staff staff1 = specialList[0];
                        staff1.changed = true;
                        DateInWeek(staff1);
                        if (specialList.Count >= 2)
                        {
                            Staff staff2 = specialList[1];
                            DateInWeek(staff2);
                            staff2.changed = true;

                            //setting calendar for staff1 and staff2
                            if (randomNumber == 0)
                            {
                                staff1.WorkSchedule.DayofWeek[i] = (Shift)other;
                                staff2.WorkSchedule.DayofWeek[i] = (Shift)2;
                            }
                            else
                            {
                                staff1.WorkSchedule.DayofWeek[i] = (Shift)2;
                                staff2.WorkSchedule.DayofWeek[i] = (Shift)other;
                            }
                        }
                        else
                        {
                            staff1.WorkSchedule.DayofWeek[i] = (Shift)other;
                        }
                    }
                    staff.changed = true;
                }
                else continue;
            }
        }
    }
}
