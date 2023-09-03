using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks.Linq;
using Game;
using Kit;
using SimplePoker.GameMessages;
using SimplePoker.UI;
using SimplePoker.UI.Data;
using UnityEngine;
using SimplePoker.Core.General;

namespace SimplePoker.Core
{
    public enum EGameRole
    {
        Dealer = 0,
        SmallBlind = 1,
        BigBlind = 2,
        EarlyPostion = 3,
        MiddlePosition = 4,
        LatePosition = 5
    }

    public enum EGameRound
    {
        Initial,
        Flop,
        Turn,
        River,
        Result
    }

    public enum EPlayerType
    {
        Main,
        NPC
    }

    public enum EHandCommandType
    {
        None,
        Fold,
        Call,
        Raise,
        Bet,
        Check
    }

    public class GameplayManager : MonoBehaviour
    {
        #region PRIVATE Fields

        private Deck _deck;
        private List<Card> _communityCards = new();
        private long _potAmount = 0;
        private long _highestBet = 0;
        private Player _player;
        private List<EnemyPlayer> _otherPlayers = new();
        private List<IGamePlayer> _totalPlayers = new();
        private EGameRound _gameRound;
        private int _roundStartPlayerIndex = 0;
        private int _currentPlayerIndexInRound = 0;
        private int _dealerCoinIndex = 0;
        private CancellationTokenSource _cancellationToken = new();
        private TableManager _tableManager => GameManager.Instance.TableManager;
        private PokerHandChecker _pokerHandChecker = new();

        #endregion

        #region Public Fields

        public const int SEQUENCE_WEIGHT = 5;
        public const int SAME_SUIT_WEIGHT = 5;
        public const int PAIR_WEIGHT = 5;
        public bool HasSomeoneRaised = false;
        public bool HasSomeoneBet = false;
        public long PotAmount => _potAmount;
        public long HighestAmount => _highestBet;
        public EGameRound GameRound => _gameRound;

        #endregion

        #region MonoBehaviour Callbacks

        public void Start()
        {
            MessageBroker.Instance.Receive<PlayerTurnCompleteMessage>()
                .Subscribe(data => OnPlayerTurnCompleteReceived(data), _cancellationToken.Token);
            MessageBroker.Instance.Receive<PotRaisedMessage>()
                .Subscribe(data => OnPotRaisedMessage(data), _cancellationToken.Token);
            MessageBroker.Instance.Receive<NextGameMessage>()
                .Subscribe(data => ResetAndNextRoundGame(), _cancellationToken.Token);
        }

        #endregion


        #region Game Initialization

        public void InitGame(List<EnemyPlayer> otherPlayers, List<IGamePlayer> totalPlayers)
        {
            _deck = new Deck();
            _otherPlayers = otherPlayers;
            _totalPlayers = totalPlayers;
            _player = (Player)totalPlayers.Find(x => x.Type == EPlayerType.Main);
            _deck.ReadyDeck();
            AssignRoles();
            DelayBeforeCommand(RaiseBlinds);
            DelayBeforeCommand(DealCards);
            DelayBeforeCommand(StartGame);
        }

        private void AssignRoles()
        {
            _totalPlayers[_dealerCoinIndex].GameRole = EGameRole.Dealer;
            int roleAssignCount = _dealerCoinIndex;
            roleAssignCount++;
            var enumValues = Enum.GetValues(typeof(EGameRole));
            int enumIndex = 1;
            for (; roleAssignCount < _totalPlayers.Count; roleAssignCount++)
            {
                _totalPlayers[roleAssignCount].GameRole = (EGameRole)enumValues.GetValue(enumIndex);
                enumIndex++;
            }

            if (_dealerCoinIndex != 0)
            {
                for (int i = 0; i < _dealerCoinIndex; i++)
                {
                    if (enumIndex > enumValues.Length)
                    {
                        _totalPlayers[i].GameRole = EGameRole.LatePosition;
                    }
                    else
                    {
                        _totalPlayers[i].GameRole = (EGameRole)enumValues.GetValue(enumIndex);
                    }

                    enumIndex++;
                }
            }
        }

        private void RaiseBlinds()
        {
            var bigBlind = _totalPlayers.Find(x => x.GameRole == EGameRole.BigBlind);
            _potAmount += bigBlind.RaiseBlind();
            _highestBet = _potAmount;
            var smallBlind = _totalPlayers.Find(x => x.GameRole == EGameRole.SmallBlind);
            _potAmount += smallBlind.RaiseBlind();
        }

        private void DealCards()
        {
            for (int i = 0; i < 2; i++)
            {
                Card card;
                for (int j = 0; j < _totalPlayers.Count; j++)
                {
                    card = _deck.Deal();
                    var cardView = SpawnCardView(card, _totalPlayers[j]);
                    _totalPlayers[j].AddCardToPlayer(card, cardView);
                }
            }
        }

