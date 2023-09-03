using System.Collections.Generic;
using System.Linq;
using SimplePoker.Core;

namespace SimplePoker.Utils
{
    public static class CardExtensions
    {
        public static int GetCardsWeight(this List<Card> cards)
        {
            int cardsWeight = 0;
            var sortedCards = cards.OrderBy(x => x.FaceValue).ToList();
            for (int i = 0; i < sortedCards.Count; i++)
            {
                cardsWeight += (int) sortedCards[i].FaceValue;
            }
            bool sequence = IsSequenceOrPair(ref sortedCards, out bool isSameSuit, out bool isPair);
            if (sequence)
            {
                cardsWeight += GameplayManager.SEQUENCE_WEIGHT;
                if (isSameSuit)
                {
                    cardsWeight += GameplayManager.SAME_SUIT_WEIGHT;
                }
            }
            
            if (isPair)
            {
                cardsWeight += GameplayManager.PAIR_WEIGHT;
            }

            return cardsWeight;
        }
        private static bool IsSequenceOrPair(ref List<Card> cards, out bool isSameSuit, out bool isPair)
        {
            bool sequence = true;
            isSameSuit = true;
            isPair = false;
            for (int i = 0; i < cards.Count - 1; i++)
            {
                sequence = sequence && cards[i].FaceValue < cards[i + 1].FaceValue;
                isSameSuit = isSameSuit && cards[i].Suit == cards[i + 1].Suit;
                isPair = isPair || cards[i].FaceValue == cards[i + 1].FaceValue;
            }

            return sequence;
        }
    }
}