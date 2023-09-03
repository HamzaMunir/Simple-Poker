using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace SimplePoker.Core
{
    public enum EPokerHand
    {
        ROYAL_FLUSH = 250,
        STRAIGHT_FLUSH = 50,
        FOUR_OF_A_KIND = 25,
        FULL_HOUSE = 9,
        FLUSH = 6,
        STRAIGHT = 4,
        THREE_OF_A_KIND = 3,
        TWO_PAIR = 2,
        PAIR = 1,
        HIGH_CARD = 0
    }

    public class PokerHand
    {
        public EPokerHand Hand;
        public int Weight;
        public List<Card> cards;
    }
    public class PokerHandChecker
    {
        public const int ROYAL_FLUSH = 250;
        public const int STRAIGHT_FLUSH = 50;
        public const int FOUR_OF_A_KIND = 25;
        public const int FULL_HOUSE = 9;
        public const int FLUSH = 6;
        public const int STRAIGHT = 4;
        public const int THREE_OF_A_KIND = 3;
        public const int TWO_PAIR = 2;
        public const int PAIR = 1;

        public PokerHand DetectHands(List<Card> playerCards, List<Card> communityCards)
        {
            var totalCards = playerCards.Concat(communityCards).ToList();
            var sortedList = SortCardsByFaceValue(totalCards);

            PokerHand hand = new PokerHand();
            List<Card> handCards = new();
            int weight = 0;
            if (RoyalFlush(sortedList, out handCards))
            {
                hand.Hand = EPokerHand.ROYAL_FLUSH;
                hand.Weight = (int)EPokerHand.ROYAL_FLUSH;
                hand.cards = handCards;
                return hand;
            }

            if (StraightFlush(sortedList, out handCards))
            {
                hand.Hand = EPokerHand.STRAIGHT_FLUSH;
                hand.Weight = (int)EPokerHand.STRAIGHT_FLUSH;
                hand.cards = handCards;
                return hand;
            }

            if (FourOfAKind(sortedList,out handCards, out weight))
            {
                hand.Hand = EPokerHand.FOUR_OF_A_KIND;
                hand.Weight = (int)EPokerHand.FOUR_OF_A_KIND + weight;
                hand.cards = handCards;
                return hand;
            }

            if (FullHouse(sortedList,out handCards, out weight))
            {
                hand.Hand = EPokerHand.FULL_HOUSE;
                hand.Weight = (int)EPokerHand.FULL_HOUSE + weight;
                hand.cards = handCards;
                return hand;
            }

            if (Flush(sortedList, out handCards))
            {
                hand.Hand = EPokerHand.FLUSH;
                hand.Weight = (int)EPokerHand.FLUSH;
                hand.cards = handCards;
                return hand;
            }

            if (Straight(sortedList,out handCards, out _, out weight))
            {
                hand.Hand = EPokerHand.STRAIGHT;
                hand.Weight = (int)EPokerHand.STRAIGHT + weight;
                hand.cards = handCards;
                return hand;
            }

            if (ThreeOfAKind(sortedList,out _, out handCards, out weight))
            {
                hand.Hand = EPokerHand.THREE_OF_A_KIND;
                hand.Weight = (int)EPokerHand.THREE_OF_A_KIND + weight;
                hand.cards = handCards;
                return hand;
            }

            if (TwoPair(sortedList,out handCards, out weight))
            {
                hand.Hand = EPokerHand.TWO_PAIR;
                hand.Weight = (int)EPokerHand.TWO_PAIR + weight;
                hand.cards = handCards;
                return hand;
            }

            if (Pair(sortedList, out weight, out _, out handCards))
            {
                hand.Hand = EPokerHand.PAIR;
                hand.Weight = (int)EPokerHand.PAIR + weight;
                hand.cards = handCards;
                return hand;
            }

            int highCard = HighCard(playerCards, out Card card);
            handCards.Add(card);
            for (int i = 0; i < communityCards.Count - 1; i++)
            {
                handCards.Add(communityCards[i]);
            }
            hand.Hand = EPokerHand.HIGH_CARD;
            hand.Weight = highCard;
            hand.cards = handCards;
            return hand;
        }

        private bool RoyalFlush(List<Card> values, out List<Card> hand)
        {
            hand = new();
            if (Straight(values,out List<Card> straight, out int lastIndex, out _) && Flush(values, out _))
            {
                if (values[lastIndex].FaceValue == EFaceValue.Ace)
                {
                    hand.AddRange(straight);
                    return true;
                }
            }

            return false;
        }

        private bool StraightFlush(List<Card> values, out List<Card> hand)
        {
            hand = new();
            if (Straight(values,out List<Card> straight, out _, out _) && Flush(values, out List<Card> flushHand))
            {
                hand.AddRange(straight);
                return true;
            }

            return false;
        }

        private bool FullHouse(List<Card> values,out List<Card> hand, out int weight)
        {
            weight = 0;
            hand = new();
            if (Pair(values,out _, out List<Card> pair, out _) && ThreeOfAKind(values,out List<Card> threePair, out _, out weight))
            {
                foreach (var card in pair)
                {
                    hand.Add(card);
                }
                
                foreach (var card in threePair)
                {
                    hand.Add(card);
                }
                return true;
            }

            return false;
        }

        private bool Flush(List<Card> values, out List<Card> hand)
        {
            var sortedValues = SortCardsBySuit(values);
            hand = new();
            if (sortedValues[0].Suit == sortedValues[4].Suit)
            {
                for (int i = 0; i < 5; i++)
                {
                    hand.Add(sortedValues[i]);
                }
                return true;
            }

            if (sortedValues.Count < 6)
            {
                return false;
            }

            if (sortedValues[1].Suit == sortedValues[5].Suit)
            {
                for (int i = 1; i < 6; i++)
                {
                    hand.Add(sortedValues[i]);
                }
                return true;
            }

            if (sortedValues.Count < 7)
            {
                return false;
            }

            if (sortedValues[2].Suit == sortedValues[6].Suit)
            {
                for (int i = 2; i < 7; i++)
                {
                    hand.Add(sortedValues[i]);
                }
                return true;
            }

            return false;
        }

        private bool Straight(List<Card> values,out List<Card> hand, out int lastIndex, out int weight)
        {
            //1345679
            bool isSequence = false;
            lastIndex = values.Count;
            weight = 0;
            hand = new();
            for (int i = 0; i < values.Count; i++)
            {
                int sequenceCount = 1;
                if (values.Count - i < 5)
                {
                    break;
                }

                int j = i;
                for (; j < i + 4; j++)
                {
                    if (values[j].FaceValue + 1 != values[j + 1].FaceValue)
                    {
                        break;
                    }
                    hand.Add(values[j]);
                    sequenceCount++;
                }

                if (sequenceCount == 5)
                {
                    lastIndex = j;
                    weight = (int)values[j].FaceValue;
                    isSequence = true;
                }
            }

            return isSequence;
        }

        private bool FourOfAKind(List<Card> values, out List<Card> hand, out int weight)
        {
            var groupedList = values.GroupBy(i => i.FaceValue);

            hand = new();
            weight = 0;
            foreach (var group in groupedList)
            {
                if (group.Count() == 4)
                {
                    foreach (var card in group.AsEnumerable())
                    {
                        hand.Add(card);
                    }

                    int cardCount = 0;
                    foreach (var card in values)
                    {
                        if (!hand.Contains(card))
                        {
                            hand.Add(card);
                            cardCount++;
                        }

                        if (cardCount == 1)
                        {
                            break;
                        }
                    }
                    weight += (int)group.Key;
                    return true;
                }
            }

            return false;
        }

        private bool ThreeOfAKind(List<Card> values,out List<Card> threePair, out List<Card> hand, out int weight)
        {
            var groupedList = values.GroupBy(i => i.FaceValue);

            weight = 0;
            hand = new();
            threePair = new();
            foreach (var group in groupedList)
            {
                if (group.Count() == 3)
                {
                    foreach (var card in group.AsEnumerable())
                    {
                        hand.Add(card);
                        threePair.Add(card);
                    }

                    int cardCount = 0;
                    foreach (var card in values)
                    {
                        if (!hand.Contains(card))
                        {
                            hand.Add(card);
                            cardCount++;
                        }

                        if (cardCount == 2)
                        {
                            break;
                        }
                    }
                    weight += (int)group.Key;
                    return true;
                }
            }

            return false;
        }

        private bool TwoPair(List<Card> values,out List<Card> hand, out int weight)
        {
            return Pair(values, out weight,out _, out hand, 2);
        }

        private bool Pair(List<Card> values,out int weight, out List<Card> pair, out List<Card> fullHand, int pairCount = 1)
        {
            pair = new();
            fullHand = new();
            // If only one duplicate in values, a pair exists
            var groupedList = values.GroupBy(i => i.FaceValue);
            weight = 0;
            int count = 0;

            foreach (var group in groupedList)
            {
                if (group.Count() == 2)
                {
                    pair = new();
                    foreach (var card in group.AsEnumerable())
                    {
                        pair.Add(card);
                        fullHand.Add(card);
                    }
                    weight += (int)group.Key;
                    count++;
                }
            }
            
            if (pairCount == count)
            {
                int cardCount = 0;
                int handLimit = 5 - (pairCount * 2);
                foreach (var card in values)
                {
                    if (!fullHand.Contains(card))
                    {
                        fullHand.Add(card);
                        cardCount++;
                    }

                    if (cardCount == handLimit)
                    {
                        break;
                    }
                }
                return true;
            }

            return false;
        }

        private int HighCard(List<Card> values, out Card card)
        {
            card = default;
            if (values[0].FaceValue > values[1].FaceValue)
            {
                card = values[0];
                return (int)values[0].FaceValue;
            }

            card = values[1];
            return (int)values[1].FaceValue;
        }

        private List<Card> SortCardsByFaceValue(List<Card> cards)
        {
            var sortedList = cards
                .OrderBy(x => (int)(x.FaceValue))
                .ToList();
            return sortedList;
        }

        private List<Card> SortCardsBySuit(List<Card> cards)
        {
            var sortedList = cards
                .OrderBy(x => (int)(x.Suit))
                .ToList();
            return sortedList;
        }
    }
}