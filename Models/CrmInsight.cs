
public sealed record CrmInsight(
    int Id,
    int CustomerId,
    string CustomerName,
    string InsightType,
    string Category,
    int Priority,
    string Status,
    decimal PotentialRevenue,
    DateTime GeneratedAt
);
