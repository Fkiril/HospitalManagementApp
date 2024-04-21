using Google.Cloud.Firestore;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace HospitalManagementApp.Models
{
    [FirestoreData]
    public class Equipment
    {
        [FirestoreProperty]
        public int? TotalCount { get; set; } = 1;
        [FirestoreProperty]
        public int? AvailableCount { get; set; } = 1;

        [FirestoreProperty]
        [Required, Range(1, 9999)]
        public int? Id { get; set; }

        [FirestoreProperty]
        [Required, StringLength(60, MinimumLength = 3)]
        public string? Name { get; set; }

        [FirestoreProperty]
        [Required, StringLength(150, MinimumLength = 3)]
        public string? Description { get; set; }

        [FirestoreProperty]
        public bool? IsAvailable { get; set; } = true;

        [FirestoreProperty]
        public DateTime? UsingUntil { get; set; } = null;

        [FirestoreProperty]
        public List<DateTime>? History { get; set; } = default;

        // note for checking if anything have changed
        [FirestoreProperty]
        public bool? changed { get; set; } = true;
    }
}

