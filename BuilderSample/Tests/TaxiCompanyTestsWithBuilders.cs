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
            private TaxiCompanyContext _serviceContext;

            public TaxiCompanyContext Context;

            public TaxiCompanyService TaxiCompanyService;

            public TaxiCompanyFixture()
            {
                Context = new TaxiCompanyContext();

                Context.Database.Delete();
                Context.Database.Create();

                _serviceContext = new TaxiCompanyContext();

                TaxiCompanyService = new TaxiCompanyService(_serviceContext);
            }

            public void ResetContext()
            {
                Context.Dispose();

                Context = new TaxiCompanyContext();
            }

            public void Dispose()
            {
                Context.Dispose();

                _serviceContext.Dispose();
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
                .WithStatus(OrderStatus.Open)
                .BuildAndSave();

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

            var taxi = new TaxiBuilder(Fixture.Context)
                .BuildAndSave();

            var alreadyTakenOrder = new OrderBuilder(Fixture.Context)
                .WithAssignedTaxi(taxi)
                .WithStatus(OrderStatus.Taken)
                .BuildAndSave();

            var order = new OrderBuilder(Fixture.Context)
                .WithStatus(OrderStatus.Open)
                .BuildAndSave();

            // act, assert

            Assert.That(() => Fixture.TaxiCompanyService.AssignTaxiToOrder(taxi.Id, order.Id),
                Throws.TypeOf<TaxiHasOngoingOrderAlreadyException>());
        }

        [Test]
        public void Fails_When_Order_Is_Already_Taken()
        {
            // arrange

            var taxi = new TaxiBuilder(Fixture.Context)
                .BuildAndSave();

            var order = new OrderBuilder(Fixture.Context)
                .WithSomeAssignedTaxi()
                .WithStatus(OrderStatus.Taken)
                .BuildAndSave();

            // act, assert

            Assert.That(() => Fixture.TaxiCompanyService.AssignTaxiToOrder(taxi.Id, order.Id),
                Throws.TypeOf<OrderAlreadyTakenException>());
        }
    }
}