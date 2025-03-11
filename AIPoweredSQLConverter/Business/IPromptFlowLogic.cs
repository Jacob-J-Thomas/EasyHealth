using AIPoweredSQLConverter.API;

namespace AIPoweredSQLConverter.Business
{
    public interface IPromptFlowLogic
    {
        Task<string?> GenerateHangmanWord();
        Task<HangmanGameData> StartHangmanGame(string word);
    }
}
