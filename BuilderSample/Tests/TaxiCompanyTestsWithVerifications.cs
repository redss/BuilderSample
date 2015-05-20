using System;
using BuilderSample.Builders;
using BuilderSample.Model;
using NUnit.Framework;

namespace BuilderSample.Tests
{
    [TestFixture]
    public class TaxiCompanyTestsWithVerifications
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

            public void VerifyOrderWasTakenBy(int orderId, int taxiId)
            {
                var changedOrder = Context.Orders.Find(orderId);

                Assert.That(changedOrder.Status, Is.EqualTo(OrderStatus.Ongoing));
                Assert.That(changedOrder.AssignedTaxi, Is.Not.Null);
                Assert.That(changedOrder.AssignedTaxi.Id, Is.EqualTo(taxiId));
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

            Fixture.VerifyOrderWasTakenBy(order.Id, taxi.Id);
        }
    }
}