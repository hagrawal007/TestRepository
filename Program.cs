using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<JsonDataLoader>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AI Tool API",
        Version = "v1"
    });
});

var app = builder.Build();

var loader = app.Services.GetRequiredService<JsonDataLoader>();

var alerts = loader.Load<Alert>("DataSource/alerts.json");
var crmInsights = loader.Load<CrmInsight>("DataSource/crmInsights.json");
var customerProfiles = loader.Load<CustomerProfile>("DataSource/customerProfiles.json");
var customerBehaviours = loader.Load<CustomerBehaviour>("DataSource/customerBehaviours.json");
var transactions = loader.Load<Transaction>("DataSource/transactions.json");

// =======================
// ALERT ENDPOINTS
// =======================

/// <summary>
/// Returns all alerts in the system.
/// Useful for AI agents to scan global risk signals.
/// </summary>
app.MapGet("/alerts", () => alerts);

/// <summary>
/// Returns a single alert by its unique alert Id.
/// </summary>
app.MapGet("/alerts/{id:int}", (int id) =>
    alerts.FirstOrDefault(a => a.Id == id)
        is Alert alert ? Results.Ok(alert) : Results.NotFound());

/// <summary>
/// Returns alerts filtered by severity (e.g. High, Medium, Low).
/// </summary>
app.MapGet("/alerts/by-severity/{severity}", (string severity) =>
    alerts.Where(a => a.Severity.Equals(severity, StringComparison.OrdinalIgnoreCase)));

/// <summary>
/// Returns all alerts for a specific customer.
/// </summary>
app.MapGet("/customers/{customerId:int}/alerts", (int customerId) =>
    alerts.Where(a => a.CustomerId == customerId));


// =======================
// CRM INSIGHTS ENDPOINTS
// =======================

/// <summary>
/// Returns all CRM insights.
/// Used by AI agents for opportunity and recommendation generation.
/// </summary>
app.MapGet("/crm-insights", () => crmInsights);

/// <summary>
/// Returns CRM insights for a specific customer.
/// </summary>
app.MapGet("/customers/{customerId:int}/crm-insights", (int customerId) =>
    crmInsights.Where(i => i.CustomerId == customerId));

/// <summary>
/// Returns high priority CRM insights only.
/// Priority threshold can be tuned by AI.
/// </summary>
app.MapGet("/crm-insights/high-priority", () =>
    crmInsights.Where(i => i.Priority >= 8));


// =======================
// CUSTOMER PROFILE ENDPOINTS
// =======================

/// <summary>
/// Returns all customer profiles.
/// </summary>
app.MapGet("/customers/profiles", () => customerProfiles);

/// <summary>
/// Returns a customer profile by customerId.
/// </summary>
app.MapGet("/customers/{customerId:int}/profile", (int customerId) =>
    customerProfiles.FirstOrDefault(p => p.CustomerId == customerId)
        is CustomerProfile profile ? Results.Ok(profile) : Results.NotFound());


// =======================
// CUSTOMER BEHAVIOUR ENDPOINTS
// =======================

/// <summary>
/// Returns all customer behavioural records.
/// </summary>
app.MapGet("/customers/behaviours", () => customerBehaviours);

/// <summary>
/// Returns behavioural data for a specific customer.
/// Useful for AI risk or churn analysis.
/// </summary>
app.MapGet("/customers/{customerId:int}/behaviour", (int customerId) =>
    customerBehaviours.FirstOrDefault(b => b.CustomerId == customerId)
        is CustomerBehaviour behaviour ? Results.Ok(behaviour) : Results.NotFound());


// =======================
// TRANSACTION ENDPOINTS
// =======================

/// <summary>
/// Returns all transactions.
/// </summary>
app.MapGet("/transactions", () => transactions);

/// <summary>
/// Returns transactions for a specific customer.
/// </summary>
app.MapGet("/customers/{customerId:int}/transactions", (int customerId) =>
    transactions.Where(t => alerts.Any(a => a.CustomerId == customerId)));

/// <summary>
/// Returns transactions filtered by status (e.g. Success, Failed).
/// </summary>
app.MapGet("/transactions/by-status/{status}", (string status) =>
    transactions.Where(t => t.Status.Equals(status, StringComparison.OrdinalIgnoreCase)));


// =======================
// AI-SUMMARY / DERIVED ENDPOINTS
// =======================

/// <summary>
/// Returns a lightweight risk summary for a customer.
/// Ideal for AI agent decision making.
/// </summary>
app.MapGet("/customers/{customerId:int}/risk-summary", (int customerId) =>
{
    var customerAlerts = alerts.Where(a => a.CustomerId == customerId).ToList();

    return Results.Ok(new
    {
        CustomerId = customerId,
        TotalAlerts = customerAlerts.Count,
        HighSeverityAlerts = customerAlerts.Count(a => a.Severity == "High"),
        AverageRiskScore = customerAlerts.Any()
            ? customerAlerts.Average(a => a.RiskScore)
            : 0
    });
});


/// <summary>
/// Health check endpoint used for monitoring and AI readiness validation.
/// </summary>
app.MapGet("/health", () =>
    Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

// Default root endpoint forwards to /health
app.MapGet("/", () => Results.Redirect("/health"));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
