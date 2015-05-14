using System;
using System.Linq;

namespace BuilderSample
{
    public interface ITaxiCompanyService
    {
        void AssignTaxiToOrder(int taxiId, int orderId);
    }

    public class TaxiHasOngoingOrderAlready : Exception
    {
    }

    public class OrderAlreadyTakenException : Exception
    {
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
                throw new TaxiHasOngoingOrderAlready();
            }

            var taxi = _taxiCompanyContext.Taxis.Single(t => t.Id == taxiId);
            var order = _taxiCompanyContext.Orders.Single(t => t.Id == orderId);

            if (order.Status == OrderStatus.Taken)
            {
                throw new OrderAlreadyTakenException();
            }

            order.Status = OrderStatus.Taken;
            order.AssignedTaxi = taxi;

            _taxiCompanyContext.SaveChanges();
        }
    }
}