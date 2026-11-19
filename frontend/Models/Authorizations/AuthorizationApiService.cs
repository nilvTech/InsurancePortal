using insureflow_portal.Models;

namespace insureflow.portal.Services;

public class AuthorizationApiService
{
    private readonly List<AuthorizationModel> _authorizations =
    [
        new AuthorizationModel
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
            AuthorizationNumber = "AUTH-2026-00001",
            PatientId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
            PatientName = "John Smith",
            PatientMrn = "MRN-10001",
            PayerId = Guid.NewGuid(),
            PayerName = "Aetna",
            AuthorizationType = "Medical Procedure",
            Status = "Pending",
            Priority = "Urgent",
            PlaceOfService = "11 - Office",
            RequestedDate = DateTime.Today.AddDays(-2),
            ServiceStartDate = DateTime.Today.AddDays(5),
            ServiceEndDate = DateTime.Today.AddDays(35),
            ExpirationDate = DateTime.Today.AddDays(35),
            ClinicalSummary = "Authorization requested for outpatient medical procedure.",
            Notes = "Clinical documentation attached.",
            ServiceLines =
            [
                new AuthorizationServiceLine
                {
                    ProcedureCode = "99214",
                    Description = "Established patient office visit",
                    RequestedUnits = 1,
                    ApprovedUnits = 0,
                    AllowedAmount = 185
                }
            ],
            Documents =
            [
                new AuthorizationDocument
                {
                    FileName = "clinical-summary.pdf",
                    DocumentType = "Clinical Documentation",
                    UploadedDate = DateTime.Today.AddDays(-2)
                }
            ],
            History =
            [
                new AuthorizationHistoryEntry
                {
                    Date = DateTime.Today.AddDays(-2),
                    Action = "Created",
                    User = "System",
                    Details = "Authorization request created."
                }
            ]
        },

