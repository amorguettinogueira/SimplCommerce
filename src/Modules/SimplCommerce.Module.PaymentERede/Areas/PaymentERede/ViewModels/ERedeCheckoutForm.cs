namespace SimplCommerce.Module.PaymentERede.Areas.PaymentERede.ViewModels
{
    public class ERedeCheckoutForm
    {
        public string ClientToken { get; set; }

        public decimal Amount { get; set; }

        public string ISOCurrencyCode { get; set; }
    }
}
