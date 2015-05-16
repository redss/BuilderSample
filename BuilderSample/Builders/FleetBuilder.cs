using BuilderSample.Model;

namespace BuilderSample.Builders
{
    public class FleetBuilder
    {
        private readonly TaxiCompanyContext _context;

        private readonly Fleet _fleet;

        public FleetBuilder(TaxiCompanyContext context)
        {
            _context = context;

            _fleet = new Fleet
            {
                Name = "Taxi Corpo SP ZOO"
            };
        }

        public Fleet Build()
        {
            return _fleet;
        }

        public Fleet BuildAndSave()
        {
            _context.Fleets.Add(_fleet);

            _context.SaveChanges();

            return _fleet;
        }
    }
}