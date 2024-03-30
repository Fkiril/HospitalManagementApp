using Google.Api.Gax;
using Google.Cloud.Firestore;

namespace HospitalManagementApp.Services
{
    public class FirestoreDbService
    {
        private FirestoreDb _db;

        public FirestoreDbService(string projectId)
        {
            _db = new FirestoreDbBuilder
            {
                ProjectId = projectId,
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction
            }.Build();
        }

        public FirestoreDb GetFirestoreDb()
        {
            return _db;
        }
    }
}
