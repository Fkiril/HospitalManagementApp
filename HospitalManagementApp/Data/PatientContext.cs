﻿using Google.Api;
using Google.Api.Gax;
using Google.Cloud.Firestore;
using HospitalManagementApp.Models;
using HospitalManagementApp.Services;

namespace HospitalManagementApp.Data
{
    public class PatientContext
    {
        public readonly FirestoreDb _firestoreDb;
        public static ICollection<Patient> PatientList { get; private set; } = default!;
        public PatientContext(FirestoreDbService firestoreDbService)
        {
            _firestoreDb = firestoreDbService.GetFirestoreDb();
            PatientList = new List<Patient>();
        }

        public async Task InitializePatientListFromFirestore()
        {
            if (PatientList.Count != 0)
            {
                return;
            }
            Console.WriteLine("Initial PatientList!");
            Query patientsQuery = _firestoreDb.Collection("Patient");
            QuerySnapshot snapshotQuery = await patientsQuery.GetSnapshotAsync();

            foreach (DocumentSnapshot docSnapshot in snapshotQuery.Documents)
            {
                Patient patient = docSnapshot.ConvertTo<Patient>();
                patient.docId = docSnapshot.Id;

                PatientList.Add(patient);
            }
        }
        public async Task SaveChangesAsync()
        {
            CollectionReference colRef = _firestoreDb.Collection("Patient");
            QuerySnapshot snapshot = await colRef.GetSnapshotAsync();

            List<string> idsInCloud = new List<string>();
            foreach (DocumentSnapshot docSnapshot in snapshot.Documents)
            {
                idsInCloud.Add(docSnapshot.Id);
            }
            List<string> idsInList = new List<string>();
            foreach (Patient patient in PatientList)
            {
                if (String.IsNullOrEmpty(patient.docId))
                    idsInList.Add(patient.docId);
            }
            IEnumerable<string> idsToDelete = idsInCloud.Except(idsInList);
            
            foreach (string id in idsToDelete)
            {
                DocumentReference docRef = colRef.Document(id);
                await docRef.DeleteAsync();
            }

            foreach (Patient patient in PatientList)
            {
                if (patient.DateOfBirth.Kind != DateTimeKind.Utc) patient.DateOfBirth = DateTime.SpecifyKind(patient.DateOfBirth, DateTimeKind.Utc);

                if (string.IsNullOrEmpty(patient.docId))
                {
                    patient.docId = "patient_" + patient.Id.ToString();
                    patient.Edited = false;
                    await colRef.Document(patient.docId).SetAsync(patient);
                }

                if ((bool)(patient.Edited = true)) 
                    await colRef.Document(patient.docId).SetAsync(patient);
            }
        }

        public void Add(Patient patient)
        {
            
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
                    p.Edited = true;
                    break;
                }
            }
        }
        public void Remove(Patient patient)
        {
            PatientList.Remove(patient);
        }
    }
}
