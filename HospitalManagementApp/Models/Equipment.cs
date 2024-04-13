using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace HospitalManagementApp.Models
{
    public enum EquipmentStatus
    {
        isFree,
        isUsing,
        isFixing
    }

    [FirestoreData]
    public class Equipment
    {
        [FirestoreProperty]
        public static int? Total { get; set; } = 0;

        [FirestoreProperty]
        [Required, Range(1, 9999)]
        public int? Id { get; set; }

        // for doc
        [FirestoreProperty]
        public string? docId { get; set; } = string.Empty;

        [FirestoreProperty]
        [Required, StringLength(60, MinimumLength = 3)]
        public string? Name { get; set; }

        [FirestoreProperty]
        [Required, StringLength(150, MinimumLength = 3)]
        public string? Description { get; set; }

        [FirestoreProperty]
        public EquipmentStatus? Status { get; set; } = EquipmentStatus.isFree;

        [FirestoreProperty(ConverterType = typeof(TreatmentListConverter))]
        public List<Treatment>? Schedule { get; set; }

        // note for checking if anything have changed
        [FirestoreProperty]
        public bool? Edited { get; set; } = true;
    }
}

