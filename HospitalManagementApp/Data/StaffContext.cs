using Google.Api;
using Google.Api.Gax;
using Google.Cloud.Firestore;
using HospitalManagementApp.Models;
using HospitalManagementApp.Services;

namespace HospitalManagementApp.Data
{
    public class StaffContext
    {
        public readonly FirestoreDb _firestoreDb;
        public static ICollection<Staff> StaffList { get; private set; } = default!;
        public StaffContext(FirestoreDbService firestoreDbService)
        {
            _firestoreDb = firestoreDbService.GetFirestoreDb();
            StaffList = new List<Staff>();
        }

        public async Task InitializeStaffListFromFirestore()
        {
            if (StaffList.Count != 0)
            {
                return;
            }
            Console.WriteLine("Initial StaffList!");
            Query staffsQuery = _firestoreDb.Collection("Staff");
            QuerySnapshot snapshotQuery = await staffsQuery.GetSnapshotAsync();

            foreach (DocumentSnapshot docSnapshot in snapshotQuery.Documents)
            {
                Staff staff = docSnapshot.ConvertTo<Staff>();
                staff.patList = docSnapshot.Id;

                StaffList.Add(staff);
            }
        }
        public async Task SaveChangesAsync()
        {
            CollectionReference colRef = _firestoreDb.Collection("Staff");
            QuerySnapshot snapshot = await colRef.GetSnapshotAsync();

            List<string> idsInCloud = new List<string>();
            List<string> idsInList = new List<string>();
            IEnumerable<string> idsToDelete = idsInCloud.Except(idsInList);

            foreach (string id in idsToDelete)
            {
                DocumentReference docRef = colRef.Document(id);
                await docRef.DeleteAsync();
            }

            foreach (Staff staff in StaffList)
            {
                staff.DateOfBirth = DateTime.SpecifyKind(staff.DateOfBirth, DateTimeKind.Utc);

                if (string.IsNullOrEmpty(staff.patList))
                {
                    DocumentReference newDocRed = await colRef.AddAsync(staff);
                    staff.patList = newDocRed.Id;
                }

                await colRef.Document(staff.patList).SetAsync(staff);
            }
        }

        public void Add(Staff staff)
        {

            StaffList.Add(staff);
        }
        public void Update(Staff staff)
        {
            foreach (var d in StaffList)
            {
                if (d.Id == staff.Id)
                {
                    d.Id = staff.Id;
                    d.Name = staff.Name;
                    d.Gender = staff.Gender;
                    d.HealthCareStaff = staff.HealthCareStaff;
                    d.DateOfBirth = staff.DateOfBirth;
                    d.Email = staff.Email;
                    d.PhoneNum = staff.PhoneNum;
                    d.Degree = staff.Degree;
                    d.Specialist = staff.Specialist;
                    d.Department = staff.Department;
                    d.WorkSchedule = staff.WorkSchedule;
                    d.Status = staff.Status;

                    break;
                }
            }
        }
    }
}
