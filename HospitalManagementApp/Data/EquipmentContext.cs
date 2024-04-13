using Google.Api;
using Google.Api.Gax;
using Google.Cloud.Firestore;
using HospitalManagementApp.Models;
using HospitalManagementApp.Services;

namespace HospitalManagementApp.Data
{
    public class EquipmentContext
    {
        public readonly FirestoreDb _firestoreDb;
        public static ICollection<Equipment> EquipmentList { get; private set; } = default!;
        public EquipmentContext(FirestoreDbService firestoreDbService)
        {
            _firestoreDb = firestoreDbService.GetFirestoreDb();
            EquipmentList = new List<Equipment>();
        }

        public async Task InitializeEquipmentListFromFirestore()
        {
            if (EquipmentList.Count != 0)
            {
                return;
            }
            Console.WriteLine("Initial EquipmentList!");
            Query equipmentsQuery = _firestoreDb.Collection("Equipment");
            QuerySnapshot snapshotQuery = await equipmentsQuery.GetSnapshotAsync();

            foreach (DocumentSnapshot docSnapshot in snapshotQuery.Documents)
            {
                Equipment equipment = docSnapshot.ConvertTo<Equipment>();
                equipment.docId = docSnapshot.Id;

                EquipmentList.Add(equipment);
            }
        }
        public async Task SaveChangesAsync()
        {
            CollectionReference colRef = _firestoreDb.Collection("Equipment");
            QuerySnapshot snapshot = await colRef.GetSnapshotAsync();

            List<string> idsInCloud = new List<string>();
            foreach (DocumentSnapshot docSnapshot in snapshot.Documents)
            {
                idsInCloud.Add(docSnapshot.Id);
            }
            List<string> idsInList = new List<string>();
            foreach (Equipment equipment in EquipmentList)
            {
                if (String.IsNullOrEmpty(equipment.docId))
                    idsInList.Add(equipment.docId);
            }
            IEnumerable<string> idsToDelete = idsInCloud.Except(idsInList);
            
            foreach (string id in idsToDelete)
            {
                DocumentReference docRef = colRef.Document(id);
                await docRef.DeleteAsync();
            }

            foreach (Equipment equipment in EquipmentList)
            {
                if (string.IsNullOrEmpty(equipment.docId))
                {
                    equipment.docId = "equipment_" + equipment.Id.ToString();
                    equipment.Edited = false;
                    await colRef.Document(equipment.docId).SetAsync(equipment);
                }

                if ((bool)(equipment.Edited = true)) 
                    await colRef.Document(equipment.docId).SetAsync(equipment);
            }
        }

        public void Add(Equipment equipment)
        {
            
            EquipmentList.Add(equipment);
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
                    p.Status = equipment.Status;
                    p.Schedule = equipment.Schedule;
                    p.Edited = true;
                    break;
                }
            }
        }
        public void Remove(Equipment equipment)
        {
            EquipmentList.Remove(equipment);
        }
    }
}
