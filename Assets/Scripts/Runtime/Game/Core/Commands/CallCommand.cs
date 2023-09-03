using Game;
using SimplePoker.GameMessages;

namespace SimplePoker.Core.Commands
{
    public class CallCommand : IPlayerCommand
    {
        public EHandCommandType CommandType => EHandCommandType.Call;
        public void Execute(BasePlayer player)
        {
            player.PerformCallCommand();
            MessageBroker.Instance.Publish(new PlayerTurnCompleteMessage()
            {
                PlayerType = player.Type,
                Index = player.PositionAtTable,
                CommandType = CommandType
            });
        }
    }
}