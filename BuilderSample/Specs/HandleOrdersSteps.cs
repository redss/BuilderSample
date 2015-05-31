using System.Data.Entity;
using System.Linq;
using BuilderSample.Builders;
using BuilderSample.Model;
using BuilderSample.Setup;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace BuilderSample.Specs
{
    [Binding]
    public class HandleOrdersSteps
    {
        public DatabaseFixture Fixture;
        public OrderHandlingPage OrderHandlingPage;

        [BeforeScenario]
        public void BeforeScenario()
        {
            Fixture = new DatabaseFixture();
            OrderHandlingPage = new OrderHandlingPage(Fixture.Context, new TaxiCompanyService());
        }

        [AfterScenario]
        public void AfterScenario()
        {
            Fixture.Dispose();
        }

        public int HandledOrderId;

        [Given(@"There is a taxi ([^ ]*)")]
        public void ThereIsATaxi(string licensePlate)
        {
            new TaxiBuilder(Fixture.Context)
                .WithLicensePlate(licensePlate)
                .BuildAndSave();
        }

        [Given(@"There is a taxi (.*) assigned to some ongoing order")]
        public void ThereIsATaxiAssignedToSomeOngoingOrder(string licensePlate)
        {
            var taxi = new TaxiBuilder(Fixture.Context)
                .WithLicensePlate(licensePlate)
                .BuildAndSave();

            new OrderBuilder(Fixture.Context)
                .WithStatus(OrderStatus.Ongoing)
                .WithAssignedTaxi(taxi)
                .BuildAndSave();
        }
        
        [Given(@"There is a new order")]
        public void GivenThereIsANewOrder()
        {
            var order = new OrderBuilder(Fixture.Context)
                .BuildAndSave();

            HandledOrderId = order.Id;
        }

        [Given(@"There is an already taken order")]
        public void GivenThereIsAnAlreadyTakenOrder()
        {
            var order = new OrderBuilder(Fixture.Context)
                .WithStatus(OrderStatus.Ongoing)
                .WithSomeAssignedTaxi()
                .BuildAndSave();

            HandledOrderId = order.Id;
        }

        [Given(@"There is already completed order")]
        public void GivenThereIsAnAlreadyCompletedOrder()
        {
            var order = new OrderBuilder(Fixture.Context)
                .WithStatus(OrderStatus.Completed)
                .WithSomeAssignedTaxi()
                .BuildAndSave();

            HandledOrderId = order.Id;
        }
        
        [When(@"I send taxi (.*) to an order")]
        public void WhenISendTaxiToAnOrder(string licensePlate)
        {
            OrderHandlingPage
                .SelectTaxi(licensePlate)
                .SelectOrder(HandledOrderId.ToString())
                .SendTaxi();

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

        [Then(@"error message should be displayed")]
        public void ErrorMessageShouldBeDisplayed()
        {
            Assert.That(OrderHandlingPage.ErrorMessageIsDisplayed(), "Error message was not displayed.");
        }
    }
}
