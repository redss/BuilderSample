﻿using System;
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

            public void VerifyOrderWasTakenBy(int orderId, int taxiId)
            {
                var chengedOrder = Context.Orders.Find(orderId);

                Assert.That(chengedOrder.Status, Is.EqualTo(OrderStatus.Taken));
                Assert.That(chengedOrder.AssignedTaxi, Is.Not.Null);
                Assert.That(chengedOrder.AssignedTaxi.Id, Is.EqualTo(taxiId));
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
                .WithStatus(OrderStatus.Open)
                .BuildAndSave();

            // act

            Fixture.TaxiCompanyService.AssignTaxiToOrder(taxi.Id, order.Id);

            Fixture.ResetContext();

            // assert

            Fixture.VerifyOrderWasTakenBy(order.Id, taxi.Id);
        }
    }
}