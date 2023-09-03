using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using SimplePoker.Core.General;

namespace SimplePoker.UI
{
    public class CardView : MonoBehaviour
    {
        private Card _card;

        [SerializeField] private TextMeshProUGUI _upperNumberText;
        [SerializeField] private TextMeshProUGUI _downNumberText;
        [SerializeField] private Image _mainImage;
        [SerializeField] private Image _upperImage;
        [SerializeField] private Image _downImage;

        public void InitView(Card card)
        {
            _card = card;
            var sprite = AssetManager.Instance.GetSprite(Enum.GetName(typeof(ESuit),card.Suit));
            _mainImage.sprite = sprite;
            _upperImage.sprite = sprite;
            _downImage.sprite = sprite;
            _upperNumberText.text = card.GetFaceIndicator();
            _downNumberText.text = card.GetFaceIndicator();
        }
        
    }
}