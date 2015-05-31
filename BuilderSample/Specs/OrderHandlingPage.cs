using System;
using System.Linq;

namespace BuilderSample.Specs
{
    public class OrderHandlingPage
    {
        private readonly TaxiCompanyContext _taxiCompanyContext;
        private readonly TaxiCompanyService _taxiCompanyService;

        public OrderHandlingPage(TaxiCompanyContext taxiCompanyContext, TaxiCompanyService taxiCompanyService)
        {
            _taxiCompanyContext = taxiCompanyContext;
            _taxiCompanyService = taxiCompanyService;
        }

        private string _taxiLicensePlate;
        private string _orderId;
        private string _errorMessage;

        public OrderHandlingPage SelectTaxi(string licensePlate)
        {
            _taxiLicensePlate = licensePlate;

            return this;
        }

        public OrderHandlingPage SelectOrder(string orderId)
        {
            _orderId = orderId;

            return this;
        }

        public OrderHandlingPage SendTaxi()
        {
            try
            {
                var taxi = _taxiCompanyContext.Taxis
                    .Single(t => t.LicensePlate == _taxiLicensePlate);

                var orderId = int.Parse(_orderId);

                _taxiCompanyService.SendTaxi(taxi.Id, orderId);
            }
            catch (Exception e)
            {
                _errorMessage = e.Message;
            }

            return this;
        }

        public bool ErrorMessageIsDisplayed()
        {
            return _errorMessage != null;
        }
    }
}