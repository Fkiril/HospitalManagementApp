using Google.Cloud.Firestore;
using HospitalManagementApp.Models;
using HospitalManagementApp.Services;

namespace HospitalManagementApp.Data
{
    public class ApplicationUserContext
    {
        public readonly FirestoreDb _firestoreDb;
        public ApplicationUserContext(FirestoreDbService firestoreDbService)
        {
            _firestoreDb = firestoreDbService.GetFirestoreDb();
        }

        private const string colName = "ApplicationUser";
        private const string docId = "applicationuser_";

        public CollectionReference GetCollectionReference()
        {
            return _firestoreDb.Collection(colName);
        }

        public DocumentReference GetDocumentReferenceWithId(string id)
        {
            return _firestoreDb.Collection(colName).Document(docId + id);
        }

        public async Task<bool> IsIdUnique(string id)
        {
            QuerySnapshot query = await _firestoreDb.Collection(colName).WhereEqualTo("Id", id).GetSnapshotAsync();
            return !(query.Count > 0);
        }
        public async Task AddUserAsync(ApplicationUser user)
        {
            if (await IsIdUnique(user.Id))
            {
                try
                {
                    await GetDocumentReferenceWithId(user.Id).SetAsync(user).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    Console.Write("Can not add new data into FirestoreDb");
                }
            }
            else
            {
                throw new Exception("Id is not unique!");
            }
        }

        public async Task DeleteUserAsync(ApplicationUser user)
        {
            try
            {
                await GetDocumentReferenceWithId(user.Id).DeleteAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                Console.Write("Can not delete data from FirestoreDb");
            }
        }

        public async Task UpdateUserAsync(ApplicationUser user)
        {
            try
            {
                await GetDocumentReferenceWithId(user.Id).SetAsync(user).ConfigureAwait(false);
            }
            catch (Exception)
            {
                Console.Write("Can not update data in FirestoreDb");
            }
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            CollectionReference colRef = GetCollectionReference();
            QuerySnapshot query = await colRef.GetSnapshotAsync();
            var docQuery = query.Documents.FirstOrDefault(doc => doc.GetValue<string>("Id") == id);

            ApplicationUser user = new();

            if (docQuery != null)
            {
                user = docQuery.ConvertTo<ApplicationUser>();
            }

            return user;
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            CollectionReference colRef = GetCollectionReference();
            QuerySnapshot query = await colRef.GetSnapshotAsync();
            var docQuery = query.Documents.FirstOrDefault(doc => doc.GetValue<string>("Email") ==  email);

            ApplicationUser user = new();

            if (docQuery != null)
            {
                user = docQuery.ConvertTo<ApplicationUser>();
            }

            return user;
        }
    }
}
