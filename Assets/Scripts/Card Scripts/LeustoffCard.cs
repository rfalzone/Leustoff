using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LeustoffNamespace;

[CreateAssetMenu(fileName = "New Leustoff", menuName = "Cards")]
public class LeustoffCard : ScriptableObject
{



    public string itemID;

    public cardType _cardType;
    public string cardName;

    public string cost;
    public string power;
    public string defense;
    public Sprite art;
    public Sprite altArt;
    public bool isFaceUp;
    public List<Ability> abilities = new List<Ability>();


}
