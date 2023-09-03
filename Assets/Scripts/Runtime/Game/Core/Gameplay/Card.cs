
namespace SimplePoker
{
    public class Card
    {
        private ESuit _suit;
        private EFaceValue _faceValue;

        public ESuit Suit
        {
            get => _suit;
            set => _suit = value;
        }

        public EFaceValue FaceValue
        {
            get => _faceValue;
            set => _faceValue = value;
        }

        public string GetFaceIndicator()
        {
            int faceValueInteger = (int)_faceValue;
            if (faceValueInteger < 11)
            {
                return faceValueInteger.ToString();
            }

            switch (faceValueInteger)
            {
                case 11:
                    return "J";
                case 12:
                    return "Q";
                case 13:
                    return "K";
                case 14:
                    return "A";
            }

            return "";
        }
        public string GetFaceName()
        {
            return $"{_faceValue} of {_suit}";
        }
    }
}