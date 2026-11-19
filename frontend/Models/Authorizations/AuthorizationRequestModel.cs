using System.ComponentModel.DataAnnotations;

namespace insureflow_portal.Models;

public class AuthorizationRequestModel
{
    [Required]
    public Guid PatientId { get; set; }

    [Required]
    public Guid PayerId { get; set; }

    [Required]
    [StringLength(100)]
    public string AuthorizationType { get; set; } = string.Empty;

    [Required]
    public string Priority { get; set; } = "Routine";

    [Required]
    public string PlaceOfService { get; set; } = string.Empty;

    [Required]
    public DateTime ServiceStartDate { get; set; } = DateTime.Today;

    [Required]
    public DateTime ServiceEndDate { get; set; } = DateTime.Today.AddDays(30);

    [StringLength(5000)]
    public string ClinicalSummary { get; set; } = string.Empty;

    [StringLength(5000)]
    public string Notes { get; set; } = string.Empty;

    public List<AuthorizationServiceLine> ServiceLines { get; set; } = [];
}