using Google.Api.Gax;
using Google.Cloud.Firestore;

namespace HospitalManagementApp.Services
{
    public class FirestoreDbService
    {
        private FirestoreDb firestoreDb;
        public FirestoreDbService(string projectId, Google.Apis.Auth.OAuth2.GoogleCredential credential)
        {
            firestoreDb = new FirestoreDbBuilder
            {
                ProjectId = projectId,
                Credential = credential,
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction
            }.Build();
        }

        public FirestoreDb GetFirestoreDb()
        {
            return firestoreDb;
        }


    }
}
