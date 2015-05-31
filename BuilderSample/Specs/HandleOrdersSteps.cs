using System.Data.Entity;
using System.Linq;
using BuilderSample.Builders;
using BuilderSample.Model;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace BuilderSample.Specs
{
    [Binding]
    public class HandleOrdersSteps
    {
        public DatabaseFixture Fixture;

        [BeforeScenario]
        public void BeforeScenario()
        {
            Fixture = new DatabaseFixture();
        }

        [AfterScenario]
        public void AfterScenario()
        {
            Fixture.Dispose();
        }

        public int HandledOrderId;

        [Given(@"There is a taxi (.*)")]
        public void GivenThereIsATaxi(string licensePlate)
        {
            new TaxiBuilder(Fixture.Context)
                .WithLicensePlate(licensePlate)
                .BuildAndSave();
        }
        
        [Given(@"There is a new order")]
        public void GivenThereIsANewOrder()
        {
            var order = new OrderBuilder(Fixture.Context)
                .BuildAndSave();

            HandledOrderId = order.Id;
        }
        
        [When(@"I send taxi (.*) to an order")]
        public void WhenISendTaxiToAnOrder(string licensePlate)
        {
            var taxi = Fixture.Context.Taxis
                .Single(t => t.LicensePlate == licensePlate);

            var sut = new TaxiCompanyService();

            sut.SendTaxi(taxi.Id, HandledOrderId);

            Fixture.ResetContext();
        }

        [Then(@"order should be assigned to taxi (.*)")]
        public void ThenTaxiShouldBeAssignedToAnOrder(string licensePlate)
        {
            var order = Fixture.Context.Orders
                .Where(o => o.Id == HandledOrderId)
                .Include(o => o.AssignedTaxi)
                .Single();

            Assert.That(order.AssignedTaxi.LicensePlate, Is.EqualTo(licensePlate));
        }
        
        [Then(@"order should be in progress")]
        public void ThenOrderShouldBeInProgress()
        {
            var order = Fixture.Context.Orders
                .Single(o => o.Id == HandledOrderId);

            Assert.That(order.Status, Is.EqualTo(OrderStatus.Ongoing));
        }
    }
}
