using BuilderSample.Builders;
using BuilderSample.Model;
using NUnit.Framework;

namespace BuilderSample.Tests
{
    [TestFixture]
    public class TaxiCompanyTestsWithBuilders
    {
        public TaxiCompanyContext TaxiCompanyContext;

        public TaxiCompanyService TaxiCompanyService;

        [SetUp] 
        public void SetUp()
        {
            TaxiCompanyContext = new TaxiCompanyContext();

            TaxiCompanyContext.Database.Delete();
            TaxiCompanyContext.Database.Create();

            TaxiCompanyService = new TaxiCompanyService(new TaxiCompanyContext());
        }

        [Test]
        public void Can_Assign_Taxi_To_A_New_Order()
        {
            // arrange

            var taxi = new TaxiBuilder(TaxiCompanyContext)
                .BuildAndSave();

            var order = new OrderBuilder(TaxiCompanyContext)
                .WithStatus(OrderStatus.Open)
                .BuildAndSave();

            // act

            TaxiCompanyService.AssignTaxiToOrder(taxi.Id, order.Id);

            ResetContext();

            // assert

            var chengedOrder = TaxiCompanyContext.Orders.Find(order.Id);

            Assert.That(chengedOrder.Status, Is.EqualTo(OrderStatus.Taken));
            Assert.That(chengedOrder.AssignedTaxi, Is.Not.Null);
            Assert.That(chengedOrder.AssignedTaxi.Id, Is.EqualTo(taxi.Id));
        }

        [Test]
        public void Assign_Taxi_Fails_When_Taxi_Is_Already_Assigned_To_Another_Order()
        {
            // arrange

            var taxi = new TaxiBuilder(TaxiCompanyContext)
                .BuildAndSave();

            var alreadyTakenOrder = new OrderBuilder(TaxiCompanyContext)
                .WithAssignedTaxi(taxi)
                .WithStatus(OrderStatus.Taken)
                .BuildAndSave();

            var order = new OrderBuilder(TaxiCompanyContext)
                .WithStatus(OrderStatus.Open)
                .BuildAndSave();

            // act, assert

            Assert.That(() => TaxiCompanyService.AssignTaxiToOrder(taxi.Id, order.Id),
                Throws.TypeOf<TaxiHasOngoingOrderAlreadyException>());
        }

        [Test]
        public void Fails_When_Order_Is_Already_Taken()
        {
            // arrange

            var taxi = new TaxiBuilder(TaxiCompanyContext)
                .BuildAndSave();

            var order = new OrderBuilder(TaxiCompanyContext)
                .WithSomeAssignedTaxi()
                .WithStatus(OrderStatus.Taken)
                .BuildAndSave();

            // act, assert

            Assert.That(() => TaxiCompanyService.AssignTaxiToOrder(taxi.Id, order.Id),
                Throws.TypeOf<OrderAlreadyTakenException>());
        }

        [TearDown]
        public void TearDown()
        {
            TaxiCompanyContext.Dispose();
        }

        private void ResetContext()
        {
            TaxiCompanyContext.Dispose();

            TaxiCompanyContext = new TaxiCompanyContext();
        }
    }
}