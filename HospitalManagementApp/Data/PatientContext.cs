using Google.Cloud.Firestore;
using HospitalManagementApp.Models;
using HospitalManagementApp.Services;
using NuGet.Versioning;
using System.Globalization;

namespace HospitalManagementApp.Data
{
    public class PatientContext 
    {
        public readonly FirestoreDb _firestoreDb;
        public static ICollection<Patient> PatientList { get; private set; } = default!;
        public readonly StaffContext _staffContext;
        public PatientContext(
            FirestoreDbService firestoreDbService,
            ICollection<Patient>? patientList,
            StaffContext staffContext)
        {
            _firestoreDb = firestoreDbService.GetFirestoreDb();
            PatientList = patientList ?? ([]);
            _staffContext = staffContext;
        }

        private const string colName = "Patient";
        private const string docId = "patient_";

        public async Task InitializePatientListFromFirestore()
        {
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

        public bool IsIdUnique(int? id)
        {
            foreach (var patient in PatientList)
            {
                if (id != null && patient.Id == id)
                {
                    return false;
                }
            }
            return true;
        }
        public void Add(Patient patient)
        {
            if (IsIdUnique(patient.Id))
            {
                patient.Changed = true;
                PatientList.Add(patient);
            }
            else
            {
                throw new Exception("Id is not unique!");
            }
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
                    if (patient.Status is not null) p.Status = patient.Status;

                    p.Changed = true;
                    break;
                }
            }
        }

        public void AddTreatmentSchedule(Patient patient, TreatmentScheduleEle newTreatment)
        {
            ArgumentNullException.ThrowIfNull(patient, nameof(patient));
            ArgumentNullException.ThrowIfNull(newTreatment, nameof(newTreatment));

            patient.TreatmentSchedule ??= [];

            foreach (var treatment in patient.TreatmentSchedule)
            {
                if (treatment.Id == newTreatment.Id)
                {
                    throw new ArgumentException("Id is not unique!");
                }
            }

            patient.TreatmentSchedule.Add(newTreatment);
            
            patient.Changed = true;
        }

        public void UpdateTreatmentSchedule(Patient patient, int id,  TreatmentScheduleEle treatment)
        {
            ArgumentNullException.ThrowIfNull(patient, nameof(patient));
            ArgumentNullException.ThrowIfNull(treatment, nameof(treatment));

            if (patient.TreatmentSchedule == null)
            {
                throw new Exception("TreatmentScheduleEle Schedule list of this patient is empty!");
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
                throw new Exception("TreatmentScheduleEle Schedule list of this patient is empty!");
            }
            if (patient.TreatmentSchedule.First(x => x.Id == id) == null)
            {
                throw new Exception("There are not any schedule that have the same id!");
            }

            var treatment = patient.TreatmentSchedule.First(x => x.Id == id);
            patient.TreatmentSchedule.Remove(treatment);

            patient.Changed = true;
        }

        public void SetTestResult(int patientId, TestResult? newTestResult)
        {
            Patient patient = PatientList.First(x => x.Id == patientId) ?? throw new Exception("Patient is not found!");
            patient.TestResult = newTestResult;
            patient.Changed = true;
        }

        public void SetStaffId(int patientId, List<int> staffIds)
        {
            Patient patient = PatientList.First(x => x.Id == patientId) ?? throw new Exception("Patient is not found!");
            foreach (var id in staffIds)
            {
                // if (_staffContext.???) 
                // {
                //      throw new Exception();
                // }
            }
            patient.StaffId = staffIds;
            patient.Changed = true;
        }

        public void AddMedicalHistory(int patientId, MedicalHistoryEle medicalHistoryEle)
        {
            Patient patient = PatientList.First(x => x.Id == patientId) ?? throw new Exception("Patient is not found!");
            if (patient.MedicalHistory != null)
            {
                patient.MedicalHistory.Add(medicalHistoryEle);
            }
            else
            {
                patient.MedicalHistory = [medicalHistoryEle];
            }
            patient.Changed = true;
        }

        // a function to add new treatment schedule for patient
        // It's depended on work schedules of the dotor that take care of the patient
        public void AddTreatmentSchedule(int patientId, TreatmentScheduleEle scheduel)
        {
            Patient patient = PatientList.First(x => x.Id == patientId) ?? throw new Exception("Patient is not found!");
                
            
            if (patient.TreatmentSchedule != null)
            {
                patient.TreatmentSchedule.Add(scheduel);
            }
            else
            {
                patient.TreatmentSchedule = [scheduel];
            }
            patient.Changed = true;
        }

        public bool IsTreatmentScheduleFix(int patientId, int scheduleId)
        {
            Patient patient = PatientList.First(x => x.Id == patientId) ?? throw new Exception("Patient is not found!");
            if (patient.StaffId is null) throw new Exception("Patient is not depending on any staffs!");

            if (patient.TreatmentSchedule is null) throw new Exception("Patient does not have any treatment schedules!");

            TreatmentScheduleEle schedule = patient.TreatmentSchedule.First(x => x.Id == scheduleId);
            if (schedule != null) throw new Exception("Can not define any treatment schedule!");

            Models.Calendar docSchedule = _staffContext.GetCalendar(patient.StaffId);
            
            if (docSchedule != null) return false;
            else
            {
                var pDate = DateTime.ParseExact(schedule.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var dStartDate = DateTime.ParseExact(docSchedule.Date[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var dEndDate = DateTime.ParseExact(docSchedule.Date[7], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                
                if (pDate <  dStartDate || pDate > dEndDate) return false;
                
                for (int idx = 0; idx < 7; idx++)
                {
                    if (docSchedule.Date[idx] == schedule.Date)
                    {
                        switch (docSchedule.DayofWeek[idx])
                        {
                            case Shift.Morning: //07:00 -> 14:00
                                {
                                    break;
                                }

                            case Shift.Afternoon: //14:00 -> 22:00
                                {
                                    break;
                                }
                            case Shift.Evening: //22:00 -> 07:00
                                {
                                    break;
                                }
                        }
                    }
                }
            }

            return true;
        }
    }
}
