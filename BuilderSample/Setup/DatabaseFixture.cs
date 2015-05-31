using System;

namespace BuilderSample.Setup
{
    public class DatabaseFixture : IDisposable
    {
        public TaxiCompanyContext Context = new TaxiCompanyContext();

        public DatabaseFixture()
        {
            Context.Database.Delete();
            Context.Database.Create();
        }

        public void ResetContext()
        {
            Context.Dispose();

            Context = new TaxiCompanyContext();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}