using System.ComponentModel.DataAnnotations;

namespace insureflow_portal.Models;

public class AuthorizationModel
{
    public Guid Id { get; set; }

    [Required]
    public string AuthorizationNumber { get; set; } = string.Empty;

    public Guid PatientId { get; set; }

    public string PatientName { get; set; } = string.Empty;

    public string PatientMrn { get; set; } = string.Empty;

    public Guid PayerId { get; set; }

    public string PayerName { get; set; } = string.Empty;

    public string AuthorizationType { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";

    public string Priority { get; set; } = "Routine";

    public string PlaceOfService { get; set; } = string.Empty;

    public DateTime RequestedDate { get; set; } = DateTime.Today;

    public DateTime ServiceStartDate { get; set; } = DateTime.Today;

    public DateTime ServiceEndDate { get; set; } = DateTime.Today.AddDays(30);

    public DateTime? ExpirationDate { get; set; }

    public string ClinicalSummary { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    public List<AuthorizationServiceLine> ServiceLines { get; set; } = [];

    public List<AuthorizationDocument> Documents { get; set; } = [];

    public List<AuthorizationHistoryEntry> History { get; set; } = [];

    public string StatusClass =>
        Status
            .ToLowerInvariant()
            .Replace(" ", "-");
}

public class AuthorizationServiceLine
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string ProcedureCode { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int RequestedUnits { get; set; }

    public int ApprovedUnits { get; set; }

    public decimal AllowedAmount { get; set; }
}

public class AuthorizationDocument
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FileName { get; set; } = string.Empty;

    public string DocumentType { get; set; } = string.Empty;

    public DateTime UploadedDate { get; set; } = DateTime.Today;
}

public class AuthorizationHistoryEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime Date { get; set; } = DateTime.Now;

    public string Action { get; set; } = string.Empty;

    public string User { get; set; } = string.Empty;

    public string Details { get; set; } = string.Empty;
}