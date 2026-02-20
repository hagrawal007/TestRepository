
public sealed record Transaction(
    string TransactionId,
    string Type,
    string TransactionType,
    string Status,
    decimal Amount,
    string Currency,
    string Channel,
    DateTime Timestamp
);
