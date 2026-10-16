using Microsoft.EntityFrameworkCore;
using VRA.Core.Models;

namespace VRA.Core.Data;

public class VraDbContext : DbContext
{
    public VraDbContext(DbContextOptions<VraDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients => Set<Patient>();

    public DbSet<HearingTestSession> HearingTestSessions
    => Set<HearingTestSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<HearingTestSession>()
            .HasOne(session => session.Patient)
            .WithMany()
            .HasForeignKey(session => session.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}