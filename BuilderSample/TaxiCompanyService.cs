using System;
using System.Linq;
using BuilderSample.Model;

namespace BuilderSample
{
    public interface ITaxiCompanyService
    {
        void AssignTaxiToOrder(int taxiId, int orderId);
    }

    public class TaxiHasOngoingOrderAlreadyException : Exception
    {
    }

    public class OrderAlreadyTakenException : Exception
    {
    }

    public class TaxiCompanyService : ITaxiCompanyService
    {
        public void AssignTaxiToOrder(int taxiId, int orderId)
        {
            using (var context = new TaxiCompanyContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                if (context.Orders.Any(o => o.AssignedTaxi.Id == taxiId && o.Status == OrderStatus.Taken))
                {
                    throw new TaxiHasOngoingOrderAlreadyException();
                }

                var taxi = context.Taxis.Single(t => t.Id == taxiId);
                var order = context.Orders.Single(t => t.Id == orderId);

                if (order.Status == OrderStatus.Taken)
                {
                    throw new OrderAlreadyTakenException();
                }

                order.Status = OrderStatus.Taken;
                order.AssignedTaxi = taxi;

                context.SaveChanges();

                transaction.Commit();
            }
        }
    }
}