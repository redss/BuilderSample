using System;
using BuilderSample.Builders;
using BuilderSample.Model;
using FluentAssertions;
using NUnit.Framework;
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

            var changedOrder = Fixture.Context.Orders.Find(order.Id);

            changedOrder.Should().NotBeNull();
            changedOrder.Status.Should().Be(OrderStatus.Ongoing);
            changedOrder.AssignedTaxi.Should().NotBeNull();
            changedOrder.AssignedTaxi.Id.Should().Be(taxi.Id);
        }

        [Test]
        public void assign_taxi_fails_when_taxi_is_already_assigned_to_another_order()
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

            Action sendingTaxi = () => Fixture.TaxiCompanyService.SendTaxi(taxi.Id, order.Id);

            sendingTaxi.ShouldThrowExactly<TaxiHasOngoingOrderException>();
        }

        [Test]
        public void assign_taxi_fails_when_order_is_already_taken()
        {
            // arrange

            var taxi = new TaxiBuilder(Fixture.Context)
                .BuildAndSave();

            var order = new OrderBuilder(Fixture.Context)
                .WithSomeAssignedTaxi()
                .WithStatus(OrderStatus.Ongoing)
                .BuildAndSave();

            // act, assert

            Action sendingTaxi = () => Fixture.TaxiCompanyService.SendTaxi(taxi.Id, order.Id);

            sendingTaxi.ShouldThrowExactly<OrderAlreadyTakenException>();
        }

        [Test]
        public void assign_taxi_fails_when_order_is_completed()
        {
            // arrange

            var taxi = new TaxiBuilder(Fixture.Context)
                .BuildAndSave();

            var order = new OrderBuilder(Fixture.Context)
                .WithSomeAssignedTaxi()
                .WithStatus(OrderStatus.Completed)
                .BuildAndSave();

            // act, assert

            Action sendingTaxi = () => Fixture.TaxiCompanyService.SendTaxi(taxi.Id, order.Id);

            sendingTaxi.ShouldThrowExactly<OrderAlreadyCompletedException>();
        }
    }
}