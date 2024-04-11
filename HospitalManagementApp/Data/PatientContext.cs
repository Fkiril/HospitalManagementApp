using Google.Api;
using Google.Api.Gax;
using Google.Cloud.Firestore;
using HospitalManagementApp.Models;
using HospitalManagementApp.Services;
using Microsoft.EntityFrameworkCore;

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
        private string docId = "patient_";
        public async Task InitializePatientListFromFirestore()
        {
            Console.WriteLine("InitializePatientListFromFirestore");
            if (PatientList.Count != 0)
            {
                return;
            }
            QuerySnapshot snapshotQuery = await _firestoreDb.Collection("Patient").GetSnapshotAsync();

            foreach (DocumentSnapshot docSnapshot in snapshotQuery.Documents)
            {
                Patient patient = docSnapshot.ConvertTo<Patient>();
                patient.changed = false;
                PatientList.Add(patient);
            }
        }
        public async Task SaveChangesAsync()
        {
            Console.WriteLine("SaveChangesAsync");
            CollectionReference colRef = _firestoreDb.Collection("Patient");
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
                    idsInList.Add("patient_" + patient.Id);
            }
            IEnumerable<string> idsToDelete = idsInCloud.Except(idsInList);
            
            foreach (string id in idsToDelete)
            {
                DocumentReference docRef = colRef.Document(id);
                await docRef.DeleteAsync();
            }

            foreach (Patient patient in PatientList)
            {
                if (patient.changed == true)
                {
                    await colRef.Document("patient_" + patient.Id).SetAsync(patient);
                    patient.changed = false;
                }
            }
        }

        public void Add(Patient patient)
        {
            patient.changed = true;
            PatientList.Add(patient);
        }
        public void Update(Patient patient)
        {
            foreach (var p in PatientList)
            {
                if (p.Id == patient.Id)
                {
                    p.Id = patient.Id;
                    p.Name = patient.Name;
                    p.Gender = patient.Gender;
                    p.DateOfBirth = patient.DateOfBirth;
                    p.Address = patient.Address;
                    p.PhoneNum = patient.PhoneNum;
                    p.MedicalHistory = patient.MedicalHistory;
                    p.TestResult = patient.TestResult;
                    p.TreatmentSchedule = patient.TreatmentSchedule;
                    p.Status = patient.Status;

                    p.changed = true;
                    break;
                }
            }
        }

        public void AddTreatmentSchedule(Patient patient, Treatment newTreatment)
        {
            if (patient == null)
            {
                throw new ArgumentNullException("patient param can be null!");
            }

            if (patient.TreatmentSchedule == null)
            {
                patient.TreatmentSchedule = new List<Treatment>();
            }

            patient.TreatmentSchedule.Add(newTreatment);
            
            patient.changed = true;
        }

        public void UpdateTreatmentSchedule(Patient patient, int id,  Treatment treatment)
        {
            if (patient == null || treatment == null)
            {
                throw new ArgumentNullException("Parameters can be null!");
            }

            if (patient.TreatmentSchedule == null)
            {
                throw new Exception("Treatment Schedule list of this patient is empty!");
            }

            if (patient.TreatmentSchedule.First(x => x.Id == id) == null)
            {
                throw new ResourceMismatchException("There are not any schedule that have the same id!");
            }

            var curTreatment = patient.TreatmentSchedule.First(x =>x.Id == id);
            curTreatment.Date = treatment.Date;
            curTreatment.StartTime = treatment.StartTime;
            curTreatment.EndTime = treatment.EndTime;

            patient.changed = true;
        }

        public void DeleteTreatmentSchedule(Patient patient, int id)
        {
            if (patient == null)
            {
                throw new ArgumentNullException("patient parameter can be null!");
            }
            if (patient.TreatmentSchedule == null)
            {
                throw new Exception("Treatment Schedule list of this patient is empty!");
            }
            if (patient.TreatmentSchedule.First(x => x.Id == id) == null)
            {
                throw new ResourceMismatchException("There are not any schedule that have the same id!");
            }

            var treatment = patient.TreatmentSchedule.First(x => x.Id == id);
            patient.TreatmentSchedule.Remove(treatment);

            patient.changed = true;
        }
    }
}
