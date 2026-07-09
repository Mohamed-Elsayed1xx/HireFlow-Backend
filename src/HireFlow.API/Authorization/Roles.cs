namespace HireFlow.API.Authorization;

/// <summary>
/// Role names as they appear in the JWT "role" claim. Kept as plain strings
/// (rather than the Domain UserRole enum) because the Candidate identity
/// is a separate entity from User and therefore has no enum member.
/// </summary>
public static class Roles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string CompanyAdmin = "CompanyAdmin";
    public const string HRManager = "HRManager";
    public const string HiringManager = "HiringManager";
    public const string Candidate = "Candidate";

    /// <summary>Anyone who belongs to a company, regardless of seniority.</summary>
    public const string AnyCompanyUser = CompanyAdmin + "," + HRManager + "," + HiringManager;

    /// <summary>Company users who manage hiring policy and reporting (not entry-level hiring managers).</summary>
    public const string CompanyLeadership = CompanyAdmin + "," + HRManager;
}
