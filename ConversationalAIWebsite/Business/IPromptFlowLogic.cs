using AiPocWebsiteTemplateWithBackend.API;
using Microsoft.AspNetCore.Mvc;
using static AiPocWebsiteTemplateWithBackend.API.GeneratedDTOs;

namespace AiPocWebsiteTemplateWithBackend.Business
{
    public interface IPromptFlowLogic
    {
        Task<string?> GenerateHangmanWord();
        Task<HangmanGameData> StartHangmanGame(string word);
    }
}
