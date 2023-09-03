using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Kit;
using SimplePoker.Core;
using SimplePoker.Core.General;
using SimplePoker.GameMessages;
using SimplePoker.UI;
using SimplePoker.UI.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SimplePoker.Views
{
    public class DisplayResultUIView : UiBaseView
    {
        private ResultUIData _uiData => Data as ResultUIData;
        [SerializeField] private RectTransform _cardContent;
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private Button _nextRoundBtn;

        private void OnEnable()
        {
            _nextRoundBtn.onClick.AddListener(OnNextRound);
        }

        private void OnDisable()
        {
            _nextRoundBtn.onClick.RemoveListener(OnNextRound);
        }

        private void OnNextRound()
        {
            Hide();
            MessageBroker.Instance.Publish(new NextGameMessage());
        }

        protected override void RefreshView()
        {
            if (!_uiData.HasEveryoneFolded)
            {
                AudioManager.Instance.PlayUI(_uiData.Type == EPlayerType.Main
                    ? GameManager.Instance.UiManager.WinSound
                    : GameManager.Instance.UiManager.LostSound);
                _resultText.text = $"{_uiData.Name} won this round. \n Winning pot amount ${_uiData.PotAmount}";
                UpdateWinnerCards(_uiData.WinningHand.cards);
                return;
            }

            _resultText.text = $"Everyone folded, Round Over";
        }

        public void UpdateWinnerCards(List<Card> cards)
        {
            ResetWinningCardContent();
            foreach (var card in cards)
            {
                var cardView = Instantiate(AssetManager.Instance.GetPrefab(nameof(CardView)), _cardContent)
                    .GetComponent<CardView>();
                cardView.InitView(card);
            }
        }

        private void ResetWinningCardContent()
        {
            foreach (Transform t in _cardContent)
            {
                Destroy(t.gameObject);
            }
        }

        protected override void ResetView()
        {
            ResetWinningCardContent();
        }
    }
}