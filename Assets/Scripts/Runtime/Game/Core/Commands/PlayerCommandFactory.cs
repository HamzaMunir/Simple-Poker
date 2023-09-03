namespace SimplePoker.Core.Commands
{
    public class PlayerCommandFactory
    {
        //Factory method
        public IPlayerCommand GetCommand(EHandCommandType commandOption)
        {
            IPlayerCommand command = default;
            switch (commandOption)
            {
                case EHandCommandType.Fold:
                    command = new FoldCommand();
                    break;
                case EHandCommandType.Call:
                    command = new CallCommand();
                    break;
                case EHandCommandType.Raise:
                    command = new RaiseCommand();
                    break;
                case EHandCommandType.Check:
                    command = new CheckCommand();
                    break;
                case EHandCommandType.Bet:
                    command = new BetCommand();
                    break;
            }

            return command;
        }
    }
}