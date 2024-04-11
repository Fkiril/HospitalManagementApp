using Google.Cloud.Firestore;
using NuGet.Protocol;
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
    public enum Status
    {
        Ill,
        Healed
    }

    [FirestoreData]    
    public class Patient
    {
        [FirestoreProperty]
        [Key, Required, Range(1,9999)]
        public int? Id { get; set; }

        [FirestoreProperty]
        [Required, StringLength(60, MinimumLength = 3)]
        public string? Name { get; set; }

        [FirestoreProperty]
        [Required]
        public Gender? Gender {  get; set; }

        [FirestoreProperty]
        [Display(Name = "Date Of Birth")]
        [RegularExpression(@"^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[012])/[0-9]{4}$", ErrorMessage = "Invalid date format. Please use the format dd/mm/yyyy.")]
        public string? DateOfBirth { get; set; }

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

        [FirestoreProperty(ConverterType = typeof(TreatmentListConverter))]
        public List<Treatment>? TreatmentSchedule { get; set; }

        [FirestoreProperty]
        [Required]
        public List<int>? StaffId { get; set; }

        [FirestoreProperty]
        public Status? Status { get; set; }

        public bool? changed { get; set; }
    }

    public class Treatment
    {
        [FirestoreProperty]
        [Required]
        [RegularExpression(@"^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[012])/[0-9]{4}$", ErrorMessage = "Invalid date format. Please use the format dd/mm/yyyy.")]
        public string? Date { get; set; }

        [FirestoreProperty]
        [Required]
        [RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]):([0-5][0-9])", ErrorMessage = "Invalid time format. Please use the format hh:mm")]
        public string? StartTime { get; set; }

        [FirestoreProperty]
        [Required]
        [RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]):([0-5][0-9])", ErrorMessage = "Invalid time format. Please use the format hh:mm")]
        public string? EndTime { get; set; }

        [FirestoreProperty]
        [Key, Required]
        public int? Id { get; set; }
    }
    

    public class TreatmentListConverter : IFirestoreConverter<List<Treatment>>
    {
        public object TreatmentToFirestore(Treatment value)
        {
            return new Dictionary<string, object>
            {
                { "Date", value.Date ?? new object() },
                { "StartTime", value.StartTime ?? new object() },
                { "EndTime", value.EndTime ?? new object() }
            };
        }
        public Treatment TreatmentFromFirestore(object value)
        {
            var dict = value as Dictionary<string, object>;
            if (dict == null)
            {
                throw new ArgumentException("Expected a dictionary");
            }

            return new Treatment
            {
                Date = (string)(dict["Date"]),
                StartTime = (string)(dict["StartTime"]),
                EndTime = (string)(dict["EndTime"])
            };
        }

        public object ToFirestore(List<Treatment> value)
        {
            Console.WriteLine("TreatmentList ToFirestore");
            return value.Select(t => TreatmentToFirestore(t)).ToList();
        }

        public List<Treatment> FromFirestore(object value)
        {
            Console.WriteLine("TreatmentList FromFirestore");
            var list = value as List<object>;
            if (list == null)
            {
                throw new ArgumentException("Expected a list");
            }

            return list.Select(o => TreatmentFromFirestore(o)).ToList();
        }
    }
}

