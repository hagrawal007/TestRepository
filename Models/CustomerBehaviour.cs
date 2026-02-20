
public sealed record CustomerBehaviour(
    int Id,
    int CustomerId,
    int AvgNoOfMonthlyTransaction,
    decimal AvgTransactionAmount,
    decimal MaxTransactionAmount,
    IReadOnlyList<string> PreferredChannels,
    string TransactionPattern,
    int LoginFrequency,
    DateTime LastActivityTimeStamp
);
