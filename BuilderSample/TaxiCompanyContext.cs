using System.Data.Entity;
using BuilderSample.Model;

namespace BuilderSample
{
    public class TaxiCompanyContext : DbContext
    {
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Taxi> Taxis { get; set; }
        public DbSet<Corporation> Corporations { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}