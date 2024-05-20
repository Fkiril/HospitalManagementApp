using Google.Cloud.Firestore;
using HospitalManagementApp.Models;
using HospitalManagementApp.Services;
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

            Query prescriptionsQuery = _firestoreDb.Collection("Prescription");
            QuerySnapshot snapshot = await prescriptionsQuery.GetSnapshotAsync();

            foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
            {
                Prescription prescription = documentSnapshot.ConvertTo<Prescription>();
                prescription.docId = documentSnapshot.Id;

                // Gán docId cho mỗi đối tượng DrugInfo trong danh sách
                if (prescription.Drug != null)
                {
                    foreach (var drugInfo in prescription.Drug)
                    {
                        drugInfo.docId = documentSnapshot.Id;
                    }
                }

                PrescriptionList.Add(prescription);
            }
        }



        public async Task SaveChangesAsync()
        {
            CollectionReference colRef = _firestoreDb.Collection("Prescription");
            QuerySnapshot snapshot = await colRef.GetSnapshotAsync();

            // Xóa các tài liệu không còn tồn tại trong PrescriptionList
            foreach (DocumentSnapshot docSnapshot in snapshot.Documents)
            {
                string id = docSnapshot.Id;
                if (!PrescriptionList.Any(prescription => prescription.docId == id))
                {
                    await colRef.Document(id).DeleteAsync();
                }
            }

            foreach (Prescription prescription in PrescriptionList)
            {
                prescription.PurchaseDate = DateTime.SpecifyKind(prescription.PurchaseDate, DateTimeKind.Utc);
                if (string.IsNullOrEmpty(prescription.docId))
                {
                    DocumentReference newDocRef = await colRef.AddAsync(prescription);
                    prescription.docId = newDocRef.Id;
                }
                else
                {
                    await colRef.Document(prescription.docId).SetAsync(prescription);
                }
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
        public Prescription FindById(int id)
        {
            foreach (var p in PrescriptionList)
            {
                if (p.Id == id)
                    return p;
            }
            return null;
        }
        public async Task AddDetail(DrugInfo model)
        {
            await InitializePrescriptionListFromFirestore();
            foreach (var p in PrescriptionList)
            {
                if (p.Id == model.PresId)
                {
                    bool checkExist = false;
                    if (p.Drug != null && p.Drug.Count > 0)
                    {
                        foreach (var d in p.Drug)
                        {
                            if (d.IdOfDrug == model.IdOfDrug)
                            {
                                d.Quantity += model.Quantity;
                                checkExist = true;
                            }
                        }
                    }
                    else
                        p.Drug = new List<DrugInfo>();

                    if (!checkExist)
                        p.Drug.Add(model);
                    break;
                }
            }
        }
    }
}