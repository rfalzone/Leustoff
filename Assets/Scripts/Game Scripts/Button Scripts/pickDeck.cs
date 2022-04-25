using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System;
using System.Threading;
using LeustoffNamespace;
public class pickDeck : NetworkBehaviour
{
    public PlayerManager PlayerManager;
    public Faction myFaction ;
    public bool clicked = false;
    private void Start()
    {
        string decklistName = FactionNamer(myFaction);


        gameObject.transform.GetChild(0).GetComponent<Text>().text = "Pick " + decklistName;

    }


    public void OnClick()
    {
        if (clicked) { return; }
        clicked = true;
        string decklistName = FactionNamer(myFaction);
        PlayerManager = GameObject.Find("Background").GetComponent<PMIdentityObject>().PlayerManager;
        PlayerManager.CMDSetID(myFaction);
        
        if (!GameObject.Find("GameManager").GetComponent<GameManager>().isMP)
        {
            BotButton bots = GameObject.Find("BotMatch").GetComponent<BotButton>();
            bots.Factions.Remove(myFaction);
            bots.Startmatch();
        }
        
    }



    public string FactionNamer(Faction myFaction)
    {
        switch (myFaction)
        {
            case Faction.Materials:
                return "Materials";
                break;
            case Faction.Planets:
                return "Planets";
                break;
            case Faction.Biomes:
                return "Biomes";
                break;
            case Faction.Media:
                return "Media";
                break;
            default:
                return "NoFaction";
                    break;
        }
      
    }
}

    



