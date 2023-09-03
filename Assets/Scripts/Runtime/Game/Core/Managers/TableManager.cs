using System.Collections.Generic;
using SimplePoker.Core.Environment;
using SimplePoker.UI;
using TMPro;
using UnityEngine;
using SimplePoker.Core.General;

namespace SimplePoker.Core
{
    public class TableManager : MonoBehaviour
    {
        private GameObject _playerPrefab => AssetManager.Instance.GetPrefab(nameof(Player));
        private GameObject _enemyPrefab => AssetManager.Instance.GetPrefab(nameof(EnemyPlayer));
        [SerializeField] private PlayerSpot _playerSpawnPoint;
        [SerializeField] private List<PlayerSpot> _enemySpawnPoints;
        [SerializeField] private RectTransform _communityCardContent;
        [SerializeField] private TextMeshProUGUI _potAmountText;
        private Player _player;
        private readonly List<EnemyPlayer> _otherPlayers = new();
        private readonly List<IGamePlayer> _totalPlayers = new();
        private int _bigBlindAmount = 100;
        
        public void InitTable()
        {
            var player = Instantiate(_playerPrefab, _playerSpawnPoint.PlayerSpawnPoint);
            _player = player.GetComponent<Player>();
            _player.InitPlayer(EPlayerType.Main, 0, _playerSpawnPoint);
            
            _totalPlayers.Add(_player);
            var enemyPrefab = _enemyPrefab;
            for (int i = 0; i < GameManager.Instance.EnemyPlayerCount; i++)
            {
                if (i < _enemySpawnPoints.Count)
                {
                    var enemy = Instantiate(enemyPrefab, _enemySpawnPoints[i].PlayerSpawnPoint).GetComponent<EnemyPlayer>();
                    enemy.InitPlayer(EPlayerType.NPC, i + 1, _enemySpawnPoints[i]);
                    _otherPlayers.Add(enemy);
                    _totalPlayers.Add(enemy);
                }
            }
            
            GameManager.Instance.GameplayManager.InitGame(_otherPlayers, _totalPlayers);

        }

        public void UpdateCommunityCards(List<Card> cards)
        {
            ResetCommunityCardContent();
            foreach (var card in cards)
            {
                var cardView = Instantiate(AssetManager.Instance.GetPrefab(nameof(CardView)), _communityCardContent).GetComponent<CardView>();
                cardView.InitView(card);
            }
        }
        
        private void ResetCommunityCardContent()
        {
            foreach (Transform t in _communityCardContent.transform)
            {
                Destroy(t.gameObject);
            }
        }

        public void UpdatePotAmount(long amount)
        {
            _potAmountText.text = $"Pot: {amount}";
        }

        public int BigBlindAmount => _bigBlindAmount;
    }
}