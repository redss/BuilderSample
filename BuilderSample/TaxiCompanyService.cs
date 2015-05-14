using System;
using System.Linq;

namespace BuilderSample
{
    public interface ITaxiCompanyService
    {
        void AssignTaxiToOrder(int taxiId, int orderId);
    }

    public class TaxiCompanyService : ITaxiCompanyService
    {
        private readonly TaxiCompanyContext _taxiCompanyContext;

        public TaxiCompanyService(TaxiCompanyContext taxiCompanyContext)
        {
            _taxiCompanyContext = taxiCompanyContext;
        }

        public void AssignTaxiToOrder(int taxiId, int orderId)
        {
            if (_taxiCompanyContext.Orders.Any(o => o.AssignedTaxi.Id == taxiId && o.Status == OrderStatus.Taken))
            {
                throw new InvalidOperationException("Taxi is already assigned to an order!");
            }

            var taxi = _taxiCompanyContext.Taxis.Single(t => t.Id == taxiId);
            var order = _taxiCompanyContext.Orders.Single(t => t.Id == orderId);

            order.Status = OrderStatus.Taken;
            order.AssignedTaxi = taxi;

            _taxiCompanyContext.SaveChanges();
        }
    }
}