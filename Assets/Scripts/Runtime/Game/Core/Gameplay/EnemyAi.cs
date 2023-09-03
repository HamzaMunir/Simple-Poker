using System;

namespace SimplePoker.Core
{
    /// <summary>
    /// This class is responsible for providing a decision to NPC for its turn
    /// </summary>
    public class EnemyAi
    {
        private const int FOLD_WEIGHT = 10;
        private const int CALL_WEIGHT = 25;
        private const int RAISE_WEIGHT = 35;
        private System.Random _random;

        private EHandCommandType[] _choices;

        public EnemyAi()
        {
            _random = new System.Random();
        }

        public EHandCommandType GetADecision(int cardsWeight, int moneyInBank, int moneySpent)
        {
            EHandCommandType decisionType;

            //Initial decision to be made on basis of two cards dealt to the player
            if (cardsWeight < FOLD_WEIGHT)
            {
                _choices = new[] { EHandCommandType.Fold, EHandCommandType.Call };
            }
            else if (cardsWeight <= CALL_WEIGHT)
            {
                _choices = new[] { EHandCommandType.Call, EHandCommandType.Call, EHandCommandType.Raise };
            }
            else if (cardsWeight <= RAISE_WEIGHT)
            {
                _choices = new[] { EHandCommandType.Call, EHandCommandType.Raise, EHandCommandType.Raise };
            }
            else
            {
                _choices = new[] { EHandCommandType.Call, EHandCommandType.Raise };
            }
            
            int nextRandom = _random.Next(0, _choices.Length);
            decisionType = _choices[nextRandom];
            //Check if it's the initial round and no money is left then fold otherwise if initial decision was
            //not to fold, either call or raise
            if (GameManager.Instance.GameplayManager.GameRound == EGameRound.Initial)
            {
                if (moneyInBank - moneySpent < GameManager.Instance.GameplayManager.HighestAmount)
                {
                    decisionType = EHandCommandType.Fold;
                }
                else if (GameManager.Instance.GameplayManager.HasSomeoneRaised && decisionType != EHandCommandType.Fold)
                {
                    _choices = new[] { EHandCommandType.Call, EHandCommandType.Call, EHandCommandType.Call, EHandCommandType.Raise };
                    nextRandom = _random.Next(0, _choices.Length);
                    decisionType = _choices[nextRandom];
                }
                
                return decisionType;
            }

            //In flop, turn, river if someone has already bet you can only fold or bet
            bool hasSomeoneBet = GameManager.Instance.GameplayManager.HasSomeoneBet;
            if (hasSomeoneBet)
            {
                if (moneyInBank - moneySpent < GameManager.Instance.GameplayManager.HighestAmount)
                {
                    decisionType = EHandCommandType.Fold;
                    return decisionType;
                }
                else
                {
                    _choices = new[] { EHandCommandType.Fold, EHandCommandType.Bet, EHandCommandType.Bet };
                    nextRandom = _random.Next(0, _choices.Length);
                    decisionType = _choices[nextRandom];
                    return decisionType;
                }
            }
            
            //Check the money, if can't bet anymore be sure to check to continue playing
            //otherwise either bet or check
            if (moneyInBank - moneySpent < GameManager.Instance.GameplayManager.HighestAmount)
            {
                _choices = new[]
                    {
                        EHandCommandType.Check
                    };
            }
            else
            {
                _choices = new[] { EHandCommandType.Check, EHandCommandType.Bet, EHandCommandType.Check, EHandCommandType.Bet, EHandCommandType.Check};
            }

            nextRandom = _random.Next(0, _choices.Length);
            decisionType = _choices[nextRandom];
            return decisionType;
        }
    }
}