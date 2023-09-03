using Game;
using SimplePoker.GameMessages;

namespace SimplePoker.Core.Commands
{
    public class CheckCommand : IPlayerCommand
    {
        public EHandCommandType CommandType => EHandCommandType.Check;
        public void Execute(BasePlayer player)
        {
            player.PerformCheckCommand();
            MessageBroker.Instance.Publish(new PlayerTurnCompleteMessage()
            {
                PlayerType = player.Type,
                Index = player.PositionAtTable,
                CommandType = CommandType
            });
        }
    }
}