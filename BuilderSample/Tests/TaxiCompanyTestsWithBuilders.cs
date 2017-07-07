using System;
using BuilderSample.Builders;
using BuilderSample.Model;
using FluentAssertions;
using NUnit.Framework;

// ReSharper disable PossibleNullReferenceException
// ReSharper disable UnusedVariable

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

            var taxi = new TaxiBuilder(Fixture.Context)
                .BuildAndSave();

            var alreadyTakenOrder = new OrderBuilder(Fixture.Context)
                .WithAssignedTaxi(taxi)
                .WithStatus(OrderStatus.Ongoing)
                .BuildAndSave();

            var order = new OrderBuilder(Fixture.Context)
                .WithStatus(OrderStatus.New)
                .BuildAndSave();

            // when

            Action sendingTaxi = () => Fixture.TaxiCompanyService.SendTaxi(taxi.Id, order.Id);

            sendingTaxi.ShouldThrowExactly<TaxiHasOngoingOrderException>();
        }

        [Test]
        public void assign_taxi_fails_when_order_is_already_taken()
        {
            // given

            var taxi = new TaxiBuilder(Fixture.Context)
                .BuildAndSave();

            var order = new OrderBuilder(Fixture.Context)
                .WithSomeAssignedTaxi()
                .WithStatus(OrderStatus.Ongoing)
                .BuildAndSave();

            // when

            Action sendingTaxi = () => Fixture.TaxiCompanyService.SendTaxi(taxi.Id, order.Id);

            // then

            sendingTaxi.ShouldThrowExactly<OrderAlreadyTakenException>();
        }

        [Test]
        public void assign_taxi_fails_when_order_is_completed()
        {
            // given

            var taxi = new TaxiBuilder(Fixture.Context)
                .BuildAndSave();

            var order = new OrderBuilder(Fixture.Context)
                .WithSomeAssignedTaxi()
                .WithStatus(OrderStatus.Completed)
                .BuildAndSave();

            // when

            Action sendingTaxi = () => Fixture.TaxiCompanyService.SendTaxi(taxi.Id, order.Id);

            // then

            sendingTaxi.ShouldThrowExactly<OrderAlreadyCompletedException>();
        }
    }
}