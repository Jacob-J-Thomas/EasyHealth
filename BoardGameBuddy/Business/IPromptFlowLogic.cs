using BoardGameBuddy.API;

namespace BoardGameBuddy.Business
{
    public interface IPromptFlowLogic
    {
        Task<string?> GenerateHangmanWord();
        Task<HangmanGameData> StartHangmanGame(string word);
    }
}
