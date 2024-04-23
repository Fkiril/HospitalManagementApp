using Microsoft.AspNetCore.Identity;
using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementApp.Models
{
    [FirestoreData]
    public class ApplicationUser : IdentityUser
    {
        [FirestoreProperty]
        [Required]
        public override string Id { get; set; }

        [FirestoreProperty]
        [PersonalData, Required]
        public override string UserName { get; set; }

        [FirestoreProperty]
        [DataType(DataType.EmailAddress)]
        [PersonalData, Required]
        public override string Email { get; set; }

        [FirestoreProperty]
        [DataType(DataType.Password)]
        [PersonalData, Required]
        public string Password { get; set; }

        [FirestoreProperty]
        [Required]
        public string Role { get; set; }

        [FirestoreProperty]
        [Required]
        public int DataId { get; set; }
    }
}
