using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementApp.Models
{
    [FirestoreData]
    public class User
    {
        [FirestoreProperty]
        public int Id { get; set; }

        [FirestoreProperty]
        [Required]
        public string? Email { get; set; }

        [FirestoreProperty]
        [Required]
        public string? Password { get; set; }
    }
}
