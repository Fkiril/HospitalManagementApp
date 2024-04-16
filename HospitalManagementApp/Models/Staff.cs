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
    public enum Shift
    {
        Morning,
        Afternoon,
        Evening
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
        public Calendar? WorkSchedule { get; set; }

        public bool? changed { get; set; }
    }


    public class Calendar
    {
        [FirestoreProperty]
        [Required]
        [RegularExpression(@"^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[012])/[0-9]{4}$", ErrorMessage = "Invalid date format. Please use the format dd/mm/yyyy.")]
        public List<string>? Date { get; set; }

        [FirestoreProperty]
        [Required]
        public List<Shift>? DayofWeek { get; set; }

        public Calendar()
        {
            Date = new List<string>();
            DayofWeek = new List<Shift>();
        }
    }

    public class CalendarListConverter : IFirestoreConverter<List<Calendar>>
    {
        public object CalendarToFirestore(Calendar value)
        {
            return new Dictionary<string, object>
            {
                { "Date", value.Date ?? new object() },
                { "DayofWeek", value.DayofWeek ?? new object() }
            };
        }

        public Calendar CalendarFromFirestore(object value)
        {
            var dict = value as Dictionary<string, object>;
            if (dict == null)
            {
                throw new ArgumentException("Expected a dictionary");
            }

            return new Calendar
            {
                Date = (List<string>)(dict["Date"]),
                DayofWeek = (List<Shift>) (dict["DayofWeek"])
            };
        }

        public List<Calendar> FromFirestore(object value)
        {
            Console.WriteLine("CalendarList FromFirestore");
            var list = value as List<object>;
            if (list == null)
            {
                throw new ArgumentException("Expected a list");
            }

            return list.Select(o => CalendarFromFirestore(o)).ToList();
        }

        public object ToFirestore(List<Calendar> value)
        {
            Console.WriteLine("Calendar ToFirestore");
            return value.Select(c => CalendarToFirestore(c)).ToList();
        }

    }

}