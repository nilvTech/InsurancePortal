namespace InsureFlow.Portal.Models.Authorization;

public class AuthorizationServiceLineModel
{
    public Guid Id { get; set; }

    public string CptHcpcsCode { get; set; } = string.Empty;

    public string ProcedureDescription { get; set; } = string.Empty;

    public string Modifier { get; set; } = string.Empty;

    public string Icd10Code { get; set; } = string.Empty;

    public string DiagnosisDescription { get; set; } = string.Empty;

    public int RequestedUnits { get; set; }

    public int ApprovedUnits { get; set; }

    public int UsedUnits { get; set; }

    public string UnitType { get; set; } = "Units";

    public DateTime ServiceStartDate { get; set; }

    public DateTime ServiceEndDate { get; set; }

    public string Status { get; set; } = "Requested";
}