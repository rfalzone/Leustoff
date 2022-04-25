using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using LeustoffNamespace;
public class BotButton : NetworkBehaviour
{

    public GameObject Bot;
    public int i = 0;
    public List<Faction> Factions = new List<Faction> {};
    public PlayerManager PlayerManager;
    /// <summary>
    /// Method <c>Start</c> populates the list that the bots pick their decks from
    /// </summary>
    public void Start()
    {
        Factions.Clear();
        Factions.Add(Faction.Materials);
        Factions.Add(Faction.Media);
        Factions.Add(Faction.Biomes);
        Factions.Add(Faction.Planets);
    }
    public void Startmatch()
    {
        PlayerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();

        Invoke("OnClick", .5f);
    }
    /// <summary>
    /// Method <c>OnClick</c> creates three AI controlled bots for the player to play the game against
    /// </summary>
    public void OnClick()
    {
      
        GameObject NewBot = Instantiate(Bot, new Vector2(0, 0), Quaternion.identity);
                NetworkServer.Spawn(NewBot);
        Faction myfaction = Factions[UnityEngine.Random.Range(0, Factions.Count)];
        Factions.Remove(myfaction);
        PlayerManager.CmdSetBotID(myfaction, NewBot.name);
        i++;
        if (i < 3)
        {
            Invoke("OnClick", .1f);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
