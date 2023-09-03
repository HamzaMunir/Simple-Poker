using SimplePoker.Core;
using SimplePoker.Views;

namespace SimplePoker.UI.Data
{
    public class ResultUIData : UiBaseData
    {
        public EPlayerType Type;
        public bool HasEveryoneFolded;
        public string Name;
        public int PotAmount;
        public PokerHand WinningHand;
    }
}