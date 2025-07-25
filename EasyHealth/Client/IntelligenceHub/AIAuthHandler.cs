﻿using EasyHealth.API;
using System.Net.Http.Headers;
using Polly;
using Polly.Retry;

namespace EasyHealth.Client.IntelligenceHub
{
    public class AIAuthHandler : DelegatingHandler
    {
        private readonly AIAuthClient _authClient;
        private AuthTokenResponse? _token;
        private DateTime _tokenExpiry;
        private readonly AsyncRetryPolicy<System.Net.Http.HttpResponseMessage> _retryPolicy;

        public AIAuthHandler(AIAuthClient authClient, HttpMessageHandler innerHandler) : base(innerHandler)
        {
            _authClient = authClient;

            // could pass this into the controller using dependency injection, if a global policy is desired
            _retryPolicy = Policy
                .HandleResult<System.Net.Http.HttpResponseMessage>(r => (int)r.StatusCode >= 500)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        protected override async Task<System.Net.Http.HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Ensure token is valid before the request
            if (_token == null || DateTime.UtcNow.AddMinutes(1) > _tokenExpiry)
            {
                _token = await _authClient.RequestElevatedAuthToken();
                if (_token == null)
                {
                    throw new InvalidOperationException("Failed to retrieve access token.");
                }
                _tokenExpiry = DateTime.UtcNow.AddSeconds(_token.ExpiresIn);
            }

            // Attach token to the Authorization header
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.AccessToken);

            // Execute the request with retry policy
            return await _retryPolicy.ExecuteAsync(() => base.SendAsync(request, cancellationToken));
        }
    }
}
