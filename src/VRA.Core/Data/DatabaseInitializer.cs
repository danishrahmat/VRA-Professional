
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VRA.Core.Models;

namespace VRA.Core.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<VraDbContext>();

        // Create the database if it does not exist.
        await context.Database.EnsureCreatedAsync();

        // Add the hearing test session table to an existing database.
        await context.Database.ExecuteSqlRawAsync(
            """
            CREATE TABLE IF NOT EXISTS HearingTestSessions (
                Id TEXT NOT NULL PRIMARY KEY,
                PatientId TEXT NOT NULL,
                StartedAt TEXT NOT NULL,
                CompletedAt TEXT NULL,
                Status INTEGER NOT NULL,
                Notes TEXT NOT NULL,
                FOREIGN KEY (PatientId) REFERENCES Patients (Id)
                    ON DELETE RESTRICT
            );
            """);

        // Index sessions by patient for efficient history lookup.
        await context.Database.ExecuteSqlRawAsync(
            """
            CREATE INDEX IF NOT EXISTS IX_HearingTestSessions_PatientId
            ON HearingTestSessions (PatientId);
            """);
    }
}
