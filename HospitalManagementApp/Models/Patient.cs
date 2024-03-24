using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace HospitalManagementApp.Models
{
    public enum Gender
    {
        Male,
        Female,
        Other
    }

    [FirestoreData]    
    public class Patient
    {
        [FirestoreProperty]
        [Required, Range(1,9999)]
        public int? Id { get; set; }

        [FirestoreProperty]
        public string? docId { get; set; } = string.Empty;

        [FirestoreProperty]
        [Required, StringLength(60, MinimumLength = 3)]
        public string? Name { get; set; }

        [FirestoreProperty]
        [Required]
        public Gender? Gender {  get; set; }

        [FirestoreProperty]
        [Display(Name = "Date Of Birth")]
        [DataType(DataType.DateTime)]
        public DateTime DateOfBirth { get; set; }

        [FirestoreProperty]
        public string? Address { get; set; }

        [FirestoreProperty]
        [RegularExpression(@"^0\d{9}$"), StringLength(10)]
        public string? PhoneNum { get; set; }

        // Need some classes to implement these field
        [FirestoreProperty]
        public string? MedicalHistory { get; set; }
        [FirestoreProperty]
        public string? TestResult { get; set; }
        [FirestoreProperty]
        public string? TreatmentSchedule { get; set; }
        [FirestoreProperty]
        public string? Status { get; set; }
    }
}

