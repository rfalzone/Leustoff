using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LeustoffNamespace;
using TMPro;
using Mirror;

public class ReminderTextManager : MonoBehaviour
{
    public Text reminderText;
    public ObjType myType;
    public GameObject TextObj;
    /// <summary>
    /// Method <c>SetText</c> creates the text explaining the abilities on cards
    /// string Ability is the name of the ability to explain
    /// </summary>
    public void SetText(Ability Ability)
    {
        switch (Ability)
        {
            case Ability.Flying:
                
                reminderText.text = "Can only be blocked by other flying";
                break;
            case Ability.Fortify:
                reminderText.text = "Fortify\nGains 1 defense when you're damaged, drains when not damaged\n";
                break;
            case Ability.Draw1:
                reminderText.text = "Draw a card";
                break;
            case Ability.Edict:
                reminderText.text = "Edict\nDestroy each players smallest creature";
                break;
            case Ability.Disrupt:
                reminderText.text = "Disrupt\nLowers the targets attack and defense by this spells power temporarily";
                break;
            case Ability.Dampen:
                reminderText.text = "";
                break;
            case Ability.Insulated:
                reminderText.text = "Take one aditional damage before taking penalty";
                break;
            case Ability.Shielded:
                reminderText.text = "Survive being destroyed once";
                break;
            case Ability.Trial:
                reminderText.text = "Destroy each creature but the largest of each player";
                break;
            case Ability.Counterspell:
                reminderText.text = "Counter a combo";
                break;
            case Ability.Glasses:
                reminderText.text = "Glasses\nSee opponents hands";
                break;


            case Ability.DestroyTarget:
                reminderText.text = "Destroy an enemy creature";
                break;

            case Ability.Sweeper:
                reminderText.text = "Destroy all creatures";
                break;


            case Ability.Bounce:
                reminderText.text = "Return target to hand";
                break;
            case Ability.Suicide:
                reminderText.text = "Debugging tool.  The card just goes to the graveyard";
                break;
            case Ability.Bloodthirsty:
                reminderText.text = "Gains +1 attack for each unpaid Tax in opponents decks";
                break;
            case Ability.Rhythm:
                reminderText.text = "Rhythm\nWhen cast: Double power if you dealt damage last turn";
                break;
            case Ability.Opportunist:
                reminderText.text = "Opportunist\nDraw an extra card when you deal 3 or more damage";
                break;
            case Ability.BlueChip:
                reminderText.text = "Blue Chip\nEach turn you don't take damage gain a revenue";
                break;
            case Ability.Anthem:
                reminderText.text = "Anthem\nYour creatures get +1/+1";
                break;
            case Ability.Hasten:
                reminderText.text = "Removes summoning sickness";
                break;
            case Ability.Haste:
                reminderText.text = "Haste\nCan attack on its first turn";
                break;
            case Ability.Dash:
                reminderText.text = "Dash\nReturns to your hand at end of turn";
                break;
            case Ability.Teamwork:
                reminderText.text = "Teamwork\nGets +1/+1 when you play another creature";
                break;
            case Ability.FortifyEnhance:
                reminderText.text = "Fortify Enhance\nDoubles the rate that Fortify accrues";
                break;
            case Ability.SpawnIce:
                reminderText.text = "Spawn Ice\nCreates a 1/1";
                break;
 
            
            case Ability.SpawnVernalPool:
                reminderText.text = "Spawn Vernal Pool\nSpawn a 2/1";
                break;
            case Ability.Beat:
                reminderText.text = "Beat\nEnables Rythm effect; you dealt damage last turn";
                break;
            case Ability.Behead:
                reminderText.text = "Behead\n Destroys each player's largest creature";
                break;
                                   

            
        }
    }
    /// <summary>
    /// Method <c>OnHoverEnter</c> creates the text explaining the UI element
    /// </summary>
    public void OnHoverEnter()
    {
        Destroy(GameObject.FindWithTag("ReminderText"));
        GameObject reminderText = Instantiate(TextObj, new Vector2(0, 0), Quaternion.identity);
        reminderText.transform.SetParent(GameObject.Find("DialogueBox").transform, false);
        reminderText.layer = LayerMask.NameToLayer("Zoom");

        reminderText.name = gameObject.name+"ReminderText";
   
        switch (myType){
            case ObjType.NumbersDisplay:
                reminderText.GetComponent<TextMeshProUGUI>().text = "Player variables \n Income.  Your attacks generate these points, when you get enough you draw a card. " +
                    "\n Deficit.  Getting attacked gives you these.  When you get too many, an entropy card is added to your deck" +
                    "\n Energy.  Used to cast spells.  Comes back each turn" +
                    "\n Dividends.  Drawing more cards than you can hold during the attack step gives dividends.  Spend a dividend to gain a temporary energy.";

                break;
            case ObjType.PointsCounter:
                reminderText.GetComponent<TextMeshProUGUI>().text = "This tracks  points.  You get points from Combo cards." +
                    "\n The first time you get seven points you do a power move based on your faciton\n" + FactionAbilityReminder() +
                    "\n The second time you get seven points you win.";
                break;
            case ObjType.Graveyard:
                reminderText.GetComponent<TextMeshProUGUI>().text = "This is a graveyard.\n Cards go here after they are destroyed or used up." +
                    "\nClick to expand." ;
               break;
            case ObjType.AttackButton:
                reminderText.GetComponent<TextMeshProUGUI>().text = "Click to end your turn.\n You will attack based on your total attack value. \n" +
                    "Attacking and dealing damage gives you income points, allowing you to draw cards" +
                    "\nCards highlighted in Red cannot attack until next turn.";
                break;
            case ObjType.ManaCrystal:
                reminderText.GetComponent<TextMeshProUGUI>().text = "Mana crystals.  Play one energy card a turn to get more." +
                    "\nIf you draw a card at the start of your turn, but your hand is full, you will get a mana crystal instead." +
                    "\nThis replaces your energy card for the turn.  This will also replace an energy card in your hand, if there is one.";
                break;
            case ObjType.SpendDividend:
                reminderText.GetComponent<TextMeshProUGUI>().text = "Gain one temporary mana." +
                    "\nYou gain a dividend when you draw a card other than the first card each turn, but your hand is full.";
                break;
            case ObjType.DeckCounter:
                reminderText.GetComponent<TextMeshProUGUI>().text = "Counts the number of cards in the deck.\nCounts the number of Entropy (bad) cards in deck.";
                break;
            case ObjType.PTCounter:
                reminderText.GetComponent<TextMeshProUGUI>().text = "Total power and toughness.  Note doesn't take summoning sickness into account.\n" +
                    "Summoning sickness: Cards highlighted in red can't attack until next turn.";
                break;
            case ObjType.Hotseat:
                reminderText.GetComponent<TextMeshProUGUI>().text = "A hotseat.  Shows the player or players with the lowest defense; they will be taking damage from opponents attacks.";
                break;
            case ObjType.ComboGauge:
                reminderText.GetComponent<TextMeshProUGUI>().text = "Combo Point Gauge.\nFill to 7 to activate faction special ability.\n" + FactionAbilityReminder() +
                    "\nFill a second time to win the game.  \nClick to see combo cards played so far.\nDamage shuffles points back into deck.";
                break;
            case ObjType.RevenueGauge:
                reminderText.GetComponent<TextMeshProUGUI>().text = "Fill by ending turn with more power than opponent's defenses.\n A full bar draws a card, or if hand is full, grants a dividend.";
                break;
            case ObjType.DamageGauge:
                reminderText.GetComponent<TextMeshProUGUI>().text = "Fills as you take damage.\nA full bar shuffles a combo point into your deck. \nIf you are out of combo points, puts a 2 mana card that draws 1 into your deck";
                break;
            case ObjType.DefenseDisplay:
                reminderText.GetComponent<TextMeshProUGUI>().text = "A prediction of how much damage you will take before your next turn.  Below are enemies current attacks.";
                break;
            case ObjType.DividendBox:
                reminderText.GetComponent<TextMeshProUGUI>().text = "Dividends are earned by drawing cards above your hand limit beyond the first card each turn.\nDividends can also be gained by discarding cards.\nDrag to graveyard to discard.";
                break;
            case ObjType.DrawButton:
                reminderText.GetComponent<TextMeshProUGUI>().text = "Spend 2 dividends to draw";
                break;
            case ObjType.ReinforceButton:
                reminderText.GetComponent<TextMeshProUGUI>().text = "Spend 4 dividends gain a permanent +1 to attack and defense";
                break;
            case ObjType.ComboButton:
                reminderText.GetComponent<TextMeshProUGUI>().text = "Spend 6 dividends gain a combo point";
                break;
            default:
                reminderText.GetComponent<TextMeshProUGUI>().text = "MISSING REMINDER TEXT";
                break;

        }

    }
    /// <summary>
    /// Method <c>FactionAbilityReminder</c> handles the reminder text for the power move ability of each faction
    /// </summary>
    public string FactionAbilityReminder()
    {
        GameManager GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        switch (GameManager.PlayerFactionList[NetworkClient.connection.identity.GetComponent<PlayerManager>().MyID])
        {
            case Faction.Planets:
                return "You are playing Planets, your power move destroys all enemy Matter";
                    break;
            case Faction.Materials:
                return "You are playing Materials, your power move refils your hand to 5 cards";
                break;
            case Faction.Biomes:
                return "You are playing Biomes, your power move lets you take an extra turn";
                break;
            case Faction.Media:
                return "You are playing Media, your power move doubles your mana crystals";
                break;

            default:
                return "Faciton not found";
                    break;

        }


    }

    public void OnHoverExit()
    {
        if (GameObject.Find(gameObject.name + "ReminderText") != null)
        {
            Destroy(GameObject.Find(gameObject.name+"ReminderText"));

        }


    }
   
}
