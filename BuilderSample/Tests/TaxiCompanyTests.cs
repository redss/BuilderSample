using System;
using BuilderSample.Model;
using FluentAssertions;
using NUnit.Framework;

// ReSharper disable PossibleNullReferenceException

namespace BuilderSample.Tests
{
    [TestFixture]
    public class TaxiCompanyTests
    {
        public class TaxiCompanyFixture : IDisposable
        {
            public TaxiCompanyContext Context = new TaxiCompanyContext();
            public TaxiCompanyService TaxiCompanyService = new TaxiCompanyService();

            public TaxiCompanyFixture()
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

        public TaxiCompanyFixture Fixture;

        [SetUp]
        public void set_up()
        {
            Fixture = new TaxiCompanyFixture();
        }

        [TearDown]
        public void tear_down()
        {
            Fixture.Dispose();
        }

        [Test]
        public void can_send_taxi_to_a_new_order()
        {
            // given

            var taxi = new Taxi
            {
                LicensePlate = "S1TAXI",

                Owner = new Driver
                {
                    FirstName = "Jan",
                    LastName = "Nowak"
                },

                Corporation = new Corporation
                {
                    Name = "Taxi Corpo SP ZOO"
                }
            };

            Fixture.Context.Taxis.Add(taxi);

            var order = new Order
            {
                Address = "Gliwice Akademicka 100",
                RequiredTime = new DateTime(2015, 5, 30, 14, 0, 0),
                Status = OrderStatus.New
            };

            Fixture.Context.Orders.Add(order);

            Fixture.Context.SaveChanges();

            // act

            Fixture.TaxiCompanyService.SendTaxi(taxi.Id, order.Id);

            Fixture.ResetContext();

            // assert

            var changedOrder = Fixture.Context.Orders.Find(order.Id);

            changedOrder.Should().NotBeNull();
            changedOrder.Status.Should().Be(OrderStatus.Ongoing);
            changedOrder.AssignedTaxi.Should().NotBeNull();
            changedOrder.AssignedTaxi.Id.Should().Be(taxi.Id);
        }

        [Test]
        public void assign_taxi_fails_when_taxi_is_already_assigned_to_another_order()
        {
            // given

            var taxi = new Taxi
            {
                LicensePlate = "S1TAXI",

                Owner = new Driver
                {
                    FirstName = "Jan",
                    LastName = "Kowalski"
                },

                Corporation = new Corporation
                {
                    Name = "Taxi Corpo SP ZOO"
                }
            };

            Fixture.Context.Taxis.Add(taxi);

            var alreadyTakenOrder = new Order
            {
                Address = "Gliwice Akademicka 100",
                RequiredTime = new DateTime(2015, 5, 30, 14, 0, 0),
                Status = OrderStatus.Ongoing,
                AssignedTaxi = taxi
            };

            Fixture.Context.Orders.Add(alreadyTakenOrder);

            var order = new Order
            {
                Address = "Gliwice Akademicka 100",
                RequiredTime = new DateTime(2015, 5, 30, 14, 0, 0),
                Status = OrderStatus.New
            };

            Fixture.Context.SaveChanges();

            // when

            Action sendingTaxi = () => Fixture.TaxiCompanyService.SendTaxi(taxi.Id, order.Id);

            // then

            sendingTaxi.ShouldThrowExactly<TaxiHasOngoingOrderException>();
        }

        [Test]
        public void assign_taxi_fails_when_order_is_already_taken()
        {
            // given

            var taxi = new Taxi
            {
                LicensePlate = "S1TAXI1",

                Owner = new Driver
                {
                    FirstName = "Jan",
                    LastName = "Kowalski"
                },

                Corporation = new Corporation
                {
                    Name = "Taxi Corpo SP ZOO"
                }
            };

            Fixture.Context.Taxis.Add(taxi);

            var order = new Order
            {
                Address = "Gliwice Akademicka 100",
                RequiredTime = new DateTime(2015, 5, 30, 14, 0, 0),
                Status = OrderStatus.Ongoing,
                
                AssignedTaxi = new Taxi
                {
                    LicensePlate = "S1TAXI2",

                    Owner = new Driver
                    {
                        FirstName = "Mateusz",
                        LastName = "Nowak"
                    },

                    Corporation = new Corporation
                    {
                        Name = "Taxi Korpo SP ZOO"
                    }
                }
            };

            Fixture.Context.Orders.Add(order);

            Fixture.Context.SaveChanges();

            // when

            Action sendingTaxi = () => Fixture.TaxiCompanyService.SendTaxi(taxi.Id, order.Id);

            // then

            sendingTaxi.ShouldThrowExactly<OrderAlreadyTakenException>();
        }

        [Test]
        public void assign_taxi_fails_when_order_is_completed()
        {
            // given

            var taxi = new Taxi
            {
                LicensePlate = "S1TAXI1",

                Owner = new Driver
                {
                    FirstName = "Jan",
                    LastName = "Kowalski"
                },

                Corporation = new Corporation
                {
                    Name = "Taxi Corpo SP ZOO"
                }
            };

            Fixture.Context.Taxis.Add(taxi);

            var order = new Order
            {
                Address = "Gliwice Akademicka 100",
                RequiredTime = new DateTime(2015, 5, 30, 14, 0, 0),
                Status = OrderStatus.Completed,

                AssignedTaxi = new Taxi
                {
                    LicensePlate = "S1TAXI2",

                    Owner = new Driver
                    {
                        FirstName = "Mateusz",
                        LastName = "Nowak"
                    },

                    Corporation = new Corporation
                    {
                        Name = "Taxi Korpo SP ZOO"
                    }
                }
            };

            Fixture.Context.Orders.Add(order);

            Fixture.Context.SaveChanges();

            // when

            Action sendingTaxi = () => Fixture.TaxiCompanyService.SendTaxi(taxi.Id, order.Id);

            // then

            sendingTaxi.ShouldThrowExactly<OrderAlreadyCompletedException>();
        }
    }
}