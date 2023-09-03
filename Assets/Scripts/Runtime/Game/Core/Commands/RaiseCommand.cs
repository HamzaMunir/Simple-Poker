using Game;
using SimplePoker.GameMessages;

namespace SimplePoker.Core.Commands
{
    public class RaiseCommand : IPlayerCommand
    {
        public EHandCommandType CommandType => EHandCommandType.Raise;
        public void Execute(BasePlayer player)
        {
            player.PerformRaiseCommand();
            MessageBroker.Instance.Publish(new PlayerTurnCompleteMessage()
            {
                PlayerType = player.Type,
                Index = player.PositionAtTable,
                CommandType = CommandType
            });
        }
    }
}