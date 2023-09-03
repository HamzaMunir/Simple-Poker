using Game;
using SimplePoker.GameMessages;

namespace SimplePoker.Core.Commands
{
    public class FoldCommand : IPlayerCommand
    {
        public EHandCommandType CommandType => EHandCommandType.Fold;

        public void Execute(BasePlayer player)
        {
            player.PerformFoldCommand();
            MessageBroker.Instance.Publish(new PlayerTurnCompleteMessage()
            {
                PlayerType = player.Type,
                Index = player.PositionAtTable,
                CommandType = CommandType
            });
        }
    }
    
}