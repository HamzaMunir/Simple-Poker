using SimplePoker.Core;

namespace SimplePoker.GameMessages
{
    public class ExecutePlayerCommandMessage
    {
        public BasePlayer Player;
        public EHandCommandType CommandType;
    }
}