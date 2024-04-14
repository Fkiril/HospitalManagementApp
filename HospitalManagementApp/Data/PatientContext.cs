using Google.Cloud.Firestore;
using HospitalManagementApp.Models;
using HospitalManagementApp.Services;

namespace HospitalManagementApp.Data
{
    public class PatientContext 
    {
        public readonly FirestoreDb _firestoreDb;
        public static ICollection<Patient> PatientList { get; private set; } = default!;
        public PatientContext(FirestoreDbService firestoreDbService, ICollection<Patient>? patientList = null)
        {
            _firestoreDb = firestoreDbService.GetFirestoreDb();
            PatientList = (patientList != null)? patientList : [];
        }

        private string colName = "Patient";
        private string docId = "patient_";

        public async Task InitializePatientListFromFirestore()
        {
            Console.WriteLine("InitializePatientListFromFirestore");
            if (PatientList.Count != 0)
            {
                return;
            }
            QuerySnapshot snapshotQuery = await _firestoreDb.Collection(colName).GetSnapshotAsync();

            foreach (DocumentSnapshot docSnapshot in snapshotQuery.Documents)
            {
                Patient patient = docSnapshot.ConvertTo<Patient>();
                patient.Changed = false;
                PatientList.Add(patient);
            }
        }
        public async Task SaveChangesAsync()
        {
            Console.WriteLine("SaveChangesAsync");
            CollectionReference colRef = _firestoreDb.Collection(colName);
            QuerySnapshot snapshot = await colRef.GetSnapshotAsync();

            List<string> idsInCloud = [];
            foreach (DocumentSnapshot docSnapshot in snapshot.Documents)
            {
                idsInCloud.Add(docSnapshot.Id);
            }
            List<string> idsInList = [];
            foreach (Patient patient in PatientList)
            {
                if (patient != null)
                    idsInList.Add(docId + patient.Id);
            }
            IEnumerable<string> idsToDelete = idsInCloud.Except(idsInList);
            
            foreach (string id in idsToDelete)
            {
                DocumentReference docRef = colRef.Document(id);
                await docRef.DeleteAsync();
            }

            foreach (Patient patient in PatientList)
            {
                if (patient.Changed == true)
                {
                    await colRef.Document(docId + patient.Id).SetAsync(patient);
                    patient.Changed = false;
                }
            }
        }

        public void Add(Patient patient)
        {
            patient.Changed = true;
            PatientList.Add(patient);
        }
        public void Update(Patient patient)
        {
            foreach (var p in PatientList)
            {
                if (p.Id == patient.Id)
                {
                    if (patient.Name != null) p.Name = patient.Name;
                    if (patient.Gender != null) p.Gender = patient.Gender;
                    if (patient.DateOfBirth != null) p.DateOfBirth = patient.DateOfBirth;
                    if (patient.Address != null) p.Address = patient.Address;
                    if (patient.PhoneNum != null) p.PhoneNum = patient.PhoneNum;
                    if (patient.MedicalHistory != null) p.MedicalHistory = patient.MedicalHistory;
                    if (patient.TestResult != null) p.TestResult = patient.TestResult;
                    if (patient.TreatmentSchedule != null) p.TreatmentSchedule = patient.TreatmentSchedule;
                    if (patient.Status != null) p.Status = patient.Status;

                    p.Changed = true;
                    break;
                }
            }
        }

        public void AddTreatmentSchedule(Patient patient, Treatment newTreatment)
        {
            ArgumentNullException.ThrowIfNull(patient, nameof(patient));

            patient.TreatmentSchedule ??= [];

            patient.TreatmentSchedule.Add(newTreatment);
            
            patient.Changed = true;
        }

        public void UpdateTreatmentSchedule(Patient patient, int id,  Treatment treatment)
        {
            ArgumentNullException.ThrowIfNull(patient, nameof(patient));
            ArgumentNullException.ThrowIfNull(treatment, nameof(treatment));

            if (patient.TreatmentSchedule == null)
            {
                throw new Exception("Treatment Schedule list of this patient is empty!");
            }

            if (patient.TreatmentSchedule.First(x => x.Id == id) == null)
            {
                throw new Exception("There are not any schedule that have the same id!");
            }

            var curTreatment = patient.TreatmentSchedule.First(x =>x.Id == id);
            curTreatment.Date = treatment.Date;
            curTreatment.StartTime = treatment.StartTime;
            curTreatment.EndTime = treatment.EndTime;

            patient.Changed = true;
        }

        public void DeleteTreatmentSchedule(Patient patient, int id)
        {
            ArgumentNullException.ThrowIfNull(patient, nameof(patient));

            if (patient.TreatmentSchedule == null)
            {
                throw new Exception("Treatment Schedule list of this patient is empty!");
            }
            if (patient.TreatmentSchedule.First(x => x.Id == id) == null)
            {
                throw new Exception("There are not any schedule that have the same id!");
            }

            var treatment = patient.TreatmentSchedule.First(x => x.Id == id);
            patient.TreatmentSchedule.Remove(treatment);

            patient.Changed = true;
        }

        public void SetTestResult(Patient patient, string? newTestResult)
        {
            ArgumentNullException.ThrowIfNull(patient, nameof(patient));

            if (PatientList.Contains(patient))
            {
                PatientList.First(p => p.Id == patient.Id).TestResult = newTestResult;
            }
        }
    }
}
