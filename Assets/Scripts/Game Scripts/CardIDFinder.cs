using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardIDFinder : MonoBehaviour
{
    
    public Deck deck;

    public  LeustoffCard GetCardByID(string ID)
    {
        foreach (LeustoffCard card in deck.cards)
        {
            if (card.itemID == ID)
            {
                return card;
            }

        }
        Debug.Log("Trying to draw missing ID");
        return null;
    }
        
        
    

}
