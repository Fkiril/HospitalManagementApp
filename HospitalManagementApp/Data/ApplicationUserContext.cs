using Google.Cloud.Firestore;
using HospitalManagementApp.Models;
using HospitalManagementApp.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

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
                await GetDocumentReferenceWithId(user.Id).SetAsync(user);
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
                await GetDocumentReferenceWithId(user.Id).DeleteAsync();
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
                await GetDocumentReferenceWithId(user.Id).SetAsync(user);
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
            var docQuery = query.Documents.FirstOrDefault(doc => doc.GetValue<string>("Email") == email);

            ApplicationUser user = new();

            if (docQuery != null)
            {
                user = docQuery.ConvertTo<ApplicationUser>();
            }

            if (user.UserName == null || user.Email == null || user.Role == null || user.DataId == null)
            {
                throw new InvalidDataException("Can not detect user by this email!");
            }

            return user;
        }

        public async Task ChancePassWordAsync(string id, string newPassword)
        {
            var docQuery = await GetDocumentReferenceWithId(id).GetSnapshotAsync();

            if (docQuery != null)
            {
                var user = docQuery.ConvertTo<ApplicationUser>();
                
                user.Password = newPassword;

                await GetDocumentReferenceWithId(id).SetAsync(user);
            }
            else
            {
                throw new InvalidDataException("Can not find suitable Application Account!");
            }
        }

        public async Task<bool> UserRegisted(int id, bool patientFlag)
        {
            CollectionReference colRef = GetCollectionReference();
            QuerySnapshot query = await colRef.GetSnapshotAsync();

            if (patientFlag)
            {
                var docQuery = query.Documents.FirstOrDefault(doc => doc.GetValue<string>("Role") == "Patient" &&
                                                                     doc.GetValue<int>("DataId") == id);
                if (docQuery != null)
                {
                    return true;
                }
            }
            else
            {
                var docQuery = query.Documents.FirstOrDefault(doc => (
                                (doc.GetValue<string>("Role") != "Patient"
                                 && doc.GetValue<int>("DataId") == id)));
                if (docQuery == null)
                {
                    return true;
                }
            };

            return false;
        }
    }
}
