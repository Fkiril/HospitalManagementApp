using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace HospitalManagementApp.Models
{
    public enum Status
    {
        New,
        Used,
        Old
    }

    [FirestoreData]
    public class Drugs
    {
        [FirestoreProperty]
        [Required, Range(1, 9999)]
        public int? IdOfDrug { get; set; }

        [FirestoreProperty]
        public string? docId { get; set; } = string.Empty;

        [FirestoreProperty]
        [Required, Range(0, 9999)]
        public int? HisUse { get; set; }

        [FirestoreProperty]
        [Required, Range(0, 9999)]
        public int? Quantity { get; set; }

        [FirestoreProperty]
        [Display(Name = "Receipt Of Day")]
        [DataType(DataType.DateTime)]
        public DateTime ReceiptDay { get; set; }

        [FirestoreProperty]
        [Required, Range(1, 9999)]
        public int? Expiry { get; set; }

        [FirestoreProperty]
        [Required, StringLength(60, MinimumLength = 3)]
        public string? Name { get; set; }

        [FirestoreProperty]
        public Status? Status { get; set; }
    }
}