using BuilderSample.Builders;
using BuilderSample.Model;
using BuilderSample.Setup;
using BuilderSample.Specs;
using NUnit.Framework;

namespace BuilderSample.Tests
{
    [TestFixture]
    public class TaxiCompanyServiceTests
    {
        public class TaxiCompanyFixture : DatabaseFixture
        {
            public TaxiCompanyService TaxiCompanyService = new TaxiCompanyService();

            public void VerifyOrderWasTakenBy(int orderId, int taxiId)
            {
                var changedOrder = Context.Orders.Find(orderId);

                Assert.That(changedOrder.Status, Is.EqualTo(OrderStatus.Ongoing));
                Assert.That(changedOrder.AssignedTaxi, Is.Not.Null);
                Assert.That(changedOrder.AssignedTaxi.Id, Is.EqualTo(taxiId));
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
        public void Can_Send_Taxi_To_A_New_Order()
        {
            // arrange

            var taxi = new TaxiBuilder(Fixture.Context)
                .BuildAndSave();

            var order = new OrderBuilder(Fixture.Context)
                .WithStatus(OrderStatus.New)
                .BuildAndSave();

            // act

            Fixture.TaxiCompanyService.SendTaxi(taxi.Id, order.Id);

            Fixture.ResetContext();

            // assert

            Fixture.VerifyOrderWasTakenBy(order.Id, taxi.Id);
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

            Assert.That(() => Fixture.TaxiCompanyService.SendTaxi(taxi.Id, order.Id),
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

            Assert.That(() => Fixture.TaxiCompanyService.SendTaxi(taxi.Id, order.Id),
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

            Assert.That(() => Fixture.TaxiCompanyService.SendTaxi(taxi.Id, order.Id),
                Throws.TypeOf<OrderAlreadyCompletedException>());
        }
    }
}