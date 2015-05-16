using BuilderSample.Model;

namespace BuilderSample.Builders
{
    public class DriverBuilder
    {
        private readonly TaxiCompanyContext _context;

        private readonly Driver _driver = new Driver
        {
            FirstName = "Jan",
            Surname = "Kowalski"
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
            _context.Persons.Add(_driver);

            _context.SaveChanges();

            return _driver;
        }
    }
}