using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace HospitalManagementApp.Models
{

    [FirestoreData]
    public class Prescription
    {
        [FirestoreProperty]
        [Required, Range(1, 9999)]
        public int? IdOfPatient { get; set; }

        [FirestoreProperty]
        [Required, Range(1, 9999)]
        public int? Id { get; set; }

        [FirestoreProperty]
        public string? docId { get; set; } = string.Empty;

        [FirestoreProperty]
        [Required, StringLength(60, MinimumLength = 3)]
        public string? Drug { get; set; }

        [FirestoreProperty]
        [Required, StringLength(500, MinimumLength = 3)]
        public string? Description { get; set; }

    }
}
