using Microsoft.AspNetCore.Identity;
using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementApp.Models
{
    [FirestoreData]
    public class ApplicationUser : IdentityUser
    {
        [FirestoreProperty]
        public override string? Id { get; set; }

        [FirestoreProperty]
        [PersonalData]
        public override string? UserName { get; set; }

        [FirestoreProperty]
        [DataType(DataType.EmailAddress)]
        [PersonalData]
        public override string? Email { get; set; }

        [FirestoreProperty]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [FirestoreProperty]
        public string? Role { get; set; }
    }
}
