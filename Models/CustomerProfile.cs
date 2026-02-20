
public sealed record CustomerProfile(
    int CustomerId,
    string KycLevel,
    string Occupation,
    string RelationshipType,
    string RiskRating,
    string Segment,
    IReadOnlyList<string> LinkedAccounts
);
