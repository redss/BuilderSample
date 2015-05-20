using System;
using BuilderSample.Builders;
using BuilderSample.Model;
using NUnit.Framework;

namespace BuilderSample.Tests
{
    [TestFixture]
    public class TaxiCompanyTestsWithBuilders
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

            var taxi = new TaxiBuilder(Fixture.Context)
                .BuildAndSave();

            var order = new OrderBuilder(Fixture.Context)
                .WithStatus(OrderStatus.New)
                .BuildAndSave();

            // act

            Fixture.TaxiCompanyService.AssignTaxiToOrder(taxi.Id, order.Id);

            Fixture.ResetContext();

            // assert

            var changedOrder = Fixture.Context.Orders.Find(order.Id);

            Assert.That(changedOrder.Status, Is.EqualTo(OrderStatus.Ongoing));
            Assert.That(changedOrder.AssignedTaxi, Is.Not.Null);
            Assert.That(changedOrder.AssignedTaxi.Id, Is.EqualTo(taxi.Id));
        }

        [Test]
        public void Assign_Taxi_Fails_When_Taxi_Is_Already_Assigned_To_Another_Order()
        {
            // arrange

            var taxi = new TaxiBuilder(Fixture.Context)
                .BuildAndSave();

            var alreadyTakenOrder = new OrderBuilder(Fixture.Context)
                .WithAssignedTaxi(taxi)
                .WithStatus(OrderStatus.Ongoing)
                .BuildAndSave();

            var order = new OrderBuilder(Fixture.Context)
                .WithStatus(OrderStatus.New)
                .BuildAndSave();

            // act, assert

            Assert.That(() => Fixture.TaxiCompanyService.AssignTaxiToOrder(taxi.Id, order.Id),
                Throws.TypeOf<TaxiHasOngoingOrderException>());
        }

        [Test]
        public void Assign_Taxi_Fails_When_Order_Is_Already_Taken()
        {
            // arrange

            var taxi = new TaxiBuilder(Fixture.Context)
                .BuildAndSave();

            var order = new OrderBuilder(Fixture.Context)
                .WithSomeAssignedTaxi()
                .WithStatus(OrderStatus.Ongoing)
                .BuildAndSave();

            // act, assert

            Assert.That(() => Fixture.TaxiCompanyService.AssignTaxiToOrder(taxi.Id, order.Id),
                Throws.TypeOf<OrderAlreadyTakenException>());
        }

        [Test]
        public void Assign_Taxi_Fails_When_Order_Is_Completed()
        {
            // arrange

            var taxi = new TaxiBuilder(Fixture.Context)
                .BuildAndSave();

            var order = new OrderBuilder(Fixture.Context)
                .WithSomeAssignedTaxi()
                .WithStatus(OrderStatus.Completed)
                .BuildAndSave();

            // act, assert

            Assert.That(() => Fixture.TaxiCompanyService.AssignTaxiToOrder(taxi.Id, order.Id),
                Throws.TypeOf<OrderAlreadyCompletedException>());
        }
    }
}