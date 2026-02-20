
public sealed record Alert(
    int Id,
    string AlertCode,
    string AlertType,
    string AlertSource,
    string Severity,
    int CustomerId,
    string CustomerName,
    string AccountNo,
    decimal Amount,
    string Currency,
    string Status,
    int RiskScore
);
