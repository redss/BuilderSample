using BuilderSample.Model;
using TechTalk.SpecFlow.Bindings;

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
                Corporation = new CorporationBuilder(_context).Build()
            };
        }

        public TaxiBuilder WithLicensePlate(string licensePlate)
        {
            _taxi.LicensePlate = licensePlate;

            return this;
        }

        public Taxi Build()
        {
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