using HollowCrescentMoonCalculator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapPost("/api/calculate", (MoonRequest req) =>
{
    try
    {
        var result = VolumeCalculator.Compute(
            req.OuterRadius,
            req.CutterRadius,
            req.CenterOffset,
            req.WallThickness
        );
        return Results.Ok(result);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }));

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");

public record MoonRequest(
    double OuterRadius,
    double CutterRadius,
    double CenterOffset,
    double WallThickness
);
