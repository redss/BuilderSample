using System.Data.Entity;

namespace BuilderSample
{
    public class TaxiCompanyContext : DbContext
    {
        public DbSet<Driver> Persons { get; set; }
        public DbSet<Taxi> Taxis { get; set; }
        public DbSet<Fleet> Fleets { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}