using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using OpenAICustomFunctionCallingAPI.API.DTOs.Hub;
using System;
using System.Threading.Tasks;

namespace ConversationalAIWebsite.Hubs
{
    public class AIHub : Hub, IDisposable
    {
        private readonly HubConnection _chatHubConnection;
        public event Action<string, string> OnBroadcastMessageReceived;
        private bool _isInitialized = false;
        private string _currentAuthor;

        public AIHub()
        {
            _chatHubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:53337/chatstream")
                .Build();

            InitializeAsync().Wait();
        }

        public async Task SendMessageToAIAPI(ChatRequest chatRequest)
        {
            // an additional security measurement to ensure jailbreaking is harder (limits context window)
            var maxQueryLength = 280;
            if (chatRequest.Message.Length > maxQueryLength) return;

            try
            {
                var request = new APIChatRequest()
                {
                    ProfileName = "string",
                    ConversationId = chatRequest.ConversationId,
                    Username = chatRequest.Username,
                    Message = chatRequest.Message,
                    MaxMessageHistory = 5,
                    Database = "string",
                    RagTarget = "Content",
                    MaxRagDocs = 5,
                };
                if (_chatHubConnection.State == HubConnectionState.Connected) await _chatHubConnection.InvokeAsync("Send", request);
                else Console.WriteLine("AIHub connection is not established.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending message: {0}", ex.Message);
            }
        }

        public async Task ReturnMessageFromAIAPI(string action, string details)
        {
            try
            {
                if (_chatHubConnection.State == HubConnectionState.Connected) await _chatHubConnection.SendAsync("ReceiveMessage", action, details);
                else Console.WriteLine("ChatHub connection is not established.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending action details: {0}", ex.Message);
            }
        }

        private async Task InitializeAsync()
        {
            try
            {
                await _chatHubConnection.StartAsync();
                Console.WriteLine("Connected to AIHub.");
                if (!_isInitialized)
                {
                    _chatHubConnection.On<string, string>("broadcastMessage", (author, message) =>
                    {
                        if (author != null) _currentAuthor = author;
                        OnBroadcastMessageReceived?.Invoke(_currentAuthor, message);
                    });

                    OnBroadcastMessageReceived += async (author, message) =>
                    {
                        if (author != null) _currentAuthor = author;
                        if (_chatHubConnection.State == HubConnectionState.Connected) await Clients.All.SendAsync("broadcastMessage", _currentAuthor, message);
                        else Console.WriteLine("The AIHub connection is not established.");
                    };
                    _isInitialized = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an error with the connection: {0}", ex.Message);
            }
        }

        public void Dispose()
        {
            _chatHubConnection.DisposeAsync().AsTask().Wait();
        }
    }
}
