using System;
using BuilderSample.Model;
using NUnit.Framework;

namespace BuilderSample.Tests
{
    [TestFixture]
    public class TaxiCompanyTests
    {
        public class TaxiCompanyFixture : IDisposable
        {
            public TaxiCompanyContext Context;

            public TaxiCompanyService TaxiCompanyService;

            public TaxiCompanyFixture()
            {
                Context = new TaxiCompanyContext();

                Context.Database.Delete();
                Context.Database.Create();

                TaxiCompanyService = new TaxiCompanyService();
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

        public TaxiCompanyFixture Fixture;

        [SetUp] 
        public void SetUp()
        {
            Fixture = new TaxiCompanyFixture();
        }

        [TearDown]
        public void TearDown()
        {
            Fixture.Dispose();
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
                    Surname = "Nowak"
                },

                Fleet = new Fleet
                {
                    Name = "Taxi Corpo SP ZOO"
                }
            };

            Fixture.Context.Taxis.Add(taxi);

            var order = new Order
            {
                Address = "Gliwice Akademicka 100",
                RequiredTime = new DateTime(2015, 5, 30, 14, 0, 0),
                Status = OrderStatus.Open
            };

            Fixture.Context.Orders.Add(order);

            Fixture.Context.SaveChanges();

            // act

            Fixture.TaxiCompanyService.AssignTaxiToOrder(taxi.Id, order.Id);

            Fixture.ResetContext();

            // assert

            var chengedOrder = Fixture.Context.Orders.Find(order.Id);

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

            Fixture.Context.Taxis.Add(taxi);

            var alreadyTakenOrder = new Order
            {
                Address = "Gliwice Akademicka 100",
                RequiredTime = new DateTime(2015, 5, 30, 14, 0, 0),
                Status = OrderStatus.Taken,
                AssignedTaxi = taxi
            };

            Fixture.Context.Orders.Add(alreadyTakenOrder);

            var order = new Order
            {
                Address = "Gliwice Akademicka 100",
                RequiredTime = new DateTime(2015, 5, 30, 14, 0, 0),
                Status = OrderStatus.Open
            };

            Fixture.Context.SaveChanges();

            // act, assert

            Assert.That(() => Fixture.TaxiCompanyService.AssignTaxiToOrder(taxi.Id, order.Id),
                Throws.TypeOf<TaxiHasOngoingOrderAlreadyException>());
        }

        [Test]
        public void Assign_Taxi_Fails_When_Order_Is_Already_Taken()
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

            Fixture.Context.Taxis.Add(taxi);

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

            Fixture.Context.Orders.Add(order);

            Fixture.Context.SaveChanges();

            // act, assert

            Assert.That(() => Fixture.TaxiCompanyService.AssignTaxiToOrder(taxi.Id, order.Id),
                Throws.TypeOf<OrderAlreadyTakenException>());
        }

        [Test]
        public void Assign_Taxi_Fails_When_Order_Is_Completed()
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

            Fixture.Context.Taxis.Add(taxi);

            var order = new Order
            {
                Address = "Gliwice Akademicka 100",
                RequiredTime = new DateTime(2015, 5, 30, 14, 0, 0),
                Status = OrderStatus.Complete,

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

            Fixture.Context.Orders.Add(order);

            Fixture.Context.SaveChanges();

            // act, assert

            Assert.That(() => Fixture.TaxiCompanyService.AssignTaxiToOrder(taxi.Id, order.Id),
                Throws.TypeOf<OrderAlreadyCompletedException>());
        }
    }
}