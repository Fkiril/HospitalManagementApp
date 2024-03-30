using Google.Api;
using Google.Api.Gax;
using Google.Cloud.Firestore;
using HospitalManagementApp.Models;
using HospitalManagementApp.Services;
using Microsoft.AspNetCore.Mvc;

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
        }



        public void Remove(Drugs drug)
        {
            DrugsList.Remove(drug);
        }

        public void Add(Drugs drugs)
        {

            DrugsList.Add(drugs);
        }
        public void Update(Drugs drugs)
        {
            foreach (var p in DrugsList)
            {
                if (p.Id == drugs.Id)
                {
                    p.Id = drugs.Id;
                    p.Name = drugs.Name;
                    p.HisUse = drugs.HisUse;
                    p.Status = drugs.Status;
                    p.Expiry = drugs.Expiry;

                    break;
                }
            }
        }
    }
}