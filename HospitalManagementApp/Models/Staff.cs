using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;

namespace HospitalManagementApp.Models
{
    public enum HealthCareStaff
    {
        Doctor,
        Nurse,
        SupportStaff
    }


    [FirestoreData]
    public class Staff
    {
        
        [FirestoreProperty]
        [Key,Required, Range(1, 9999)]
        public int Id { get; set; }

        [FirestoreProperty]
        [Required, StringLength(60, MinimumLength = 3)]
        public string? Name { get; set; }

        [FirestoreProperty]
        [Required]
        public Gender? Gender { get; set; }

        [FirestoreProperty]
        [Required]
        public HealthCareStaff? HealthCareStaff { get; set; }

        [FirestoreProperty]
        [Display(Name = "Date Of Birth")]
        [RegularExpression(@"^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[012])/[0-9]{4}$", ErrorMessage = "Invalid date format. Please use the format dd/mm/yyyy.")]
        public string? DateOfBirth { get; set; }

        [FirestoreProperty]
        public string? Email { get; set; }

        [FirestoreProperty]
        [RegularExpression(@"^0\d{9}$"), StringLength(10)]
        public string? PhoneNum { get; set; }

        // Need some classes to implement these field
        [FirestoreProperty]
        public string? Degree { get; set; }
        [FirestoreProperty]
        public string? Specialist { get; set; }
        [FirestoreProperty]
        public string? Department { get; set; }
        [FirestoreProperty]
        public string? WorkSchedule { get; set; } = String.Empty;

        public bool? changed { get; set; }

    }
}