using SimplePoker.Core;
using SimplePoker.Views;

namespace SimplePoker.UI.Data
{
    public class PlayerUIData : UiBaseData
    {
        public EGameRound GameRound;
        public bool HasSomeoneBet;
        public bool CanCall;
        public bool CanRaise;
        public bool CanBet;
    }
}