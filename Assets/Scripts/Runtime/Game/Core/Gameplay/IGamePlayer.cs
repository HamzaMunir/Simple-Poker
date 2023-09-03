using System.Collections.Generic;
using SimplePoker.UI;

namespace SimplePoker.Core
{
    /// <summary>
    /// This interface is responsible for providing necessary methods and public fields to every game player
    /// </summary>
    public interface IGamePlayer
    {
        /// <summary>
        /// This field corresponds to the player, if its an NPC or Main
        /// </summary>
        public EPlayerType Type { get; }

        /// <summary>
        /// This field corresponds to which role this specific game player is
        /// </summary>
        public EGameRole GameRole { get; set; }

        /// <summary>
        /// This field corresponds to having the current state of the player
        /// </summary>
        public EHandCommandType CurrentState { get; }

        /// <summary>
        /// This field gives the information about the money spent by the player
        /// </summary>
        public long MoneySpent { get; }

        /// <summary>
        /// This fields corresponds to player's total buy in money
        /// </summary>
        public long MoneyInBank { get; }

        /// <summary>
        /// Add the dealt card to player's cards
        /// </summary>
        /// <param name="card">Card object having the information about card</param>
        /// <param name="cardView">Card View Gameobject </param>
        public void AddCardToPlayer(Card card, CardView cardView);

        /// <summary>
        /// This method will raise blind according to player's role
        /// </summary>
        /// <returns></returns>
        public int RaiseBlind();

        /// <summary>
        /// This method start the decision making logic
        /// </summary>
        public void MakeDecision();

        /// <summary>
        /// Weight of the dealt cards
        /// </summary>
        /// <returns></returns>
        public int GetCardsWeight();

        /// <summary>
        /// Update the winnings of this player
        /// </summary>
        /// <param name="amount"></param>
        public void UpdateWins(long amount);

        /// <summary>
        /// Update display indicator
        /// </summary>
        /// <param name="value"></param>
        public void UpdateIndicator(bool value);

        /// <summary>
        /// Get the dealt cards
        /// </summary>
        public List<Card> GetCards { get; }

        /// <summary>
        /// Reset and ready the player for next round
        /// </summary>
        public void Reset();
        
        public string GetName { get; }
    }
}