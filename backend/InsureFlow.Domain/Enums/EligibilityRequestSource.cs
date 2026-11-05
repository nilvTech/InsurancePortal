using InsureFlow.Domain.Common;
using InsureFlow.Domain.Enums;

namespace InsureFlow.Domain.Entities;

public enum EligibilityRequestSource
{
    API = 1,
    Portal = 2,
    Batch = 3,
    Scheduled = 4,
    System = 5
}