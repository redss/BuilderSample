using System;
using System.Linq;
using BuilderSample.Model;

namespace BuilderSample
{
    public class TaxiHasOngoingOrderException : Exception
    {
    }

    public class OrderAlreadyTakenException : Exception
    {
    }

    public class OrderAlreadyCompletedException : Exception
    {
    }

    public interface ITaxiCompanyService
    {
        void AssignTaxiToOrder(int taxiId, int orderId);
    }

    public class TaxiCompanyService : ITaxiCompanyService
    {
        public void AssignTaxiToOrder(int taxiId, int orderId)
        {
            using (var context = new TaxiCompanyContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                if (context.Orders.Any(o => o.AssignedTaxi.Id == taxiId && o.Status == OrderStatus.Ongoing))
                {
                    throw new TaxiHasOngoingOrderException();
                }

                var taxi = context.Taxis.SingleOrDefault(t => t.Id == taxiId);

                if (taxi == null)
                {
                    throw new InvalidOperationException("Taxi " + taxiId + " was not found.");
                }

                var order = context.Orders.SingleOrDefault(t => t.Id == orderId);

                if (order == null)
                {
                    throw new InvalidOperationException("Order " + orderId + " was not found.");
                }

                if (order.Status == OrderStatus.Ongoing)
                {
                    throw new OrderAlreadyTakenException();
                }

                if (order.Status == OrderStatus.Completed)
                {
                    throw new OrderAlreadyCompletedException();
                }

                order.Status = OrderStatus.Ongoing;
                order.AssignedTaxi = taxi;

                context.SaveChanges();
                transaction.Commit();
            }
        }
    }
}