using System;
using BuilderSample.Model;

namespace BuilderSample.Builders
{
    public class OrderBuilder
    {
        private readonly TaxiCompanyContext _context;

        private readonly Order _order;

        public OrderBuilder(TaxiCompanyContext context)
        {
            _context = context;

            _order = new Order
            {
                Address = "Gliwice Akademicka 100",
                RequiredTime = new DateTime(2015, 5, 30, 14, 0, 0),
                Status = OrderStatus.New
            };
        }

        public OrderBuilder WithId(int orderId)
        {
            _order.Id = orderId;

            return this;
        }

        public OrderBuilder WithStatus(OrderStatus orderStatus)
        {
            _order.Status = orderStatus;

            return this;
        }

        public OrderBuilder WithSomeAssignedTaxi()
        {
            _order.AssignedTaxi = new TaxiBuilder(_context)
                .Build();

            return this;
        }

        public OrderBuilder WithAssignedTaxi(Taxi assignedTaxi)
        {
            _order.AssignedTaxi = assignedTaxi;

            return this;
        }
        
        public Order Build()
        {
            return _order;
        }

        public Order BuildAndSave()
        {
            _context.Orders.Add(_order);

            _context.SaveChanges();

            return _order;
        }
    }
}