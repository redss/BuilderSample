using BuilderSample.Model;

namespace BuilderSample.Builders
{
    public class DriverBuilder
    {
        private readonly TaxiCompanyContext _context;

        private readonly Driver _driver = new Driver
        {
            FirstName = "Jan",
            Surname = "Kowalski",
            //Email = "driver@qa.com"
        };

        public DriverBuilder(TaxiCompanyContext context)
        {
            _context = context;
        }

        public Driver Build()
        {
            return _driver;
        }

        public Driver BuildAndSave()
        {
            _context.Drivers.Add(_driver);

            _context.SaveChanges();

            return _driver;
        }
    }
}