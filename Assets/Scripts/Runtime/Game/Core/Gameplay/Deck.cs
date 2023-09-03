using System;
using System.Collections.Generic;

namespace SimplePoker
{
    public enum ESuit
    {
        Diamond,
        Clubs,
        Heart,
        Spade
    }

    public enum EFaceValue
    {
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Ace = 14
    }

    public class Deck
    {
        private const int _deckSize = 52;
        private List<Card> _deck = new();
        private int currentCardIndex = 0;

        private System.Random _random;

        public Deck()
        {
            _random = new System.Random();
            GenerateDeck();
            ReadyDeck();
        }

        private void GenerateDeck()
        {
            foreach (ESuit suitValue in Enum.GetValues(typeof(ESuit)))
            {
                foreach (EFaceValue faceValue in Enum.GetValues(typeof(EFaceValue)))
                {
                    _deck.Add(new Card
                    {
                        Suit = suitValue,
                        FaceValue = faceValue
                    });
                }
            }
        }

        private void Shuffle()
        {
            int randomNumber;
            for (int i = 0; i < _deckSize; i++)
            {
                randomNumber = _random.Next(_deckSize);
                SwapCard(i, randomNumber);
            }
        }

        private void SwapCard(int oldPos, int newPos)
        {
            (_deck[oldPos], _deck[newPos]) = (_deck[newPos], _deck[oldPos]);
        }

        public Card Deal()
        {
            currentCardIndex++;
            if (IsDeckEmpty())
            {
                return default;
            }
            return _deck[currentCardIndex];
        }

        public bool IsDeckEmpty()
        {
            return currentCardIndex > _deckSize - 1;
        }

        public void ReadyDeck()
        {
            currentCardIndex = 0;
            Shuffle();
        }
    }
}