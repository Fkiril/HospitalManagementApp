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
        public static string CreateCalendar()
        {
            Random random = new Random();
            string result = "";
            string[] week = new string[] { "a2", "b2", "a3", "b3", "a4", "b4", "a5", "b5", "a6", "b6", "a7", "b7", "a8", "b8" };

            foreach (string day in week)
            {
                int ranNum = random.Next(0, 2);
                if(ranNum == 0)
                {
                    result += day + ' ' + "Off" ;
                }
                else
                {
                    result += day + ' ' + "Work" ;
                }
            }

            return result;
        }

        [FirestoreProperty]
        [Required, Range(1, 9999)]
        public int? Id { get; set; }

        [FirestoreProperty]
        public string? patList { get; set; } = string.Empty;

        //[FirestoreProperty]
        //public List<Patient>? patList { get; set; } = new List<Patient>();

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
        [DataType(DataType.DateTime)]
        public DateTime DateOfBirth { get; set; }

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
        public string? WorkSchedule { get; set; }
        [FirestoreProperty]
        public string? Status { get; set; }
    }
}