        private void StartGame()
        {
            _gameRound = EGameRound.Initial;
            var earlyPositionIndex = _totalPlayers.FindIndex(x => x.GameRole == EGameRole.EarlyPostion);
            _currentPlayerIndexInRound = earlyPositionIndex;
            _roundStartPlayerIndex = earlyPositionIndex;
            DelayBeforeExecutingTurns(earlyPositionIndex);
        }

        private void ResetAndNextRoundGame()
        {
            for (int i = 0; i < _totalPlayers.Count; i++)
            {
                _totalPlayers[i].Reset();
            }

            _deck.ReadyDeck();
            _highestBet = 0;
            _potAmount = 0;
            _tableManager.UpdateCommunityCards(new List<Card>());
            _tableManager.UpdatePotAmount(_potAmount);
            _communityCards.Clear();
            _dealerCoinIndex++;
            AssignRoles();
            DelayBeforeCommand(RaiseBlinds);
            DelayBeforeCommand(DealCards);
            DelayBeforeCommand(StartGame);
        }

        #endregion

        #region Gameplay

        private void ExecuteTurns(int currentTurnIndex)
        {
            if (_totalPlayers[currentTurnIndex].Type == EPlayerType.Main)
            {
                if (_totalPlayers[currentTurnIndex].CurrentState != EHandCommandType.Fold)
                {
                    ExecutePlayerTurn(currentTurnIndex);
                    return;
                }
            }

            ExecuteNPCTurn(currentTurnIndex);
        }

        /// <summary>
        /// This method is only here to simulate delay in decision making
        /// </summary>
        private async void DelayBeforeExecutingTurns(int currentIndex)
        {
            var player = _totalPlayers[currentIndex];
            player.UpdateIndicator(player.CurrentState != EHandCommandType.Fold);
            if (player.CurrentState != EHandCommandType.Fold)
            {
                ControlHelper.DelaySeconds(1, () => { ExecuteTurns(currentIndex); }, _cancellationToken.Token);
            }
            else
            {
                ExecuteTurns(currentIndex);
            }
        }

        private void DelayBeforeCommand(Action action)
        {
            CancellationTokenSource token = new CancellationTokenSource();
            ControlHelper.DelaySeconds(1, action, token.Token);
        }

        private void ExecuteNPCTurn(int currentTurnIndex)
        {
            _totalPlayers[currentTurnIndex].MakeDecision();
        }

        private void ExecutePlayerTurn(int currentIndex)
        {
            var player = _totalPlayers[currentIndex];
            if (player.CurrentState == EHandCommandType.Fold)
            {
                player.MakeDecision();
            }

            bool canCallRaiseOrBet = player.MoneyInBank - player.MoneySpent > _highestBet;
            GameManager.Instance.UiManager.ShowPlayerUI(new PlayerUIData()
            {
                GameRound = _gameRound,
                HasSomeoneBet = HasSomeoneBet,
                CanBet = canCallRaiseOrBet,
                CanCall = canCallRaiseOrBet,
                CanRaise = canCallRaiseOrBet
            });
        }

        private CardView SpawnCardView(Card card, IGamePlayer player)
        {
            var cardView =
                Instantiate(AssetManager.Instance.GetPrefab(nameof(CardView)), ((BasePlayer)player).PlayerSpot.CardRect)
                    .GetComponent<CardView>();
            cardView.InitView(card);
            return cardView;
        }

        private void GetNextRound()
        {
            switch (_gameRound)
            {
                case EGameRound.Initial:
                    GetFlop();
                    _tableManager.UpdatePotAmount(_potAmount);
                    break;
                case EGameRound.Flop:
                    GetTurn();
                    _tableManager.UpdatePotAmount(_potAmount);
                    break;
                case EGameRound.Turn:
                    GetRiver();
                    _tableManager.UpdatePotAmount(_potAmount);
                    break;
                case EGameRound.River:
                    _gameRound = EGameRound.Result;
                    break;
            }
        }

        private void CheckForGameWinner(List<IGamePlayer> players)
        {
            if (_gameRound == EGameRound.Initial && players.Count < 2)
            {
                GameManager.Instance.UiManager.ShowResultUI(new ResultUIData()
                {
                    HasEveryoneFolded = true
                });
                return;
            }

            Dictionary<IGamePlayer, PokerHand> pokerHands = new Dictionary<IGamePlayer, PokerHand>();
            for (int i = 0; i < players.Count; i++)
            {
                var pokerHand = _pokerHandChecker.DetectHands(players[i].GetCards, _communityCards);
                pokerHands.Add(players[i], pokerHand);
            }

            DeclareWinner(pokerHands);
        }

