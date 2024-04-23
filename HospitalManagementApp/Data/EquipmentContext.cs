using Google.Api;
using Google.Api.Gax;
using Google.Cloud.Firestore;
using HospitalManagementApp.Models;
using HospitalManagementApp.Services;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using System.Globalization;

namespace HospitalManagementApp.Data
{
    public class EquipmentContext
    {
        public readonly FirestoreDb _firestoreDb;
        public static ICollection<Equipment> EquipmentList { get; private set; } = default!;
        public EquipmentContext(FirestoreDbService firestoreDbService, ICollection<Equipment>? equipmentList = null)
        {
            _firestoreDb = firestoreDbService.GetFirestoreDb();
            EquipmentList = (equipmentList != null) ? equipmentList : [];
        }
        public async Task InitializeEquipmentListFromFirestore()
        {
            UpdateSchedule();
            UpdateCount("InitFirestore");

            Console.WriteLine("InitializeEquipmentListFromFirestore");
            if (EquipmentList.Count != 0)
            {
                return;
            }
            QuerySnapshot snapshotQuery = await _firestoreDb.Collection("Equipment").GetSnapshotAsync();

            Console.WriteLine("Convert from string to DateTime");
            foreach (DocumentSnapshot docSnapshot in snapshotQuery.Documents)
            {
                Equipment equipment = docSnapshot.ConvertTo<Equipment>();
                equipment.History = new List<DateTime>();

                if (equipment.historyString != null)
                {
                    Console.WriteLine("Id " + equipment.Id);
                    foreach (var s in equipment.historyString)
                    {
                        Console.WriteLine(s);
                    }
                    // Convert List<string> to List<DateTime>
                    equipment.History = equipment.historyString.Select(dtString => DateTime.ParseExact(dtString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)).ToList();
                }

                equipment.changed = false;
                EquipmentList.Add(equipment);
            }

            UpdateSchedule();
            UpdateCount("InitFirestore");
        }
        public async Task SaveChangesAsync()
        {
            Console.WriteLine("SaveChangesAsync");
            CollectionReference colRef = _firestoreDb.Collection("Equipment");
            QuerySnapshot snapshot = await colRef.GetSnapshotAsync();

            List<string> idsInCloud = [];
            foreach (DocumentSnapshot docSnapshot in snapshot.Documents)
            {
                idsInCloud.Add(docSnapshot.Id);
            }
            List<string> idsInList = [];
            foreach (Equipment equipment in EquipmentList)
            {
                if (equipment != null)
                    idsInList.Add("equipment_" + equipment.Id);
            }
            IEnumerable<string> idsToDelete = idsInCloud.Except(idsInList);

            foreach (string id in idsToDelete)
            {
                DocumentReference docRef = colRef.Document(id);
                await docRef.DeleteAsync();
            }

            foreach (Equipment equipment in EquipmentList)
            {
                if (equipment.changed == true)
                {
                    await colRef.Document("equipment_" + equipment.Id).SetAsync(equipment);
                    equipment.changed = false;
                }
            }
        }
        public bool IsIdUnique(int? id)
        {
            foreach (var equipment in EquipmentList)
            {
                if (id != null && equipment.Id == id)
                {
                    return false;
                }
            }
            return true;
        }
        public void Add(Equipment equipment)
        {
            if (IsIdUnique(equipment.Id))
            {
                equipment.changed = true;
                EquipmentList.Add(equipment);
            }
            else
            {
                throw new Exception("This equipment id is not unique!");
            }
        }
        public void Update(Equipment equipment)
        {
            foreach (var p in EquipmentList)
            {
                if (p.Id == equipment.Id)
                {
                    p.Id = equipment.Id;
                    p.Name = equipment.Name;
                    p.Description = equipment.Description;
                    p.IsAvailable = equipment.IsAvailable;
                    p.UsingUntil = equipment.UsingUntil;
                    p.History = equipment.History;

                    p.historyString = equipment.historyString;
                    p.changed = true;
                    break;
                }
            }
        }

        public void Remove(Equipment equipment)
        {
            EquipmentList.Remove(equipment);
        }

        public static bool IsBetweenDate(DateTime date, DateTime target)
        {
            DateTime startTime = target.AddDays(-7);
            DateTime finishTime = target.AddDays(7);

            if (date >= startTime && date < finishTime)
            {
                return true;
            }

            return false;
        }

        public void AddSchedule(int id, DateTime schedule)
        {
            var equipment = EquipmentList.FirstOrDefault(e => e.Id == id);
            if (equipment == null)
            {
                throw new Exception("Add Schedule bug");
            }

            equipment.History ??= new List<DateTime>();
            equipment.History.Add(schedule);
        }

        private void UpdateCount(string taskName = "empty")
        {
            // Update TotalCount
            var equipmentByType = EquipmentList.GroupBy(equipment => equipment.Name);

            foreach (var group in equipmentByType)
            {
                int count = group.Count();
                foreach (var equipment in group)
                {
                    equipment.TotalCount = count;
                }
            }

            // Update AvailableCount
            var availableByType = EquipmentList.Where(equipment => equipment.IsAvailable == true)
                                               .GroupBy(equipment => equipment.Name);

            foreach (var group in availableByType)
            {
                int count = group.Count(); // Get the quantity of available equipment for this type
                foreach (var equipment in EquipmentList)
                {
                    if (equipment.Name == group.First().Name)
                    {
                        equipment.AvailableCount = count;
                    }
                }
            }
        }

        private void UpdateSchedule() {
            Console.WriteLine("Convert History to string");
            foreach (var equipment in EquipmentList)
            {
                equipment.History ??= new List<DateTime>();

                equipment.IsAvailable = true;
                foreach (DateTime startTime in equipment.History)
                {
                    bool currentAvailable = !IsBetweenDate(DateTime.Now, startTime);
                    if (currentAvailable == false)
                    {
                        equipment.IsAvailable = false;
                        break;
                    }
                }

                if (equipment.History != null)
                {
                    List<string> historyStringList = equipment.History.Select(dt => dt.ToString("yyyy-MM-dd HH:mm:ss")).ToList();
                    equipment.historyString = historyStringList;
                }
            }
        }
    }
}
