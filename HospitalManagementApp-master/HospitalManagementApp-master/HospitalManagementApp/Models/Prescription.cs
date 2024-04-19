using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

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
        public List<DrugInfo>? Drug { get; set; }

        [FirestoreProperty]
        [Required, StringLength(500, MinimumLength = 3)]
        public string? Description { get; set; }

    }

    [FirestoreData]
    public class DrugInfo
    {
        [FirestoreProperty]
        [Required, Range(1, 9999)]
        public int? IdOfDrug { get; set; }

        [FirestoreProperty]
        public int Quantity { get; set; }

        [FirestoreProperty]
        [Required, Range(1, 9999)]
        public int? PresId { get; set; }

        [FirestoreProperty]
        [Required, StringLength(500, MinimumLength = 1)]
        public string? NameOfDrug { get; set; }

        [FirestoreProperty]
        public string? docId { get; set; } = string.Empty;

    }




}
