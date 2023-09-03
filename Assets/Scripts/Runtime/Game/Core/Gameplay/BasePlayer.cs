using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Game;
using SimplePoker.Core.Environment;
using SimplePoker.GameMessages;
using SimplePoker.UI;
using SimplePoker.Utils;
using TMPro;
using UnityEngine;

namespace SimplePoker.Core
{
    public abstract class BasePlayer : MonoBehaviour, IGamePlayer
    {
        [SerializeField] protected TextMeshProUGUI RoleText;
        [SerializeField] protected TextMeshProUGUI MoneySpentText;
        [SerializeField] protected TextMeshProUGUI MoneyRemainingText;
        [SerializeField] protected TextMeshProUGUI CurrentStateText;
        [SerializeField] protected TextMeshProUGUI Name;
        [SerializeField] protected GameObject Indicator;
        protected EGameRole _gameRole;
        private EPlayerType _id;
        protected int _positionAtTable;
        private string _playerName;

        public readonly List<Card> _playerCards = new();

        protected long _moneyInBank = 10000;
        protected long _moneySpent = 0;

        protected EHandCommandType _currentState = EHandCommandType.None;

        protected PlayerSpot _playerSpot;

        protected CancellationTokenSource _cancellationToken = new();

        private List<CardView> _cardViews = new();

        private const int MAX_BET_AMOUNT = 400;

        public virtual void InitPlayer(EPlayerType id, int positionAtTable, PlayerSpot playerSpot)
        {
            _id = id;
            _positionAtTable = positionAtTable;
            _playerSpot = playerSpot;
            _playerName = id == EPlayerType.Main ? $" Your Player" : $"NPC {positionAtTable}";
            Name.text = _playerName;
        }

        public void AddCardToPlayer(Card card, CardView cardView)
        {
            _playerCards.Add(card);
            _cardViews.Add(cardView);
        }

        public void MakeDecision()
        {
            DecideAndExecuteCommand();
        }

        protected abstract void DecideAndExecuteCommand();

        public int GetCardsWeight()
        {
            var cards = _playerCards.OrderBy(x => x.FaceValue).ToList();
            return cards.GetCardsWeight();
        }

        public virtual int RaiseBlind()
        {
            int amount = 0;
            switch (_gameRole)
            {
                case EGameRole.BigBlind:
                    _moneySpent += (int)GameManager.Instance.TableManager.BigBlindAmount;
                    amount = 50;
                    break;
                case EGameRole.SmallBlind:
                    amount = (int)GameManager.Instance.TableManager.BigBlindAmount / 2;
                    _moneySpent += amount;
                    break;
            }

            _currentState = EHandCommandType.Raise;
            UpdateTexts();
            return amount;
        }

        public void UpdateWins(long amount)
        {
            _moneyInBank += amount;
            UpdateTexts();
        }

        public void ChangeStateToNone()
        {
            _currentState = EHandCommandType.None;
        }
        
        public void Reset()
        {
            ResetCards();
            _moneySpent = 0;
            //if the player has even less amount than Table's big blind. Make him buy in again
            if (_moneyInBank - _moneySpent < GameManager.Instance.TableManager.BigBlindAmount)
            {
                _moneyInBank = 10000;
            }
            _currentState = EHandCommandType.None;
            UpdateTexts();
        }

        public virtual void PerformFoldCommand()
        {
            _currentState = EHandCommandType.Fold;
            UpdateTexts();
            ResetCards();
        }

        public virtual void PerformRaiseCommand()
        {
            System.Random random = new System.Random();
            var limit = _moneyInBank - _moneySpent;
            int amountToBeRaised = 0;
            if ((int)GameManager.Instance.GameplayManager.HighestAmount > _moneyInBank - _moneySpent)
            {
                Debug.Log("Cannot Call or Bet");
            }
            else
            {
                if (_moneySpent == (int)GameManager.Instance.GameplayManager.HighestAmount)
                {
                    var min = Math.Min(limit, MAX_BET_AMOUNT);
                    amountToBeRaised = random.Next(100, (int)min);
                }
                else
                {
                    PerformCallCommand();
                    limit = _moneyInBank - _moneySpent;
                    var min = Math.Min(limit, MAX_BET_AMOUNT);
                    amountToBeRaised = random.Next(100, (int)min);
                }
            }
            _currentState = EHandCommandType.Raise;
            _moneySpent += amountToBeRaised;
            PublishPotRaisedMessage(amountToBeRaised, EHandCommandType.Raise);
            UpdateTexts();
        }

