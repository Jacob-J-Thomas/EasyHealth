namespace AIPoweredSQLConverter.Host.Config
{
    public class AuthSettings
    {
        public string BasicAuthUsername { get; set; }
        public string BasicAuthPassword { get; set; }
        public string Audience { get; internal set; }
        public string Domain { get; internal set; }
    }
}
