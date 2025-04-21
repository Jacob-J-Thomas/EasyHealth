namespace PersonalPortfolio.API
{
    public class PublicApiResponse
    {
        public string? Data { get; set; }
        public string? ErrorMesssage { get; set; }
        public bool Success { get; set; } = true;
        public int RemainingFreeRequests { get; set; }
    }
}
