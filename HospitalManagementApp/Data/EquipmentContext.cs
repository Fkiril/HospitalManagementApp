using Google.Api;
using Google.Api.Gax;
using Google.Cloud.Firestore;
using HospitalManagementApp.Models;
using HospitalManagementApp.Services;
using Microsoft.EntityFrameworkCore;

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
            Console.WriteLine("InitializeEquipmentListFromFirestore");
            if (EquipmentList.Count != 0)
            {
                return;
            }
            QuerySnapshot snapshotQuery = await _firestoreDb.Collection("Equipment").GetSnapshotAsync();

            foreach (DocumentSnapshot docSnapshot in snapshotQuery.Documents)
            {
                Equipment equipment = docSnapshot.ConvertTo<Equipment>();
                equipment.changed = false;
                EquipmentList.Add(equipment);
            }
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

        public void Add(Equipment equipment)
        {
            equipment.changed = true;
            EquipmentList.Add(equipment);
        }
        public void Update(Equipment equipment)
        {
            foreach (var p in EquipmentList)
            {
                if (p.Id == equipment.Id)
                {
                    p.Id = equipment.Id;
                    p.docId = equipment.docId;
                    p.Name = equipment.Name;
                    p.Description = equipment.Description;
                    p.IsAvailable = equipment.IsAvailable;
                    p.UsingUntil = equipment.UsingUntil;
                    p.History = equipment.History;

                    p.changed = true;
                    break;
                }
            }
        }
    }
}
