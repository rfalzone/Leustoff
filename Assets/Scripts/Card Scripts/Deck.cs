using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "Deck", menuName = "Deck")]
public class Deck : ScriptableObject
{
    //The decklist that does not change game to game
    public List<LeustoffCard> masterCards;
    //The current cards left in the deck
    public List<LeustoffCard> cards;
    public string deckName;

}
