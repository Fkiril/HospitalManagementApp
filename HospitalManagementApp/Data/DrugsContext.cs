using Microsoft.AspNetCore;
using Google.Cloud.Firestore;
using HospitalManagementApp.Models;
using HospitalManagementApp.Services;

namespace HospitalManagementApp.Data
{
    public class DrugsContext
    {
        public readonly FirestoreDb _firestoreDb;
        public static ICollection<Drugs> DrugsList { get; private set; } = default!;
        public static List<Drugs> DrugsList_V2 { get; private set; } = default!;
        public DrugsContext(FirestoreDbService firestoreDbService)
        {
            _firestoreDb = firestoreDbService.GetFirestoreDb();
            DrugsList = new List<Drugs>();
            DrugsList_V2 = new List<Drugs>();
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
                if (drug.ExpirationDate < DateTime.Now)
                {
                    
                   drug.Enable = false;
                    await GetAll();
                    Update_V2(drug);
                    await SaveChangesAsync_V2();
                }
                if (drug.Enable == true)
                {
                    DrugsList.Add(drug);
                }

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
                drug.ManufactureDate = DateTime.SpecifyKind(drug.ManufactureDate, DateTimeKind.Utc);
                drug.ExpirationDate = DateTime.SpecifyKind(drug.ExpirationDate, DateTimeKind.Utc);
                if (string.IsNullOrEmpty(drug.docId))
                {
                    DocumentReference newDocRed = await colRef.AddAsync(drug);
                    drug.docId = newDocRed.Id;
                }

                await colRef.Document(drug.docId).SetAsync(drug);
            }
        }

        public async Task SaveChangesAsync_V2()
        {
            CollectionReference colRef = _firestoreDb.Collection("drugs");

            List<string> idsInCloud = new List<string>();
            List<string> idsInList = new List<string>();
            IEnumerable<string> idsToDelete = idsInCloud.Except(idsInList);

            foreach (string id in idsToDelete)
            {
                DocumentReference docRef = colRef.Document(id);
                await docRef.DeleteAsync();
            }


            foreach (Drugs drug in DrugsList_V2)
            {
                drug.ReceiptDay = DateTime.SpecifyKind(drug.ReceiptDay, DateTimeKind.Utc);
                drug.ManufactureDate = DateTime.SpecifyKind(drug.ManufactureDate, DateTimeKind.Utc);
                drug.ExpirationDate = DateTime.SpecifyKind(drug.ExpirationDate, DateTimeKind.Utc);
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

                    Drugs drugsToRemove = DrugsList.FirstOrDefault(drugs => !string.IsNullOrEmpty(drugs.docId) ? drugs.docId.Equals(id) : true);
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
                    p.Name = drugs.Name;
                    if (p.Quantity == 0 && drugs.Quantity != 0)
                    {
                        p.ReceiptDay = DateTime.Now;
                        p.Status = Status.Available;
                    }
                    if(drugs.Quantity == 0)
                        p.Status = Status.NotAvailable;
                    p.Quantity = drugs.Quantity;
                    p.ManufactureDate = drugs.ManufactureDate;
                    p.ExpirationDate = drugs.ExpirationDate;
                    
                    break;
                }
            }
        }

        public async Task GetAll()
        {
            DrugsList_V2 = new List<Drugs>();
            Query drugsQuery = _firestoreDb.Collection("drugs");
            QuerySnapshot snapshotQuery = await drugsQuery.GetSnapshotAsync();

            foreach (DocumentSnapshot docSnapshot in snapshotQuery.Documents)
            {
                Drugs drug = docSnapshot.ConvertTo<Drugs>();
                drug.docId = docSnapshot.Id;
                DrugsList_V2.Add(drug);
            }
        }

        public async Task<Drugs> FindById(int id)
        {
            Drugs drug = new();
            await InitializeDrugsListFromFirestore();
            foreach (Drugs d in DrugsList)
            {
                if (d.IdOfDrug == id)
                    drug = d;
            }
            return drug;
        }

        public void Update_V2(Drugs drugs)
        {
            foreach (var p in DrugsList_V2)
            {
                if (p.IdOfDrug == drugs.IdOfDrug)
                {
                    p.Name = drugs.Name;
                    p.Status = drugs.Status;
                    p.Quantity = drugs.Quantity;
                    p.ManufactureDate = drugs.ManufactureDate;
                    p.ExpirationDate = drugs.ExpirationDate;
                    p.Enable = drugs.Enable;
                    break;
                }
            }
        }
    }
}