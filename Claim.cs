using System;
using System.ComponentModel.DataAnnotations;

namespace Prog_Poe.Models
{
    public enum ClaimStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class Claim
    {
        public string Id { get; set; }

        public string LecturerId { get; set; }
        public string LecturerName { get; set; }
        public DateTime SubmissionDate { get; set; }

        // Validation: Matches "The HoursWorked field is required."
        [Required(ErrorMessage = "The HoursWorked field is required.")]
        [Range(0, 160, ErrorMessage = "HoursWorked must be between 0 and 160.")]
        public double? HoursWorked { get; set; }

        // Validation: Matches "The HourlyRate field is required."
        [Required(ErrorMessage = "The HourlyRate field is required.")]
        [Range(0, 350, ErrorMessage = "HourlyRate cannot exceed R350.00.")]
        public double? HourlyRate { get; set; }

        public string ClaimType { get; set; }
        public double Amount { get; set; }

        public double TotalAmount => (HoursWorked ?? 0) * (HourlyRate ?? 0);

        [Required(ErrorMessage = "The Description field is required.")]
        public string Description { get; set; }

        // Validated in Controller to give "The FileName field is required."
        public string FileName { get; set; }

        public ClaimStatus Status { get; set; }

        // Approval Metadata
        public string RejectionReason { get; set; }
        public string ReviewerName { get; set; }
        public DateTime? ReviewDate { get; set; }

        public string ReviewedBy { get; set; }



    }
}