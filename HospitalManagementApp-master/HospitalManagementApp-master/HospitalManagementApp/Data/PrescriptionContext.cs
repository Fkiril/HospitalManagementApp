using Google.Api;
using Google.Api.Gax;
using Google.Cloud.Firestore;
using HospitalManagementApp.Models;
using HospitalManagementApp.Services;
using Microsoft.AspNetCore.Mvc;
namespace HospitalManagementApp.Data
{
    public class PrescriptionContext
    {
        public readonly FirestoreDb _firestoreDb;
        public static ICollection<Prescription> PrescriptionList { get; private set; } = default!;
        public PrescriptionContext(FirestoreDbService firestoreDbService)
        {
            _firestoreDb = firestoreDbService.GetFirestoreDb();
            PrescriptionList = new List<Prescription>();
        }

        public async Task InitializePrescriptionListFromFirestore()
        {
            if (PrescriptionList.Count != 0)
            {
                return;
            }
            Console.WriteLine("Initial PrescriptionList!");
            Query drugsQuery = _firestoreDb.Collection("Prescription");
            QuerySnapshot snapshotQuery = await drugsQuery.GetSnapshotAsync();


            foreach (DocumentSnapshot docSnapshot in snapshotQuery.Documents)
            {
                Prescription prescription = docSnapshot.ConvertTo<Prescription>();
                prescription.docId = docSnapshot.Id;

                PrescriptionList.Add(prescription);
            }
        }
        public async Task SaveChangesAsync()
        {
            CollectionReference colRef = _firestoreDb.Collection("Prescription");
            QuerySnapshot snapshot = await colRef.GetSnapshotAsync();

            List<string> idsInCloud = new List<string>();
            List<string> idsInList = new List<string>();
            IEnumerable<string> idsToDelete = idsInCloud.Except(idsInList);

            foreach (string id in idsToDelete)
            {
                DocumentReference docRef = colRef.Document(id);
                await docRef.DeleteAsync();
            }

            foreach (Prescription prescription in PrescriptionList)
            {

                if (string.IsNullOrEmpty(prescription.docId))
                {
                    DocumentReference newDocRed = await colRef.AddAsync(prescription);
                    prescription.docId = newDocRed.Id;
                }

                await colRef.Document(prescription.docId).SetAsync(prescription);
            }
        }


        public async Task DeleteAsync(string id)
        {
            try
            {
                DocumentReference docRef = _firestoreDb.Collection("Prescription").Document(id);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    await docRef.DeleteAsync();
                    Prescription prescriptionToRemove = PrescriptionList.FirstOrDefault(prescription => prescription.docId == id);
                    if (prescriptionToRemove != null)
                    {
                        PrescriptionList.Remove(prescriptionToRemove);
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

        public async Task<IEnumerable<Prescription>> GetPrescriptionsForPatientAsync(int Id)
        {
            CollectionReference prescriptionsCollection = _firestoreDb.Collection("Prescription");
            Query query = prescriptionsCollection.WhereEqualTo("IdOfPatient", Id);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            List<Prescription> prescriptions = new List<Prescription>();

            foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
            {
                Prescription prescription = documentSnapshot.ConvertTo<Prescription>();
                prescription.docId = documentSnapshot.Id;
                prescriptions.Add(prescription);
            }

            return prescriptions;
        }


        public void Add(Prescription prescription)
        {

            PrescriptionList.Add(prescription);
        }
        public void Update(Prescription prescription)
        {
            foreach (var p in PrescriptionList)
            {
                if (p.Id == prescription.Id)
                {
                    p.Id = prescription.Id;
                    p.IdOfPatient = prescription.IdOfPatient;
                    p.Drug = prescription.Drug;
                    p.Description = prescription.Description;

                    break;
                }
            }
        }
    }
}