        private void DeclareWinner(Dictionary<IGamePlayer, PokerHand> pokerHands)
        {
            var sortedDict = pokerHands.OrderBy(x => x.Value.Hand).ToDictionary(x => x.Key, x => x.Value);
            PokerHand temp = new PokerHand() { Hand = EPokerHand.HIGH_CARD, Weight = 0 };
            IGamePlayer winner = default;
            if (sortedDict.First().Value.Hand == EPokerHand.HIGH_CARD)
            {
                int tempWeight = 0;
                foreach (var value in sortedDict)
                {
                    if (value.Value.Weight > tempWeight)
                    {
                        tempWeight = value.Value.Weight;
                        winner = value.Key;
                        temp = value.Value;
                    }
                }
            }
            else
            {
                foreach (var value in sortedDict)
                {
                    if (value.Value.Hand == temp.Hand)
                    {
                        if (value.Value.Weight > temp.Weight)
                        {
                            temp = value.Value;
                            winner = value.Key;
                        }
                    }
                    else if (value.Value.Hand > temp.Hand)
                    {
                        temp = value.Value;
                        winner = value.Key;
                    }
                }
            }

            winner?.UpdateWins((int)_potAmount);
            Debug.Log($"winner: {temp.Hand}");
            GameManager.Instance.UiManager.ShowResultUI(new ResultUIData()
            {
                Type = winner.Type,
                HasEveryoneFolded = false,
                Name = winner?.GetName,
                PotAmount = (int)_potAmount,
                WinningHand = temp
            });
        }

        private void GetRiver()
        {
            //Show one community Card
            _communityCards.Add(_deck.Deal());
            _tableManager.UpdateCommunityCards(_communityCards);
            _gameRound = EGameRound.River;
        }

        private void GetTurn()
        {
            //show one community card
            _communityCards.Add(_deck.Deal());
            _tableManager.UpdateCommunityCards(_communityCards);
            _gameRound = EGameRound.Turn;
        }

        private void GetFlop()
        {
            //Show three community cards;
            for (int i = 0; i < 3; i++)
            {
                _communityCards.Add(_deck.Deal());
            }

            _tableManager.UpdateCommunityCards(_communityCards);
            _gameRound = EGameRound.Flop;
        }

        #endregion

        #region Validation Methods

        private bool CheckIfSomeoneRaised(int currentIndex)
        {
            for (int i = currentIndex - 1; i >= 0; i--)
            {
                if (_totalPlayers[i].CurrentState != EHandCommandType.Fold)
                {
                    if (_totalPlayers[i].MoneySpent < _highestBet)
                    {
                        HasSomeoneRaised = true;
                        return true;
                    }
                }
            }

            for (int i = _totalPlayers.Count - 1; i > currentIndex; i--)
            {
                if (_totalPlayers[i].CurrentState != EHandCommandType.Fold)
                {
                    if (_totalPlayers[i].MoneySpent < _highestBet)
                    {
                        HasSomeoneRaised = true;
                        return true;
                    }
                }
            }

            return false;
        }

        private void CheckIfSomeoneBet(int currentIndex)
        {
            for (int i = currentIndex - 1; i > 0; i--)
            {
                if (_totalPlayers[i].CurrentState != EHandCommandType.Fold)
                {
                    HasSomeoneBet = HasSomeoneBet || _totalPlayers[i].CurrentState == EHandCommandType.Bet;
                }
            }

            for (int i = _totalPlayers.Count - 1; i > currentIndex; i--)
            {
                if (_totalPlayers[i].CurrentState != EHandCommandType.Fold)
                {
                    HasSomeoneBet = HasSomeoneBet || _totalPlayers[i].CurrentState == EHandCommandType.Bet;
                }
            }
        }

        private void ResetPlayersOnNextRound()
        {
            foreach (var player in _totalPlayers)
            {
                if (player.CurrentState != EHandCommandType.Fold)
                {
                    ((BasePlayer)player).ChangeStateToNone();
                }
            }
        }

        #endregion

        #region Message Callbacks

        private void OnPlayerTurnCompleteReceived(PlayerTurnCompleteMessage data)
        {
            var playersInGame = _totalPlayers.FindAll(x => x.CurrentState != EHandCommandType.Fold);
            if (playersInGame.Count < 2)
            {
                Debug.Log($"Player count {playersInGame.Count} Check for winner");
                CheckForGameWinner(playersInGame);
                return;
            }

            if (_currentPlayerIndexInRound == _roundStartPlayerIndex)
            {
                //Check fold call raise if highest amount is bigger than player's bet
                if (_gameRound == EGameRound.Initial)
                {
                    if (!CheckIfSomeoneRaised(_currentPlayerIndexInRound))
                    {
                        GetNextRound();
                    }
                }
                else
                {
                    GetNextRound();
                    if (_gameRound == EGameRound.Result)
                    {
                        playersInGame = _totalPlayers.FindAll(x => x.CurrentState != EHandCommandType.Fold);
                        CheckForGameWinner(playersInGame);
                        return;
                    }

                    ResetPlayersOnNextRound();
                }
            }

            _currentPlayerIndexInRound = data.Index + 1;
            CheckIfSomeoneBet(_currentPlayerIndexInRound);
            if (_currentPlayerIndexInRound >= _totalPlayers.Count)
            {
                _currentPlayerIndexInRound = 0;
            }

            DelayBeforeExecutingTurns(_currentPlayerIndexInRound);
        }

        private void OnPotRaisedMessage(PotRaisedMessage data)
        {
            _potAmount += data.Amount;
            _highestBet = data.HighestBet;
        }

        #endregion
    }
}