using HireFlow.API.Extensions;
using HireFlow.API.Middleware;
using HireFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordService = scope.ServiceProvider.GetRequiredService<HireFlow.Domain.Interfaces.Services.IPasswordService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Seeder");

    await context.Database.MigrateAsync();
    await Seeder.SeedAsync(context, passwordService, builder.Configuration, logger);

    // Demo data (jobs, candidates, applications, interviews, evaluations) —
    // Development only, and a no-op after the first run / once real
    // candidates exist. See DemoDataSeeder for the idempotency guard.
    if (app.Environment.IsDevelopment())
    {
        await DemoDataSeeder.SeedAsync(context, passwordService);
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "HireFlow API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("HireFlowCors");
app.UseRateLimiter();
app.UseMiddleware<ExceptionMiddleware>();

// CVs used to be served here as public static files at /uploads/**, with no
// auth check at all — anyone with a link (candidate CVs contain PII: name,
// phone, address) could read it. Both real consumers have since moved to
// authenticated endpoints instead:
//   - Candidate: GET /api/my/profile/cv        (MyController)
//   - Company:   GET /api/candidates/{id}/cv   (CandidatesController)
// which stream the file only to the owning candidate or a company user
// authorized to view that candidate. The storage directory below is kept
// only so those two endpoints have somewhere to read from — it is
// intentionally NOT registered as a static file route.
var fileStorageRoot = Path.Combine(
    app.Environment.ContentRootPath,
    builder.Configuration["FileStorage:Path"] ?? "App_Data/uploads");
Directory.CreateDirectory(fileStorageRoot);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
