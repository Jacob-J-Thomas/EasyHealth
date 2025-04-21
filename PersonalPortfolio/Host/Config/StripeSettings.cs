namespace PersonalPortfolio.Host.Config
{
    public class StripeSettings
    {
        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }
        public string SignupWebhookKey { get; set; }
        public string CancelationWebhookKey { get; set; }
        public string MeterName { get; set; }
        public string MeterPriceKey { get; set; }
        public string Domain { get; set; }
    }
}
