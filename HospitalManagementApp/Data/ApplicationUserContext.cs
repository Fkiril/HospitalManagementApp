using Google.Cloud.Firestore;
using HospitalManagementApp.Models;
using HospitalManagementApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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

        private string colName = "ApplicationUser";
        private string docId = "applicationuser_";

        public CollectionReference GetCollectionReference()
        {
            return _firestoreDb.Collection(colName);
        }

        public DocumentReference GetDocumentReferenceWithId(string id)
        {
            return _firestoreDb.Collection(colName).Document(docId + id);
        }

        public async Task AddUserAsync(ApplicationUser user)
        {
            try
            {
                await GetDocumentReferenceWithId(user.Id).SetAsync(user).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.Write("Can not add new data into FirestoreDb");
            }
        }

        public async Task DeleteUserAsync(ApplicationUser user)
        {
            try
            {
                await GetDocumentReferenceWithId(user.Id).DeleteAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
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
            catch (Exception ex)
            {
                Console.Write("Can not update data in FirestoreDb");
            }
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            CollectionReference colRef = GetCollectionReference();
            QuerySnapshot query = await colRef.GetSnapshotAsync();
            var docQuery = query.Documents.FirstOrDefault(doc => doc.GetValue<string>("Id") == id);

            ApplicationUser user = new ApplicationUser();

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

            ApplicationUser user = new ApplicationUser();

            if (docQuery != null)
            {
                user = docQuery.ConvertTo<ApplicationUser>();
            }

            return user;
        }
    }
}
