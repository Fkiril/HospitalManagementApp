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
        public void CreateCalendar()
        {
            Random random = new Random();

            string[] week = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
            string[] shift = ["morning", "afternoon", "evening"];
            //department have two doctor
            for(var i = 0; i < 7; i++)
            {
                foreach ( Staff staff in StaffList)
                {
                    if(staff.WorkSchedule == String.Empty)
                    {
                        staff.WorkSchedule = "";
                        int randomNumber = random.Next(0, 2);
                        //setting shift

                        staff.WorkSchedule += week[i] + ' ' + shift[randomNumber];
                        int other = 1 - randomNumber;
                        var specialList = StaffList.Where(s => s.Specialist == staff.Specialist && s != staff).ToList();

                        Staff staff1 = specialList[0];
                        staff1.WorkSchedule = "";
                        if(specialList.Count >= 2)
                        {
                            Staff staff2 = specialList[1];
                            staff2.WorkSchedule = "";

                            //setting calendar for staff1 and staff2
                            if(randomNumber == 0)
                            {
                                staff1.WorkSchedule += week[i] + ' ' + shift[other];
                                staff2.WorkSchedule += week[i] + ' ' + shift[2];
                            }
                            else
                            {
                                staff1.WorkSchedule += week[i] + ' ' + shift[2];
                                staff2.WorkSchedule += week[i] + ' ' + shift[other];
                            }
                        }
                        else
                        {
                            staff1.WorkSchedule += week[i] + ' ' + shift[other];
                        }
                        
                    }
                    else  continue;
                }
            }

        }
    }
}