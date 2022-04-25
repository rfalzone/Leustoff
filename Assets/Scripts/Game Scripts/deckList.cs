using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deckList : MonoBehaviour
{

    public List<LeustoffCard> fullDecklist;
    public List<string> CurrentDecklist;
    public string deckName;
    public Deck sourceDeck;
    /// <summary>
    /// Method <c>UpdateDeckcounter</c> updates UI elements which show total deck counts, as well as number of penalty cards
    /// Rpc Subfunction
    /// </summary>
    public void CopyList(Deck otherDeck)
    {
        sourceDeck = otherDeck;
        foreach(LeustoffCard _card in otherDeck.masterCards)
        {
            fullDecklist.Add(_card);
            CurrentDecklist.Add(_card.cardName);
            
        }
        deckName = otherDeck.deckName;
      //  winnerCard = otherDeck.winnerCard;


    }
    /// <summary>
    /// Method <c>GetRandomCard</c> returns the name of a random card in your deck
    /// "Winner" is a special card to indicate you have depleted your deck and therefore won
    /// Design note: This will not happen during normal gameplay, but exists to make sure the game cannot become unfinishable
    /// </summary>
    public string GetRandomCard()
    {
        if (CurrentDecklist.Count > 0)
        {
            
            string card = CurrentDecklist[Random.Range(0, CurrentDecklist.Count)];
            return (card);
        }
        else
        {

            return ("Winner");

        }
    }
    /// <summary>
    /// Method <c>RemoveByName</c> removes a card based on its game name
    /// string CardName is the name of the card to be removed
    /// </summary>
    public void RemoveByName(string CardName)
    {
        foreach (string _card in CurrentDecklist)
        {
            if (_card == CardName)
            {
                CurrentDecklist.Remove(_card);
                return;
            }
        }
    }
    /// <summary>
    /// Method <c>AddTempCard</c> adds a card for this round of gameplay to your deck
    /// string card is the name of the card to add
    /// This is only currently used for the penalty card, which is added when the player takes enough damage
    /// </summary>
    public void AddTempCard(string card)
    {
        CurrentDecklist.Add(card);
    }




}
