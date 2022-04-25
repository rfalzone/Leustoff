using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Text.RegularExpressions;
using LeustoffNamespace;

public class NonPlayerManager : PlayerManager
{
    public string Behavior;
    public bool firstturn = true;
    public override void CMDSetID(Faction myFaction)
    {

        RpcSetID(myFaction);
    }
    /// <summary>
    /// Method <c>RpcSetID</c> sets up the game info for a bot controlled player
    /// string decklistName is the name of the deck the bot will use
    /// </summary>
    [ClientRpc]
    public override void RpcSetID(Faction playerFaction)
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        GameManager.addPlayer(gameObject.name,true);
        MyID = GameManager.PlayerID;
        GameManager.PlayerID++;
        string BotName = "Bot" + MyID;
        gameObject.name = BotName;
        GameManager.PlayerList[MyID] = BotName;
        GameManager.PlayerFactionList[MyID] = playerFaction;
        UIManager.UpdatePlayerText();
        GameManager.SelectDeck(playerFaction, MyID);
        haveDeck = true;
        Behavior = "HandAssessor";
        PlayerManager Human = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        if (Human.MyID == 0)
        {
            //   Human.DealID = MyID;
             Human.DealBotOpener(MyID);
            
        }

    }
    [ClientRpc]
    public void RpcTakeTurn()
    {
        TakeTurn();
    }
    /// <summary>
    /// Method <c>TakeTurn</c> powers the bot taking their turn
    ///RPC subcommand
    /// </summary>
    public void TakeTurn()
    {
        PlayerManager Human = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        if (Human.MyID != 0) { return; }//Only one person should be sending commands to play the bots cards
        switch (Behavior)
        {
            case "Passive":
                Human.DealBotCards(MyID);
                Invoke("Attack", .5f);

                break;
            case "EnergyGo":
                Human.DealBotCards(MyID);
                BotPlayEnergy();
                Invoke("Attack", .5f);
                break;
            case "RandomCreature":
                Human.DealBotCards(MyID);
                Invoke("BotPlayEnergy",.25f);
                Invoke("BotPlayMatter", .25f);
                Invoke("Attack", .5f);
                break;
            case "RandomComboandCreature":
                if (firstturn)
                {
                    Human.DealBotCards(MyID);
                    Human.DealBotCards(MyID);
                    Human.DealBotCards(MyID);
                    firstturn = false;
                }
                Invoke("BotPlayEnergy", .25f);
                Invoke("BotPlayCombo", .5f);
                Invoke("BotPlayMatter", .75f);
                Invoke("BotCastNontargetedSpell", 1f);
                Invoke("BotCastTargetedSpell", 1.25f);              
                Invoke("Attack", 1.5f);
                break;
                case ("HandAssessor"):
               
                Invoke("BotPlayEnergy", .1f);
                Invoke("AssessHand",.2f);
               
                break;

        }
    }
    public void Attack()
    {
        GameObject.Find("AttackButton").GetComponent<Attack>().DoAttack();
    }
    //Ok this is a bad variable name, but what's happening here is I want to not play a creature on the first go-thru of the hand because we Might play a Sweeper this turn.
    //After the first checking of the hand, we'll be sure we're not going to do that, so we'll play our targeted removal and creatures
    public bool MightSweep = true;
    public int ViableCards = 0;
    /// <summary>
    /// Method <c>AssessHand</c> is a simple flow of logic that checks if the bot can play any cards, plays any they can, and once they can't, has them pass their turn
    /// </summary>
    public void AssessHand()
    {
        bool breakword = false;
        PlayerManager Human = NetworkClient.connection.identity.GetComponent<PlayerManager>();
      
            foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
            {
                    CardDisplay _card = card.GetComponent<CardDisplay>();
                    if (_card.zone == "Hand" & _card.owner == MyID & _card.typeText.text!="Energy")
                    {
                        if(!breakword)
                        if (GameManager.HaveMana(int.Parse(_card.costText.text),MyID))
                            {
                                switch (_card.typeText.text.ToString())
                                {
                                    case "Energy":
                                        break;
                                    case "Matter":

                                        if (!MightSweep)
                                        {
                                            Human.PlayBotCard(card.name, _card.typeText.text.ToString(), MyID);
                                             breakword = true;
                                        }

                                        break;
                                    case "Combo":
                                        Human.PlayBotCard(card.name, _card.typeText.text.ToString(), MyID);
                                        breakword = true;
                                        break;
                                    case "Spell":
                                        if (_card.Abilities.Contains(Ability.Sweeper) ^ _card.Abilities.Contains(Ability.Edict) ^ _card.Abilities.Contains(Ability.Trial))
                                        {
                                            if (MightSweep)
                                            {
                                                Human.PlayBotCard(card.name, _card.typeText.text.ToString(), MyID);
                                                MightSweep = false;
                                                breakword = true;
                                            }

                                        }
                                        else if (!MightSweep)
                                        {
                                    
                                            if (_card.Abilities[0] == Ability.Targeted)
                                            {
                                                Human.PlayBotTargetCard(card.name, FindBigTarget());
                                                breakword = true;
                                            }
                                            else
                                            {
                                                
                                                Human.PlayBotCard(card.name, _card.typeText.text.ToString(), MyID);
                                                breakword = true;
                                            }
                                        }


                                        break;
                                }
                            }
            
                    }
            }
        MightSweep = false;
        int ViableCardsThisLoop = 0;
        foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardDisplay _card = card.GetComponent<CardDisplay>();
            if (_card.zone == "Hand" & _card.owner == MyID & _card.typeText.text != "Energy")
            {
                if (int.Parse(_card.costText.text) <= GameManager.currentManaList[MyID])
                {
                    ViableCardsThisLoop++;
                }
            }
        }
        if (ViableCardsThisLoop!= ViableCards)
        {
            ViableCards = ViableCardsThisLoop;
            Invoke("TakeTurn", .1f);
       
        }
        else
        {
            ViableCards = 0;
            Invoke("Attack", .5f);

        }

    }
    /// <summary>
    /// Method <c>BotPlayEnergy</c> lets a bot play an energy card
    /// </summary>
    public void BotPlayEnergy()
    {
        PlayerManager Human = NetworkClient.connection.identity.GetComponent<PlayerManager>();

        foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardDisplay _card = card.GetComponent<CardDisplay>();
            if (_card.zone == "Hand" & _card.typeText.text.ToString() == "Energy" & _card.owner == MyID)
            {

                Human.PlayBotCard(card.name, _card.typeText.text.ToString(),MyID);
                return;
            }
        }
    }
    /// <summary>
    /// Method <c>BotPlayMatter</c> lets a bot play a matter card
    /// </summary>
    public void BotPlayMatter()
    {
        PlayerManager Human = NetworkClient.connection.identity.GetComponent<PlayerManager>();

        foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardDisplay _card = card.GetComponent<CardDisplay>();
            if (_card.zone == "Hand" & _card.typeText.text.ToString() == "Matter" & _card.owner == MyID)
            {
                if (int.Parse(_card.costText.text) <= GameManager.currentManaList[MyID])
                {
                    Human.PlayBotCard(card.name, _card.typeText.text.ToString(), MyID);
                    return;
                }
            }
        }
    }
    /// <summary>
    /// Method <c>BotPlayCombo</c> lets a bot play a combo card
    /// </summary>
    public void BotPlayCombo()
    {
        PlayerManager Human = NetworkClient.connection.identity.GetComponent<PlayerManager>();

        foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardDisplay _card = card.GetComponent<CardDisplay>();
            if (_card.zone == "Hand" & _card.typeText.text.ToString() == "Combo" & _card.owner == MyID)
            {
                if (int.Parse(_card.costText.text) <= GameManager.currentManaList[MyID])
                {
                    Human.PlayBotCard(card.name, _card.typeText.text.ToString(), MyID);
                    return;
                }
            }
        }
    }
    /// <summary>
    /// Method <c>BotCastNontargetedSpell</c> lets a bot play a nontargeted spell
    /// </summary>
    public void BotCastNontargetedSpell()
    {
        PlayerManager Human = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardDisplay _card = card.GetComponent<CardDisplay>();
            if (_card.zone == "Hand" & _card.typeText.text.ToString() == "Spell" & _card.owner == MyID & _card.Abilities[0]!= Ability.Targeted)
            {
                if (int.Parse(_card.costText.text) <= GameManager.currentManaList[MyID])
                {
                    Human.PlayBotCard(card.name, _card.typeText.text.ToString(), MyID);
                    return;
                }
            }
        }
    }
    /// <summary>
    /// Method <c>FindBigTarget</c> finds the highest total power and toughness on an enemy creature
    /// return string is the gameObject name of that card
    /// </summary>
    public string FindBigTarget()
    {
        string target = "None";
        int bigValue = -1;
        foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardDisplay _card = card.GetComponent<CardDisplay>();
            if (_card.zone == "Board" & _card.typeText.text.ToString() == "Matter" & _card.owner != MyID)
            {
                if (int.Parse(_card.attackText.text) + int.Parse(_card.defenseText.text) >= bigValue)
                {
                    bigValue = int.Parse(_card.attackText.text) + int.Parse(_card.defenseText.text);
                    target = card.name;
                }
            }

        }
        return target;
    }
    /// <summary>
    /// Method <c>BotCastTargetedSpell</c> lets the bot play a targeted spell
    /// </summary>
    public void BotCastTargetedSpell()
    {

        PlayerManager Human = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardDisplay _card = card.GetComponent<CardDisplay>();
            if (_card.zone == "Hand" & _card.typeText.text.ToString() == "Spell" & _card.owner == MyID )
            {
                if (_card.Abilities[0] == Ability.Targeted)
                {
                    if (int.Parse(_card.costText.text) <= GameManager.currentManaList[MyID])
                    {
                        Human.PlayBotTargetCard(card.name, FindBigTarget());
                    }
                }
            }
        }
    }
}