        public virtual void PerformCallCommand()
        {
            _currentState = EHandCommandType.Call;
            int amountToBeRaised = 0;
            if ((int)GameManager.Instance.GameplayManager.HighestAmount > _moneyInBank - _moneySpent)
            {
                Debug.Log("Cannot Call");
            }
            else
            {
                amountToBeRaised = (int)GameManager.Instance.GameplayManager.HighestAmount - (int)_moneySpent;
                _moneySpent = (int)GameManager.Instance.GameplayManager.HighestAmount;
            }

            PublishPotRaisedMessage(amountToBeRaised, EHandCommandType.Call);
            UpdateTexts();
        }

        public virtual void PerformCheckCommand()
        {
            _currentState = EHandCommandType.Check;
            UpdateTexts();
        }

        public virtual void PerformBetCommand()
        {
            System.Random random = new System.Random();
            var limit = _moneyInBank - _moneySpent;
            int amountToBeRaised = 0;
            if ((int)GameManager.Instance.GameplayManager.HighestAmount > _moneyInBank - _moneySpent)
            {
                Debug.Log("Cannot Call or Bet");
            }
            else
            {
                if (_moneySpent == (int)GameManager.Instance.GameplayManager.HighestAmount)
                {
                    var min = Math.Min(limit, MAX_BET_AMOUNT);
                    amountToBeRaised = random.Next(100, (int)min);
                }
                else
                {
                    PerformCallCommand();
                    limit = _moneyInBank - _moneySpent;
                    var min = Math.Min(limit, MAX_BET_AMOUNT);
                    amountToBeRaised = random.Next(100, (int)min);
                }
            }

            _moneySpent += amountToBeRaised;
            PublishPotRaisedMessage(amountToBeRaised, EHandCommandType.Bet);
            _currentState = EHandCommandType.Bet;
            UpdateTexts();
        }

        public void UpdateIndicator(bool value)
        {
            Indicator.SetActive(value);
        }
        
        private void PublishPotRaisedMessage(int amountToBeRaised, EHandCommandType commandType)
        {
            MessageBroker.Instance.Publish(new PotRaisedMessage()
            {
                Amount = (int)amountToBeRaised,
                HighestBet = (int)_moneySpent,
                CommandType = commandType
            });
        }

        protected void UpdateTexts()
        {
            RoleText.text = $"{Enum.GetName(typeof(EGameRole), _gameRole)}";
            MoneyRemainingText.text = $"Remaining: {_moneyInBank - _moneySpent}";
            MoneySpentText.text = $"Spent: {_moneySpent}";
            CurrentStateText.text = _currentState == EHandCommandType.None? "" : $"LastMove: {Enum.GetName(typeof(EHandCommandType), _currentState)}";
            UpdateIndicator(false);
        }

        protected void ResetCards()
        {
            if (_cardViews.Count > 0)
            {
                foreach (var cardView in _cardViews)
                {
                    Destroy(cardView.gameObject);
                }
                _cardViews.Clear();
                _playerCards.Clear();
            }
        }
        public EPlayerType Type => _id;

        public EGameRole GameRole
        {
            get => _gameRole;
            set
            {
                _gameRole = value;
                UpdateTexts();
            }
        }

        public int PositionAtTable => _positionAtTable;
        public EHandCommandType CurrentState => _currentState;

        public List<Card> GetCards => _playerCards;

        public long MoneySpent => _moneySpent;

        public PlayerSpot PlayerSpot => _playerSpot;

        public long MoneyInBank => _moneyInBank;

        public string GetName => _playerName;
    }
}