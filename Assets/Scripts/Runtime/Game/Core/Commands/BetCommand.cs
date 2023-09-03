using Game;
using SimplePoker.GameMessages;

namespace SimplePoker.Core.Commands
{
    public class BetCommand : IPlayerCommand
    {
        public EHandCommandType CommandType => EHandCommandType.Bet;
        public void Execute(BasePlayer player)
        {
            player.PerformBetCommand();
            MessageBroker.Instance.Publish(new PlayerTurnCompleteMessage()
            {
                PlayerType = player.Type,
                Index = player.PositionAtTable,
                CommandType = CommandType
            });
        }
    }
}