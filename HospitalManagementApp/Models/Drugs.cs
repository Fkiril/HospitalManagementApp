using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace HospitalManagementApp.Models
{
    public enum Status
    {
        Available,
        NotAvailable
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
        public int? Quantity { get; set; }
        [FirestoreProperty]
        [Display(Name = "Date of Manufacture")]
        [DataType(DataType.DateTime)]
        public DateTime ManufactureDate { get; set; }
        [FirestoreProperty]
        [Display(Name = "Expiration Date")]
        [DataType(DataType.DateTime)]
        public DateTime ExpirationDate { get; set; }

        [FirestoreProperty]
        [Display(Name = "Receipt Of Day")]
        [DataType(DataType.DateTime)]
        public DateTime ReceiptDay { get; set; }

        [FirestoreProperty]
        [Required, StringLength(60, MinimumLength = 3)]
        public string? Name { get; set; }
        [FirestoreProperty]
        public bool Enable { get; set; } = true;

        [FirestoreProperty]
        public Status? Status { get; set; }

    }
}