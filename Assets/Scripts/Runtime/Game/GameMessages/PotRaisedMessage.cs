using SimplePoker.Core;

namespace SimplePoker.GameMessages
{
    public class PotRaisedMessage
    {
        public int Amount;
        public int HighestBet;
        public EHandCommandType CommandType;
    }
}