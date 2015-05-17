using BuilderSample.Model;

namespace BuilderSample.Builders
{
    public class CorporationBuilder
    {
        private readonly TaxiCompanyContext _context;

        private readonly Corporation _corporation;

        public CorporationBuilder(TaxiCompanyContext context)
        {
            _context = context;

            _corporation = new Corporation
            {
                Name = "Taxi Corpo SP ZOO"
            };
        }

        public Corporation Build()
        {
            return _corporation;
        }

        public Corporation BuildAndSave()
        {
            _context.Corporations.Add(_corporation);

            _context.SaveChanges();

            return _corporation;
        }
    }
}