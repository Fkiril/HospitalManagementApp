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
    public enum PatientStatus
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
        [FirestoreProperty(ConverterType = typeof(MedicalHistoryConverter))]
        public List<MedicalHistoryEle>? MedicalHistory { get; set; }
        [FirestoreProperty(ConverterType = typeof(TestResultConverter))]
        public TestResult? TestResult { get; set; }

        [FirestoreProperty(ConverterType = typeof(TreatmentScheduleConverter))]
        public List<TreatmentScheduleEle>? TreatmentSchedule { get; set; }

        //[FirestoreProperty]
        //public Prescription? Prescription { get; set; }

        [FirestoreProperty]
        public List<int>? StaffId { get; set; }

        [FirestoreProperty]
        public PatientStatus? Status { get; set; }

        public bool? Changed { get; set; }
    }

    public class TestResult
    {
        [FirestoreProperty]
        [Required]
        public required string Disease { get; set; }
        [FirestoreProperty]
        [Required]
        [RegularExpression(@"^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[012])/[0-9]{4}$", ErrorMessage = "Invalid date format. Please use the format dd/mm/yyyy.")]
        public required string StartDate { get; set; }
    }

    public class TestResultConverter : IFirestoreConverter<TestResult>
    {
        public object ToFirestore(TestResult value)
        {
            return new Dictionary<string, object>
            {
                { "Disease", value.Disease },
                { "StartDate", value.StartDate }
            };
        }
        public TestResult FromFirestore(object value)
        {
            Dictionary<string, object>? dict = value as Dictionary<string, object>;
            if (dict is null) throw new ArgumentNullException("dict");
            return new TestResult
            {
                Disease = (string)dict["Disease"],
                StartDate = (string)dict["StartDate"]
            };
        }
    }

    public class MedicalHistoryEle
    {
        [FirestoreProperty]
        [Required]
        public required string Disease { get; set; }
        [FirestoreProperty]
        [RegularExpression(@"^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[012])/[0-9]{4}$", ErrorMessage = "Invalid date format. Please use the format dd/mm/yyyy.")]
        public required string StartDate { get; set; }
        [FirestoreProperty]
        [Required]
        [RegularExpression(@"^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[012])/[0-9]{4}$", ErrorMessage = "Invalid date format. Please use the format dd/mm/yyyy.")]
        public required string EndDate { get; set; }
    }

    public class MedicalHistoryConverter : IFirestoreConverter<List<MedicalHistoryEle>>
    {
        public object MedicalHistoryEleToFirestore(MedicalHistoryEle value)
        {
            return new Dictionary<string, object>
            {
                { "Disease", value.Disease },
                { "StartDate", value.StartDate },
                { "EndDate", value.EndDate}
            };
        }
        public MedicalHistoryEle MedicalHistoryEleFromFirestore(object value)
        {
            var dict = value as Dictionary<string, object> ?? throw new ArgumentException("Expected a dictionary");
            return new MedicalHistoryEle
            {
                Disease = (string)dict["Disease"],
                StartDate = (string)dict["StartDate"],
                EndDate = (string)dict["EndDate"]
            };
        }

        public object ToFirestore(List<MedicalHistoryEle> value)
        {
            return value.Select(m => MedicalHistoryEleToFirestore(m)).ToList();
        }
        public List<MedicalHistoryEle> FromFirestore(object value)
        {
            if (value is null) return [];
            List<object>? list = value as List<object>;
            if (list is null) return [];
            return list.Select(o => MedicalHistoryEleFromFirestore(o)).ToList();
        }
    }

    public class TreatmentScheduleEle
    {
        [FirestoreProperty]
        [Required]
        [RegularExpression(@"^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[012])/[0-9]{4}$", ErrorMessage = "Invalid date format. Please use the format dd/mm/yyyy.")]
        public required string Date { get; set; }

        [FirestoreProperty]
        [Required]
        [RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]):([0-5][0-9])", ErrorMessage = "Invalid time format. Please use the format hh:mm")]
        public required string StartTime { get; set; }

        [FirestoreProperty]
        [Required]
        [RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]):([0-5][0-9])", ErrorMessage = "Invalid time format. Please use the format hh:mm")]
        public required string EndTime { get; set; }

        [FirestoreProperty]
        [Key, Required]
        public required int Id { get; set; }
    }
    

    public class TreatmentScheduleConverter : IFirestoreConverter<List<TreatmentScheduleEle>>
    {
        public object TreatmentScheduleEleToFirestore(TreatmentScheduleEle value)
        {
            return new Dictionary<string, object>
            {
                { "Date", value.Date },
                { "StartTime", value.StartTime },
                { "EndTime", value.EndTime },
                { "Id", value.Id }
            };
        }
        public TreatmentScheduleEle TreatmentScheduleEleFromFirestore(object value)
        {
            var dict = value as Dictionary<string, object> ?? throw new ArgumentException("Expected a dictionary");
            return new TreatmentScheduleEle
            {
                Date = (string)dict["Date"],
                StartTime = (string)dict["StartTime"],
                EndTime = (string)dict["EndTime"],
                Id = (int)(long)dict["Id"]
            };
        }

        public object ToFirestore(List<TreatmentScheduleEle> value)
        {
            return value.Select(t => TreatmentScheduleEleToFirestore(t)).ToList();
        }

        public List<TreatmentScheduleEle> FromFirestore(object value)
        {
            var list = value as List<object> ?? throw new ArgumentException("Expected a list");
            return list.Select(o => TreatmentScheduleEleFromFirestore(o)).ToList();
        }
    }
}

