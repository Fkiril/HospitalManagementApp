using Google.Cloud.Firestore;
using HospitalManagementApp.Models;
using HospitalManagementApp.Services;

namespace HospitalManagementApp.Data
{
    public class DrugsContext
    {
        public readonly FirestoreDb _firestoreDb;
        public static ICollection<Drugs> DrugsList { get; private set; } = default!;
        public DrugsContext(FirestoreDbService firestoreDbService)
        {
            _firestoreDb = firestoreDbService.GetFirestoreDb();
            DrugsList = new List<Drugs>();
        }

        public async Task InitializeDrugsListFromFirestore()
        {
            if (DrugsList.Count != 0)
            {
                return;
            }
            Console.WriteLine("Initial DrugsList!");
            Query drugsQuery = _firestoreDb.Collection("drugs");
            QuerySnapshot snapshotQuery = await drugsQuery.GetSnapshotAsync();


            foreach (DocumentSnapshot docSnapshot in snapshotQuery.Documents)
            {
                Drugs drug = docSnapshot.ConvertTo<Drugs>();
                drug.docId = docSnapshot.Id;

                DrugsList.Add(drug);
            }
        }
        public async Task SaveChangesAsync()
        {
            CollectionReference colRef = _firestoreDb.Collection("drugs");
            QuerySnapshot snapshot = await colRef.GetSnapshotAsync();

            List<string> idsInCloud = new List<string>();
            List<string> idsInList = new List<string>();
            IEnumerable<string> idsToDelete = idsInCloud.Except(idsInList);

            foreach (string id in idsToDelete)
            {
                DocumentReference docRef = colRef.Document(id);
                await docRef.DeleteAsync();
            }

            foreach (Drugs drug in DrugsList)
            {
                drug.ReceiptDay = DateTime.SpecifyKind(drug.ReceiptDay, DateTimeKind.Utc);
                if (string.IsNullOrEmpty(drug.docId))
                {
                    DocumentReference newDocRed = await colRef.AddAsync(drug);
                    drug.docId = newDocRed.Id;
                }

                await colRef.Document(drug.docId).SetAsync(drug);
            }
        }


        public async Task DeleteAsync(string id)
        {
            try
            {
                DocumentReference docRef = _firestoreDb.Collection("drugs").Document(id);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    await docRef.DeleteAsync();

                    Drugs drugsToRemove = DrugsList.FirstOrDefault(drugs => drugs.docId == id);
                    if (drugsToRemove != null)
                    {
                        DrugsList.Remove(drugsToRemove);
                    }
                }
                else
                {
                    Console.WriteLine($"Document with ID {id} does not exist in Firestore.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting document: {ex.Message}");
            }
        }


        public void Add(Drugs drugs)
        {
            DrugsList.Add(drugs);
        }
        public void Update(Drugs drugs)
        {
            foreach (var p in DrugsList)
            {
                if (p.IdOfDrug == drugs.IdOfDrug)
                {
                    p.IdOfDrug = drugs.IdOfDrug;
                    p.Name = drugs.Name;
                    p.HisUse = drugs.HisUse;
                    p.Status = drugs.Status;
                    p.Expiry = drugs.Expiry;
                    p.Quantity = drugs.Quantity;
                    p.ReceiptDay = drugs.ReceiptDay;


                    break;
                }
            }
        }

        public async Task<List<Drugs>> GetAll()
        {
            List<Drugs> listDrug = new List<Drugs>();
            Query drugsQuery = _firestoreDb.Collection("drugs");
            QuerySnapshot snapshotQuery = await drugsQuery.GetSnapshotAsync();

            foreach (DocumentSnapshot docSnapshot in snapshotQuery.Documents)
            {
                Drugs drug = docSnapshot.ConvertTo<Drugs>();
                drug.docId = docSnapshot.Id;
                listDrug.Add(drug);
            }
            return listDrug;
        }

        public async Task<Drugs> FindById(int id)
        {
            Drugs drug = null;
            await InitializeDrugsListFromFirestore();
            foreach (Drugs d in DrugsList)
            {
                if (d.IdOfDrug == id)
                    drug = d;
            }
            return drug;
        }
    }
}


