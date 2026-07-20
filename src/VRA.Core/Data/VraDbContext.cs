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
}