using BuilderSample.Model;

namespace BuilderSample.Builders
{
    public class TaxiBuilder
    {
        private readonly TaxiCompanyContext _context;

        private readonly Taxi _taxi;

        public TaxiBuilder(TaxiCompanyContext context)
        {
            _context = context;

            _taxi = new Taxi
            {
                LicensePlate = "S1TAXI",
                Owner = new DriverBuilder(_context).Build(),
                Fleet = new FleetBuilder(_context).Build()
            };
        }

        public Taxi Build()
        {
            _context.Taxis.Add(_taxi);

            _context.SaveChanges();

            return _taxi;
        }

        public Taxi BuildAndSave()
        {
            _context.Taxis.Add(_taxi);

            _context.SaveChanges();

            return _taxi;
        }
    }
}