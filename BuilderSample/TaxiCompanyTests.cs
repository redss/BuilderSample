using System;
using NUnit.Framework;

namespace BuilderSample
{
    [TestFixture]
    public class TaxiCompanyTests
    {
        public TaxiCompanyContext TaxiCompanyContext;

        public TaxiCompanyService TaxiCompanyService;

        [SetUp] 
        public void SetUp()
        {
            TaxiCompanyContext = new TaxiCompanyContext();

            TaxiCompanyContext.Database.Delete();
            TaxiCompanyContext.Database.Create();

            TaxiCompanyService = new TaxiCompanyService(new TaxiCompanyContext());
        }

        [Test]
        public void Can_Assign_Taxi_To_A_New_Order()
        {
            // arrange

            var taxi = new Taxi
            {
                LicensePlate = "S1TAXI",

                Owner = new Driver
                {
                    FirstName = "Jan",
                    Surname = "Kowalski"
                },

                Fleet = new Fleet
                {
                    Name = "Taxi Corpo SP ZOO"
                }
            };

            TaxiCompanyContext.Taxis.Add(taxi);

            var order = new Order
            {
                Address = "Gliwice Akademicka 100",
                RequiredTime = new DateTime(2015, 5, 30, 14, 0, 0),
                Status = OrderStatus.Open
            };

            TaxiCompanyContext.Orders.Add(order);

            TaxiCompanyContext.SaveChanges();

            // act

            TaxiCompanyService.AssignTaxiToOrder(taxi.Id, order.Id);

            ResetContext();

            // assert

            var chengedOrder = TaxiCompanyContext.Orders.Find(order.Id);

            Assert.That(chengedOrder.Status, Is.EqualTo(OrderStatus.Taken));
            Assert.That(chengedOrder.AssignedTaxi, Is.Not.Null);
            Assert.That(chengedOrder.AssignedTaxi.Id, Is.EqualTo(taxi.Id));
        }

        [Test]
        public void Assign_Taxi_Fails_When_Taxi_Is_Already_Assigned_To_Another_Order()
        {
            // arrange

            var taxi = new Taxi
            {
                LicensePlate = "S1TAXI",

                Owner = new Driver
                {
                    FirstName = "Jan",
                    Surname = "Kowalski"
                },

                Fleet = new Fleet
                {
                    Name = "Taxi Corpo SP ZOO"
                }
            };

            TaxiCompanyContext.Taxis.Add(taxi);

            var alreadyTakenOrder = new Order
            {
                Address = "Gliwice Akademicka 100",
                RequiredTime = new DateTime(2015, 5, 30, 14, 0, 0),
                Status = OrderStatus.Taken,
                AssignedTaxi = taxi
            };

            TaxiCompanyContext.Orders.Add(alreadyTakenOrder);

            var order = new Order
            {
                Address = "Gliwice Akademicka 100",
                RequiredTime = new DateTime(2015, 5, 30, 14, 0, 0),
                Status = OrderStatus.Open
            };

            TaxiCompanyContext.SaveChanges();

            // act, assert

            Assert.That(() => TaxiCompanyService.AssignTaxiToOrder(taxi.Id, order.Id),
                Throws.TypeOf<TaxiHasOngoingOrderAlready>());
        }

        [Test]
        public void Fails_When_Order_Is_Already_Taken()
        {
            // arrange

            var taxi = new Taxi
            {
                LicensePlate = "S1TAXI1",

                Owner = new Driver
                {
                    FirstName = "Jan",
                    Surname = "Kowalski"
                },

                Fleet = new Fleet
                {
                    Name = "Taxi Corpo SP ZOO"
                }
            };

            TaxiCompanyContext.Taxis.Add(taxi);

            var order = new Order
            {
                Address = "Gliwice Akademicka 100",
                RequiredTime = new DateTime(2015, 5, 30, 14, 0, 0),
                Status = OrderStatus.Taken,
                
                AssignedTaxi = new Taxi
                {
                    LicensePlate = "S1TAXI2",

                    Owner = new Driver
                    {
                        FirstName = "Mateusz",
                        Surname = "Nowak"
                    },

                    Fleet = new Fleet
                    {
                        Name = "Taxi Korpo SP ZOO"
                    }
                }
            };

            TaxiCompanyContext.Orders.Add(order);

            TaxiCompanyContext.SaveChanges();

            // act, assert

            Assert.That(() => TaxiCompanyService.AssignTaxiToOrder(taxi.Id, order.Id),
                Throws.TypeOf<OrderAlreadyTakenException>());
        }

        [TearDown]
        public void TearDown()
        {
            TaxiCompanyContext.Dispose();
        }

        private void ResetContext()
        {
            TaxiCompanyContext.Dispose();

            TaxiCompanyContext = new TaxiCompanyContext();
        }
    }
}