﻿using System;
using BuilderSample.Builders;
using BuilderSample.Model;
using FluentAssertions;
using NUnit.Framework;

// ReSharper disable PossibleNullReferenceException

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

                changedOrder.Should().NotBeNull();
                changedOrder.Status.Should().Be(OrderStatus.Ongoing);
                changedOrder.AssignedTaxi.Should().NotBeNull();
                changedOrder.AssignedTaxi.Id.Should().Be(taxiId);
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

            var taxi = new TaxiBuilder(Fixture.Context)
                .BuildAndSave();

            var order = new OrderBuilder(Fixture.Context)
                .WithStatus(OrderStatus.New)
                .BuildAndSave();

            // when

            Fixture.TaxiCompanyService.SendTaxi(taxi.Id, order.Id);

            Fixture.ResetContext();

            // then

            Fixture.VerifyOrderWasTakenBy(order.Id, taxi.Id);
        }
    }
}