        new AuthorizationModel
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
            AuthorizationNumber = "AUTH-2026-00002",
            PatientId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
            PatientName = "Mary Jones",
            PatientMrn = "MRN-10002",
            PayerId = Guid.NewGuid(),
            PayerName = "Blue Cross Blue Shield",
            AuthorizationType = "Imaging",
            Status = "Approved",
            Priority = "Routine",
            PlaceOfService = "22 - Outpatient Hospital",
            RequestedDate = DateTime.Today.AddDays(-8),
            ServiceStartDate = DateTime.Today,
            ServiceEndDate = DateTime.Today.AddDays(60),
            ExpirationDate = DateTime.Today.AddDays(60),
            ClinicalSummary = "MRI authorization request.",
            Notes = "Approved by payer.",
            ServiceLines =
            [
                new AuthorizationServiceLine
                {
                    ProcedureCode = "70551",
                    Description = "MRI brain without contrast",
                    RequestedUnits = 1,
                    ApprovedUnits = 1,
                    AllowedAmount = 950
                }
            ],
            History =
            [
                new AuthorizationHistoryEntry
                {
                    Date = DateTime.Today.AddDays(-8),
                    Action = "Submitted",
                    User = "System",
                    Details = "Authorization submitted to payer."
                },
                new AuthorizationHistoryEntry
                {
                    Date = DateTime.Today.AddDays(-5),
                    Action = "Approved",
                    User = "Payer",
                    Details = "Authorization approved."
                }
            ]
        },

        new AuthorizationModel
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000003"),
            AuthorizationNumber = "AUTH-2026-00003",
            PatientId = Guid.Parse("10000000-0000-0000-0000-000000000003"),
            PatientName = "Robert Brown",
            PatientMrn = "MRN-10003",
            PayerId = Guid.NewGuid(),
            PayerName = "UnitedHealthcare",
            AuthorizationType = "Specialist",
            Status = "Denied",
            Priority = "High",
            PlaceOfService = "11 - Office",
            RequestedDate = DateTime.Today.AddDays(-12),
            ServiceStartDate = DateTime.Today.AddDays(2),
            ServiceEndDate = DateTime.Today.AddDays(30),
            ExpirationDate = DateTime.Today.AddDays(30),
            ClinicalSummary = "Specialist referral authorization.",
            Notes = "Additional clinical documentation required.",
            ServiceLines =
            [
                new AuthorizationServiceLine
                {
                    ProcedureCode = "99205",
                    Description = "New patient specialist consultation",
                    RequestedUnits = 1,
                    ApprovedUnits = 0,
                    AllowedAmount = 325
                }
            ],
            History =
            [
                new AuthorizationHistoryEntry
                {
                    Date = DateTime.Today.AddDays(-12),
                    Action = "Created",
                    User = "System",
                    Details = "Authorization request created."
                },
                new AuthorizationHistoryEntry
                {
                    Date = DateTime.Today.AddDays(-7),
                    Action = "Denied",
                    User = "Payer",
                    Details = "Authorization denied."
                }
            ]
        },

        new AuthorizationModel
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000004"),
            AuthorizationNumber = "AUTH-2026-00004",
            PatientId = Guid.Parse("10000000-0000-0000-0000-000000000004"),
            PatientName = "Linda Davis",
            PatientMrn = "MRN-10004",
            PayerId = Guid.NewGuid(),
            PayerName = "Cigna",
            AuthorizationType = "Surgery",
            Status = "Pending",
            Priority = "High",
            PlaceOfService = "21 - Inpatient Hospital",
            RequestedDate = DateTime.Today.AddDays(-1),
            ServiceStartDate = DateTime.Today.AddDays(10),
            ServiceEndDate = DateTime.Today.AddDays(20),
            ExpirationDate = DateTime.Today.AddDays(20),
            ClinicalSummary = "Surgical authorization request.",
            Notes = "Awaiting payer review.",
            ServiceLines =
            [
                new AuthorizationServiceLine
                {
                    ProcedureCode = "27447",
                    Description = "Total knee arthroplasty",
                    RequestedUnits = 1,
                    ApprovedUnits = 0,
                    AllowedAmount = 12500
                }
            ]
        }
    ];

    public Task<List<AuthorizationModel>> GetAllAsync()
    {
        return Task.FromResult(
            _authorizations
                .Select(Clone)
                .ToList());
    }

    public Task<AuthorizationModel?> GetByIdAsync(Guid id)
    {
        var authorization = _authorizations
            .FirstOrDefault(x => x.Id == id);

        return Task.FromResult(
            authorization == null
                ? null
                : Clone(authorization));
    }

    public Task UpdateAsync(
        Guid id,
        AuthorizationRequestModel request)
    {
        var authorization = _authorizations
            .FirstOrDefault(x => x.Id == id);

        if (authorization == null)
        {
            return Task.CompletedTask;
        }

        authorization.PatientId = request.PatientId;
        authorization.PayerId = request.PayerId;
        authorization.AuthorizationType = request.AuthorizationType;
        authorization.Priority = request.Priority;
        authorization.PlaceOfService = request.PlaceOfService;
        authorization.ServiceStartDate = request.ServiceStartDate;
        authorization.ServiceEndDate = request.ServiceEndDate;
        authorization.ClinicalSummary = request.ClinicalSummary;
        authorization.Notes = request.Notes;
        authorization.ServiceLines = request.ServiceLines;

        authorization.History.Add(
            new AuthorizationHistoryEntry
            {
                Date = DateTime.Now,
                Action = "Updated",
                User = "Current User",
                Details = "Authorization details updated."
            });

        return Task.CompletedTask;
    }

    public Task SubmitAsync(Guid id)
    {
        var authorization = _authorizations
            .FirstOrDefault(x => x.Id == id);

        if (authorization != null)
        {
            authorization.Status = "Submitted";

            authorization.History.Add(
                new AuthorizationHistoryEntry
                {
                    Date = DateTime.Now,
                    Action = "Submitted",
                    User = "Current User",
                    Details = "Authorization submitted for payer review."
                });
        }

        return Task.CompletedTask;
    }

    public Task AppealAsync(
        Guid id,
        string reason)
    {
        var authorization = _authorizations
            .FirstOrDefault(x => x.Id == id);

        if (authorization != null)
        {
            authorization.Status = "Appeal Submitted";

            authorization.History.Add(
                new AuthorizationHistoryEntry
                {
                    Date = DateTime.Now,
                    Action = "Appeal Submitted",
                    User = "Current User",
                    Details = reason
                });
        }

        return Task.CompletedTask;
    }

    private static AuthorizationModel Clone(
        AuthorizationModel source)
    {
        return new AuthorizationModel
        {
            Id = source.Id,
            AuthorizationNumber = source.AuthorizationNumber,
            PatientId = source.PatientId,
            PatientName = source.PatientName,
            PatientMrn = source.PatientMrn,
            PayerId = source.PayerId,
            PayerName = source.PayerName,
            AuthorizationType = source.AuthorizationType,
            Status = source.Status,
            Priority = source.Priority,
            PlaceOfService = source.PlaceOfService,
            RequestedDate = source.RequestedDate,
            ServiceStartDate = source.ServiceStartDate,
            ServiceEndDate = source.ServiceEndDate,
            ExpirationDate = source.ExpirationDate,
            ClinicalSummary = source.ClinicalSummary,
            Notes = source.Notes,

            ServiceLines = source.ServiceLines
                .Select(x => new AuthorizationServiceLine
                {
                    Id = x.Id,
                    ProcedureCode = x.ProcedureCode,
                    Description = x.Description,
                    RequestedUnits = x.RequestedUnits,
                    ApprovedUnits = x.ApprovedUnits,
                    AllowedAmount = x.AllowedAmount
                })
                .ToList(),

            Documents = source.Documents
                .Select(x => new AuthorizationDocument
                {
                    Id = x.Id,
                    FileName = x.FileName,
                    DocumentType = x.DocumentType,
                    UploadedDate = x.UploadedDate
                })
                .ToList(),

            History = source.History
                .Select(x => new AuthorizationHistoryEntry
                {
                    Id = x.Id,
                    Date = x.Date,
                    Action = x.Action,
                    User = x.User,
                    Details = x.Details
                })
                .ToList()
        };
    }
}