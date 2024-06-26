﻿using Google.Cloud.Firestore;
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
        public PatientContext(
            FirestoreDbService firestoreDbService,
            ICollection<Patient>? patientList)
        {
            _firestoreDb = firestoreDbService.GetFirestoreDb();
            PatientList = patientList ?? [];
        }

        private const string colName = "Patient";
        private const string docId = "patient_";

        public async Task InitializePatientListFromFirestore()
        {
            if (PatientList.Count != 0)
            {
                return;
            }
            else
            {
                Console.WriteLine("InitializePatientListFromFirestore");
                QuerySnapshot snapshotQuery = await _firestoreDb.Collection(colName).GetSnapshotAsync();

                foreach (DocumentSnapshot docSnapshot in snapshotQuery.Documents)
                {
                    Patient patient = docSnapshot.ConvertTo<Patient>();
                    patient.Changed = false;
                    PatientList.Add(patient);
                }
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
                    if (patient.Email != null) p.Email = patient.Email;
                    if (patient.MedicalHistory != null) p.MedicalHistory = patient.MedicalHistory;
                    if (patient.TestResult != null) p.TestResult = patient.TestResult;
                    if (patient.TreatmentSchedule != null) p.TreatmentSchedule = patient.TreatmentSchedule;
                    if (patient.Status is not null) p.Status = patient.Status;

                    p.Changed = true;

                    break;
                }
            }
        }


        public void AddTreatmentSchedule(Patient patient,
            TreatmentScheduleEle newTreatment,
            Models.Calendar? docSchedule)
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

            if (IsTreatmentScheduleFix(patient, newTreatment, docSchedule))
            {
                patient.TreatmentSchedule.Add(newTreatment);

                patient.Changed = true;
            }
            else
            {
                throw new InvalidDataException("New treatment schedule does not fix!");
            }
        }

        public void UpdateTreatmentSchedule(Patient patient,
            int id,
            TreatmentScheduleEle treatment,
            Models.Calendar? docSchedule)
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

            if (IsTreatmentScheduleFix(patient, treatment, docSchedule))
            {
                var curTreatment = patient.TreatmentSchedule.First(x => x.Id == id);
                curTreatment.Date = treatment.Date;
                curTreatment.StartTime = treatment.StartTime;
                curTreatment.EndTime = treatment.EndTime;

                patient.Changed = true;
            }
            else
            {
                throw new InvalidDataException("New treatment schedule does not fix!");
            }
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

        public bool IsTreatmentScheduleFix(Patient patient,
            TreatmentScheduleEle pSchedule,
            Models.Calendar? docSchedule)
        {
            if (pSchedule == null || 
                pSchedule.Date == null || 
                pSchedule.StartTime == null || 
                pSchedule.EndTime == null)
                throw new ArgumentNullException(nameof(pSchedule));

            if (patient.StaffIds == null || docSchedule == null) return true;
            
            TimeSpan pStartTime = TimeSpan.Parse(pSchedule.StartTime);
            TimeSpan pEndTime = TimeSpan.Parse(pSchedule.EndTime);
            if (pStartTime >  pEndTime) return false;

            if (docSchedule == null || 
                docSchedule.DayofWeek == null || 
                docSchedule.Date == null) 
                return true;

            else
            {
                var pDate = DateTime.ParseExact(pSchedule.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var dStartDate = DateTime.ParseExact(docSchedule.Date[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var dEndDate = DateTime.ParseExact(docSchedule.Date[6], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                if (pDate < dStartDate || pDate > dEndDate) return false;
                
                for (int idx = 0; idx < 7; idx++)
                {
                    if (docSchedule.Date[idx] == pSchedule.Date)
                    {
                        TimeSpan dStartTime = pStartTime, dEndTime = pEndTime;
                        if (docSchedule.DayofWeek.Count == 0) 
                        {
                            switch (docSchedule.Date[idx + 7])
                            {
                                case "0":
                                    {
                                        dStartTime = TimeSpan.Parse("06:00");
                                        dEndTime = TimeSpan.Parse("11:30");
                                        break;
                                    }

                                case "1":
                                    {
                                        dStartTime = TimeSpan.Parse("12:30");
                                        dEndTime = TimeSpan.Parse("16:30");
                                        break;
                                    }
                                case "2":
                                    {
                                        dStartTime = TimeSpan.Parse("16:30");
                                        dEndTime = TimeSpan.Parse("19:00");
                                        break;
                                    }
                            }
                            if (pStartTime < dStartTime || pEndTime > dEndTime) return false;

                        }
                        else
                        {
                            switch (docSchedule.DayofWeek[idx])
                            {
                                case Shift.Morning:
                                    {
                                        dStartTime = TimeSpan.Parse("06:00");
                                        dEndTime = TimeSpan.Parse("11:30");
                                        break;
                                    }

                                case Shift.Afternoon:
                                    {
                                        dStartTime = TimeSpan.Parse("12:30");
                                        dEndTime = TimeSpan.Parse("16:30");
                                        break;
                                    }
                                case Shift.Evening:
                                    {
                                        dStartTime = TimeSpan.Parse("16:30");
                                        dEndTime = TimeSpan.Parse("19:00");
                                        break;
                                    }
                            }
                            Console.WriteLine(dStartTime.ToString() + " " + dEndTime.ToString());
                            if (pStartTime < dStartTime || pEndTime > dEndTime) return false;

                        }
                    }
                }
            }

            return true;
        }

        public void SetTestResult(Patient patient, TestResult? newTestResult)
        {
            ArgumentNullException.ThrowIfNull(patient, nameof(patient));

            if (patient.TestResult != null)
            {
                MedicalHistoryEle newHistory = new MedicalHistoryEle
                {
                    Disease = patient.TestResult.Disease,
                    StartDate = patient.TestResult.StartDate,
                    EndDate = DateTime.Now.ToString("dd/MM/yyyy")
                };
                if (patient.MedicalHistory != null)
                {
                    patient.MedicalHistory.Add(newHistory);
                }
                else
                {
                    patient.MedicalHistory = new List<MedicalHistoryEle> { newHistory };
                }
            }
            else
            {
                patient.TestResult = newTestResult;
                patient.Status = PatientStatus.Ill;
            }

            patient.Changed = true;
        }

        public void SetStaffId(Patient patient, List<int> staffIds)
        {
            ArgumentNullException.ThrowIfNull(patient, nameof (patient));
            
            patient.StaffIds = staffIds;

            patient.Changed = true;
        }

        public void AddMedicalHistory(Patient patient, MedicalHistoryEle history)
        {
            ArgumentNullException.ThrowIfNull(patient, nameof(patient));
            ArgumentNullException.ThrowIfNull(history, nameof(history));

            if (patient.MedicalHistory != null)
            {
                patient.MedicalHistory.Add(history);
            }
            else
            {
                patient.MedicalHistory = [history];
            }
            patient.Changed = true;
        }

        public void DeleteMedicalHistory(Patient patient, string startDate)
        {
            ArgumentNullException.ThrowIfNull(patient, nameof (patient));

            if (patient.MedicalHistory != null)
            {
                var history = patient.MedicalHistory.FirstOrDefault(x => x.StartDate == startDate);

                if (history != null)
                {
                    patient.MedicalHistory.Remove(history);

                    patient.Changed = true;
                }
            }
        }


        public List<int> GetPatientIdListFromStaffId(int staffId)
        {
            List<int> ids = new List<int>();

            foreach (var patient in PatientList)
            {
                if (patient != null && patient.Id != null && patient.StaffIds != null && patient.StaffIds.Contains(staffId))
                {
                    ids.Add((int) patient.Id);
                }
            }

            return ids;
        }

        public ICollection<Patient> GetPatientListFromIdList(List<int>? ids)
        {
            ICollection<Patient> patientList = [];

            if (ids == null) return patientList;

            foreach (var id in ids)
            {
                var patient = PatientList.FirstOrDefault(x => x.Id == id);
                if (patient != null)
                {
                    patientList.Add(patient);
                }
            }

            return patientList;
        }

        public Patient? GetPatientById(int id)
        {
            var patient = PatientList.FirstOrDefault(x => x.Id == id);

            return patient;

        }
    }
}
