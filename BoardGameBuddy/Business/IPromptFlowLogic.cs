using BoardGameBuddy.API;
using Microsoft.AspNetCore.Mvc;
using static BoardGameBuddy.API.GeneratedDTOs;

namespace BoardGameBuddy.Business
{
    public interface IPromptFlowLogic
    {
        Task<string?> GenerateHangmanWord();
        Task<HangmanGameData> StartHangmanGame(string word);
    }
}
