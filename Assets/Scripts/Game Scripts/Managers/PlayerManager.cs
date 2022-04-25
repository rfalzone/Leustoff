using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Text.RegularExpressions;
using LeustoffNamespace;
using TMPro;

/// <summary>
/// Welcome to my code, start here or the GameManager class if you'd like to see how the game works.
/// A note on terms:
/// Many phrases are used interchangably in documentation
/// Mana and energy
/// Creature and matter
/// tax, penalty card, and entropy
/// attack and power
/// defense and toughness
/// damage (to opponents) and revenue
/// </summary>
public class PlayerManager : NetworkBehaviour
{
    public GameManager GameManager;
    public GameObject CardMain;
    public GameObject PlayerArea;
    public GameObject EnemyArea;
    public GameObject PlayerYard;
    public GameObject EnemyYard;
    public GameObject Canvas;
    public GameObject PlayerBoard;
    public GameObject EnemyBoard;
    public Deck masterCardList;
    public GameObject zoomCard;
    public int zoomfactor = 0;
    public LeustoffCard newcard;
    private GameObject card;
    public GameObject EnergyZone;
    public UIManager UIManager;
    public Image image;
    public Deck mydeck;
    public deckListList DeckListList;
    public bool haveDeck = false;
    public GameObject DrawButton;
    public GameObject SpellArea;
    public GameObject PlayerComboZone;
    public GameObject EnemyComboZone;
    public GameObject EndGameScreen;
    public SpellManager SpellManager;
    public int MyID;
    public string DragDropParent;
    public int MaxCarddraw;
    public List<string> Hands = new List<string> { "PlayerArea", "EnemyArea0", "EnemyArea1", "EnemyArea2" };
    public List<string> Boards = new List<string> { "PlayerBoard", "EnemyBoard0", "EnemyBoard1", "EnemyBoard2" };
    public List<string> ComboZones = new List<String> { "PlayerComboZone", "EnemyComboZone0", "EnemyComboZone1", "EnemyComboZone2" };
    public List<string> Yards = new List<String> { "PlayerYard", "EnemyYard0", "EnemyYard1", "EnemyYard2" };
    /// <summary>
    /// Method <c>OnStartClient</c> is called automatically at start, and finds gameobjects in the scene.
    /// </summary>
    public override void OnStartClient()
    {
        MyID = -1;
        base.OnStartClient();
        PlayerArea = GameObject.Find("PlayerArea");
        Canvas = GameObject.Find("Main Canvas");
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        PlayerArea = GameObject.Find("PlayerArea");
        EnemyArea = GameObject.Find("EnemyArea");
        PlayerYard = GameObject.Find("PlayerYard");
        EnemyYard = GameObject.Find("EnemyYard");
        PlayerBoard = GameObject.Find("PlayerBoard");
        EnemyBoard = GameObject.Find("EnemyBoard");
        SpellArea = GameObject.Find("SpellArea");
        EndGameScreen = GameObject.Find("EndGameScreen");
        GameObject.Find("Background").GetComponent<PMIdentityObject>().PlayerManager = this;
    }

    /// <summary>
    /// Method <c>SetMeUp</c> takes a name of a deck and tells the server to set up this player object's deck and ID.
    /// string decklistName is the decklist name to send to the server
    /// </summary>
    public void SetMeUp(Faction myFaction)
    {
        CMDSetID(myFaction);
    }
    /// <summary>
    /// Method <c>CMDSetID</c> takes a name of a deck and tells the server to set up this player object's deck and ID.
    /// string decklistName is the deck sent from the server to the clients
    /// </summary>
    [Command]
    public virtual void CMDSetID(Faction myFaction)
    {

        RpcSetID(myFaction);
    }
    /// <summary>
    /// Method <c>RpcSetID</c> tells each client the new player's ID and decklist.
    /// The fuction to draw an opening hand is invoked here.
    /// string decklistName is the faction name saved to the GameManager and the deck selected
    /// </summary>
    [ClientRpc]
    public virtual void RpcSetID(Faction playerFaction)
    {
        
        EndGameScreen.SetActive(false);
        
        PlayerManager PM = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        
        GameManager.addPlayer(gameObject.name,false);
        MyID = GameManager.PlayerID;
        int CherrySoup = MyID;
        GameManager.PlayerID++;
        
        GameManager.PlayerFactionList[CherrySoup] = playerFaction;

        
        UIManager.UpdatePlayerText();
        
     
        
        if (PM.MyID == 0)
        {
            SelectDeck(playerFaction, MyID);
            //    DealID = CherrySoup;
            StartCoroutine(DealOpener(CherrySoup));
        }
        
    }
    public int DealID;
    /// <summary>
    /// Method <c>DealOpener</c> draws the opening hand of four cards.
    /// </summary>
    public IEnumerator DealOpener(int DealID)
    {
        yield return new WaitForSeconds(.1f);
        CmdDrawCard(DealID);
        yield return new WaitForSeconds(.1f);
        CmdDrawCard(DealID);
        yield return new WaitForSeconds(.1f);
        CmdDrawCard(DealID);
        yield return new WaitForSeconds(.1f);
        CmdDrawCard(DealID);
        /*
        System.Threading.Timer timer = null;
     
            timer = new System.Threading.Timer((obj) =>
            {
                CmdDrawCard(DealID);
                timer.Dispose();
            },
                        null, 100, System.Threading.Timeout.Infinite);

        timer = new System.Threading.Timer((obj) =>
        {
            CmdDrawCard(DealID);
            timer.Dispose();
        },
                    null, 200, System.Threading.Timeout.Infinite);

        timer = new System.Threading.Timer((obj) =>
        {
            CmdDrawCard(DealID);
            timer.Dispose();
        },
                   null, 300, System.Threading.Timeout.Infinite);
        timer = new System.Threading.Timer((obj) =>
        {
            CmdDrawCard(DealID);
            timer.Dispose();
        },
                   null, 400, System.Threading.Timeout.Infinite);
        timer = new System.Threading.Timer((obj) =>
        {
            CmdDrawCard(DealID);
            timer.Dispose();
        },
              null, 500, System.Threading.Timeout.Infinite);
*/
    }

    /// <summary>
    /// Method <c>DealBotOpener</c> is called during the bot setup routine to give the bots opening hands.
    /// </summary>
    public void DealBotOpener(int botID)
    {
        StartCoroutine(DealOpener(botID));
    }

    /// <summary>
    /// Method <c>DealBotCards</c> is called to give a player with the given ID a card.  Used to let bots draw cards.
    /// This is called locally and invokes a command to the server.
    /// int PlayerID is the ID of the bot that will draw cards
    /// </summary>
    public void DealBotCards(int PlayerID)
    {
        CmdDrawCard(PlayerID);
    }

    /// <summary>
    /// Method <c>DealCards</c> is the first step after the draw cards button is pressed.
    /// Catches if the player hasn't yet selected a deck.
    /// </summary>
    public void DealCards()
    {
        if (haveDeck)
        {
            CmdDrawCard(MyID);
        }
        else
        {
            Debug.Log("Go pick a deck first");
        }
    }
    /// <summary>
    /// Method <c>DealACard</c> gives a player with the given ID a card.
    /// Rpc Subcommand
    /// int PlayerID is the ID of the player that will draw a card
    /// </summary>
    public void DealACard(int PlayerID)
    {
        if (NetworkClient.connection.identity.GetComponent<PlayerManager>().MyID == 0)
        {
            CmdDrawCard(PlayerID);
        }
       


    }
    /// <summary>
    /// Method <c>CmdDrawCard</c> tells the server to give the player card.
    /// int PlayerID is the ID of the player that will draw a card
    /// </summary>
    [Command]
    public void CmdDrawCard(int PlayerID)
    {
        GameManager.PlayerManager = this;

        GameManager.DrawCard(PlayerID);
        UIManager.RpcUpdateDeckcounter();
    }
    [Command]
    public void CmdTrueDrawCard(int PlayerID)
    {
        GameManager.PlayerManager = this;

        GameManager.TrueDrawCard(PlayerID);
        UIManager.RpcUpdateDeckcounter();
    }
    /// <summary>
    /// Method <c>ShowCard</c> spawns a blank card on the server, then tells clients to populate the card's info locally.
    /// Command Subcommand
    /// string cardName is the name of the card to create
    /// int PlayerID is the player who drew that card
    /// </summary>
    public void ShowCard(string cardName, int PlayerID)
    {
        GameObject cardObj = Instantiate(CardMain, new Vector2(0, 0), Quaternion.identity);
         NetworkServer.Spawn(cardObj);
        freshcard = cardObj.name;
        NewPlayerDrawingID = PlayerID;
            newcardname = cardName;
 
        RpcShowCard(cardObj.name, PlayerID, cardName);

    }
    /// <summary>
    /// Method <c>RpcShowCard</c> creates new card objects and adds them to a player's hand.
    /// string cardobj is the ID of the card object in the scene
    /// int PlayerID is the player who controls the card
    /// string cardName is the game name of the card to generate the cards stats
    /// </summary>
    public string freshcard;
    public int NewPlayerDrawingID;
    public string newcardname;
    [ClientRpc]
    public void RpcShowCard(string cardobj, int PlayerID, string cardName)
    {
     
        
        GameObject cardObj = GameObject.Find(cardobj);
        
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        int localID = NetworkClient.connection.identity.GetComponent<PlayerManager>().MyID;
        //search the deck for the card name and grab it
        string ID = GameManager.ID.ToString();
        cardObj.name = ID;
        //build a new card
        
         LeustoffCard cardData = masterCardList.masterCards.Find(x => x.cardName == cardName);
         CardDisplay _card = cardObj.GetComponent<CardDisplay>();
         _card.owner = PlayerID;
         _card.UpdateCard(cardData);
         _card.zone = "Hand";
         GameManager.ID++;
         GameObject TargetHand = GameObject.Find(Hands[ZoneSorterOuter(PlayerID)]);
         cardObj.transform.SetParent(TargetHand.transform, false);
         ///WILL BREAK WITH BOT I THINK
         
         if (localID == PlayerID)
         {
             _card.GainControl();
         }

         else
         {
             _card.FlipFaceDown();
         }
         _card.UpdateHandStatus();
         
    }
    /// <summary>
    /// Method <c>CMDSpawnCard</c> spawns the card on the server.
    /// string ID is the unique name of the card object in the scene
    /// </summary>
    [Command]
    public void CMDSpawnCard(string ID)
    {
        NetworkServer.Spawn(GameObject.Find(ID));
    }
    /// <summary>
    /// Method <c>GrantPayout</c> is called when the player reaches five AP.
    /// Draws a card if below max hand size (5)
    /// Gives a dividend if at max hand size.
    /// int PlayerID is the ID of the player gaining a payout
    /// RPC subfunction
    /// </summary>
    public void GrantPayout(int PlayerID)
    {
        PayoutAnim(PlayerID);      
        GameObject hi = GameObject.Find(Hands[ZoneSorterOuter(PlayerID)]);
        if (hi.transform.childCount >= 5)
        {
         
                GameManager.GrantDividend(PlayerID);
               
    } 
        else
        {   if (MaxCarddraw > 0)
            {
                DealACard(PlayerID);
                MaxCarddraw--;
            }
            else
            {
                GameManager.GrantDividend(PlayerID);
            }
        }
    }

    /// <summary>
    /// Method <c>PayoutAnim</c> triggers the payout animation on the appropriate hand.
    /// RPC subfunction
    /// int PlayerID is ID of the player who got a payout
    /// </summary>
    public void PayoutAnim(int PlayerID)
    {
     
        GameObject hi = GameObject.Find(Hands[ZoneSorterOuter(PlayerID)]);
        hi.GetComponent<PlayerAreaAnimManager>().GainedPayout();
    }
    /// <summary>
    /// Method <c>TaxAnim</c> triggers the penalty animation on the appropriate hand.
    /// RPC subfunction
    /// int PlayerID is ID of the player who got a penalty card
    /// </summary>
    public void TaxAnim(int PlayerID)
    {
       
        GameObject Hi = GameObject.Find(Hands[ZoneSorterOuter(PlayerID)]);
        Hi.GetComponent<PlayerAreaAnimManager>().TookTax();
    }

    private string GMResponse;
    private int PlayerID;
    public string currentSpell;
    public void PlayCard(string ID)
    {
        CmdPlayCard(ID, MyID);
    }
    public void PlayBotCard(string ID, string Type, int BotID)
    {
        CmdPlayCard(ID, BotID);
    }
    public void PlayBotTargetCard(string ID, string TargetID)
    {
        if (TargetID != "None")
        {
            CmdBotTargetSpell(ID, TargetID);
        }
    }
    /// <summary>
    /// Method <c>CmdPlayCard</c> handles cards being played
    /// Deducts mana, then triggers appropriate function given card type.
    /// string ID is the unique gameobject name of the card being played
    /// string Type is the card type of the card
    /// int PlayerID is the ID of the player who is playing the card
    /// </summary>
    [Command]
    public void CmdPlayCard(string ID, int PlayerID)
    {
        
        card = GameObject.Find(ID);
        int cost=0;
        CardDisplay _card = card.GetComponent<CardDisplay>();
        cardType cardType = _card.myType;
        if (!string.IsNullOrEmpty(_card.costText.text.ToString()))
        {
              cost = int.Parse(_card.costText.text);

        }
        
        if (spellcasting)
        {
            card.GetComponent<DragDrop>().RpcReturnToSender(PlayerID);
            return;
        }
        switch (cardType)
        {
            case cardType.Energy:
                if (GameManager.EnergyPlayForTurn[PlayerID] > 0)
                {
                    RpcTableEnergyCard(ID, PlayerID);
                }
                else
                {
                    card.GetComponent<DragDrop>().RpcReturnToSender(PlayerID);
                }

                break;


            case cardType.Matter:

             
                    if (GameManager.HaveMana(cost, PlayerID))
                    {
                        GameManager.RpcPayMana(cost, PlayerID);
                        CastCreature(ID, PlayerID);
                    }
                    else
                    {

                        card.GetComponent<DragDrop>().RpcReturnToSender(PlayerID);
                    }
              
                break;
            case cardType.Spell:
               
                    currentCost = cost;
                    if (_card.Abilities.Contains(Ability.Counterspell))
                    {
                        card.GetComponent<DragDrop>().RpcReturnToSender(PlayerID);
                    }
                    else if (GameManager.HaveMana(cost, PlayerID))
                    {
                        GameManager.RpcPayMana(cost, PlayerID);
                        if (_card.Abilities[0] == Ability.Targeted)
                        {
                        
                            RpcCastTargetedSpell(ID, PlayerID);

                        }

                        else
                        {

                            RpcResolveSpell(PlayerID, ID);
                        }
                    }
                    else
                    {

                        card.GetComponent<DragDrop>().RpcReturnToSender(PlayerID);
                    }
                
                break;
            case cardType.Combo:
              
                    if (GameManager.HaveMana(cost, PlayerID))
                    {
                        GameManager.RpcPayMana(cost, PlayerID);

                   
                    FinishCastingCombo(ID);
                    }
                    else
                    {

                        card.GetComponent<DragDrop>().RpcReturnToSender(PlayerID);
                    }

                
                break;
            default:
                Debug.Log("Bad Card Type");
                break;
        }

    }

    [Command]
    public void CmdPayMana(int currentCost, int PlayerID)
    {
        GameManager.RpcPayMana(currentCost, PlayerID);
    }


    [Command]
    public void CmdUnPayMana(int PlayerID)
    {
        GameManager.RpcUnPayMana(PlayerID);
    }
    //Counterspells not currently in game, leaving in case reimplemented later.
    [Command]
    public void CmdCastCounter(string ID, string csID)
    {
        CardDisplay _counterCard = GameObject.Find(csID).GetComponent<CardDisplay>();
        GameManager.RpcPayMana(int.Parse(_counterCard.costText.text), _counterCard.owner);
        RpcCastCounter(ID, csID);
    }
    [ClientRpc]
    public void RpcCastCounter(string ID, string csID)
    {
        CardDisplay _comboCard = GameObject.Find(ID).GetComponent<CardDisplay>();
        CardDisplay _counterCard = GameObject.Find(csID).GetComponent<CardDisplay>();
        _comboCard.Graveyard();
        GameManager.DisplayEventText("Counterspell");
        _counterCard.Graveyard();
        _comboCard.isCountered = true;
        GameObject.Find(ID).GetComponent<CardDisplay>().Graveyard();
    }
    /// <summary>
    /// Method <c>CalculateComboPower</c> sums up everyone's total combo points.
    /// Command subcommand.
    /// int owner is the ID of the player who played a combo card
    /// </summary>
    public void CalculateComboPower(int owner)
    {
        for (int i = 0; i < GameManager.playerComboPowerList.Count; i++)
        {
            GameManager.playerComboPowerList[i] = 0;
        }
        foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardDisplay _card = card.GetComponent<CardDisplay>();
            if (_card.zone == "Board" & _card.typeText.text == "Combo")
            {
                GameManager.playerComboPowerList[_card.owner] += int.Parse(_card.attackText.text);
            }
        }
        for(int i=0;i<GameManager.PlayerID;i++) 
        {
            GameManager.playerComboPowerList[i] -= GameManager.playerComboDamageList[i];
        }
    }
    /// <summary>
    /// Method <c>FinishCastingCombo</c> adds combo points, triggers game end or power moves.
    /// Command subcommand.
    /// string ID is the gameObject name of the combo card being played
    /// </summary>
    public void FinishCastingCombo(string ID)
    {
        GameObject card = GameObject.Find(ID);
        CardDisplay _card = card.GetComponent<CardDisplay>();
        ServerTableCombo(ID);
        CalculateComboPower(_card.owner);
        if (GameManager.playerComboPowerList[_card.owner] >= 7)
        {
            foreach (GameObject zcard in GameObject.FindGameObjectsWithTag("Card"))
            {
                CardDisplay _zcard = zcard.GetComponent<CardDisplay>();
                if (_zcard.zone == "Board" & _zcard.typeText.text.ToString() == "Combo" & _zcard.owner == _card.owner)
                {
                   RpcGraveyard(zcard.name);
                }

            }
            CalculateComboPower(_card.owner);
            GameManager.PlayerScoreList[_card.owner]++;
            if (GameManager.PlayerScoreList[_card.owner] == 2)
            {
                RpcYouWin(_card.owner);
       
            }
            else
            {
                RpcPowerMove(_card.owner);
                GameManager.playerComboDamageList[_card.owner] = 0;
            }
        }

        _card.cETB();
        CalculateComboPower(_card.owner);

        UIManager.RpcDisplayPointsSums();
    }
    [ClientRpc]
    public void RpcYouWin(int PlayerID)
    {
        EndGameScreen.SetActive(true);
        GameObject WinnerText = GameObject.Find("Winnertext");
        WinnerText.GetComponent<TextMeshProUGUI>().text = "Player " + PlayerID + " wins!";
        if(PlayerID== NetworkClient.connection.identity.GetComponent<PlayerManager>().MyID)
        {
            WinnerText.GetComponent<TextMeshProUGUI>().text = "You win!";

        }
    }
    /// <summary>
    /// Method <c>ServerTableCombo</c> puts combo cards on the board on the server, then tells clients to do the same.
    /// This is handled on the server before the clients so the server can calculate the total combo points immediately.
    /// Command subcommand.
    /// string ID is the gameObject name of the combo card being played
    /// </summary>
    public void ServerTableCombo(string ID)
    {

        GameObject card = GameObject.Find(ID);
        CardDisplay _card = card.GetComponent<CardDisplay>();

        int zonenumber = ZoneSorterOuter(_card.owner);
        _card.zone = "Board";
        GameObject TargetZone = GameObject.Find(ComboZones[zonenumber]);

        card.transform.SetParent(TargetZone.transform, false);
        RPCPutCombo(ID);
    }
    /// <summary>
    /// Method <c>RPCPutCombo</c> puts combo cards on the board for clients.
    /// string ID is the gameObject name of the combo card being played
    /// </summary>
    [ClientRpc]
    public void RPCPutCombo(string ID)
    {
        GameObject card = GameObject.Find(ID);
        CardDisplay _card = card.GetComponent<CardDisplay>();

        int zonenumber = ZoneSorterOuter(_card.owner);
        _card.zone = "Board";
        GameObject TargetZone = GameObject.Find(ComboZones[zonenumber]);
        _card.FlipFaceUp();
        card.transform.SetParent(TargetZone.transform, false);
        card.transform.rotation = TargetZone.transform.rotation;
       // _card.ShrinkForComboZone();
    }
    
    /// <summary>
    /// Method <c>RpcPowerMove</c> checks the player's faction, then executes a function to do that faction's power move
    /// int PlayerID is the ID of the player doing a power move
    /// </summary>
    [ClientRpc]
    public void RpcPowerMove(int PlayerID)
    {
            Faction _Faction = GameManager.PlayerFactionList[PlayerID];
            switch (_Faction)
            {
                case Faction.Materials:
                    MaterialsPowerMove(PlayerID);
                    break;
                case Faction.Planets:
                    PlanetsPowerMove(PlayerID);
                    break;
                case Faction.Media:
                    MediaPowerMove(PlayerID);
                    break;
                case Faction.Biomes:
                     BiomesPowerMove(PlayerID);
                break;
                default:
                    Debug.Log("Faction not recognized");
                    break;
            }
    }
    /// <summary>
    /// Method <c>MaterialsPowerMove</c> refils the player's hand.
    /// int PlayerID is the ID of the player doing a power move
    /// RPC subcommand
    /// </summary>
    public void MaterialsPowerMove(int PlayerID)
    {
        Debug.Log("Materials Power move, refil hand");
        
            int shiboleth = 5 - GameObject.Find(Hands[ZoneSorterOuter(PlayerID)]).transform.childCount;
            int sargoth = 5 - shiboleth;
            for (int i = 0; i < shiboleth; i++)
            {
               DealACard(PlayerID);

            }
            for (int i = 0; i < sargoth; i++)
            {
              GameManager.GrantDividend(PlayerID);
            }
        
    }
    /// <summary>
    /// Method <c>BiomesPowerMove</c> grants a second turn.
    /// int PlayerID is the ID of the player doing a power move
    /// RPC subfuction
    /// </summary>
    public void BiomesPowerMove(int PlayerID)
    {
        Debug.Log("Biomes Power move, extra turn");

        GameManager.TimeWalk = true;

    }
    /// <summary>
    /// Method <c>PlanetsPowerMove</c> destroys all enemy creatures.
    /// int PlayerID is the ID of the player doing a power move
    /// RPC subfuction
    /// </summary>
    public void PlanetsPowerMove(int PlayerID)
    {

        Debug.Log("Planets Power Move, one sided boardwipe");
        GameManager.PlagueWind(PlayerID);

    }
    /// <summary>
    /// Method <c>MediaPowerMove</c> doubles mana crystals.
    /// int PlayerID is the ID of the player doing a power move
    /// RPC subfuction
    /// </summary>
    public void MediaPowerMove(int PlayerID)
    {
        Debug.Log("Media Power Move, double mana");
        int shiboleth = GameManager.playerManaList[PlayerID];
        GameManager.currentManaList[PlayerID] += shiboleth;
        int rancor = GameManager.playerManaList[PlayerID];
        GameManager.playerManaList[PlayerID] += rancor;
        UIManager.UpdatePlayerText();
    }


    public GameObject HitMarker;
    public bool spellcasting = false;
    /// <summary>
    /// Method <c>RpcCastTargetedSpell</c>  starts the targeting process.  The spell is placed in a temporary area called the stack.
    /// The spellcasting bool is set to true, which changes card objects OnClick() behavior.
    /// int PlayerID is the ID of the player casting the spell
    /// string ID is the unique gameObject name of the card being cast
    /// </summary>
    [ClientRpc]
    public void RpcCastTargetedSpell(string ID, int PlayerID)
    {
        card = GameObject.Find(ID);
        CardDisplay _card = card.GetComponent<CardDisplay>();
        currentSpell = ID;
        if (_card.owner == PlayerID)
        {
            SpellArea = GameObject.Find("SpellArea");
            card.GetComponent<DragDrop>().beingCast = true;
            spellcasting = true;
            _card.zone = "Stack";
            card.transform.SetParent(SpellArea.transform, false);
        }
    }
    /// <summary>
    /// Method <c>ResolveSpell</c> finishes the targeted spellcasting process.
    /// Called by the OnClick() func of card objects while spellcasting bool is true
    /// int PlayerID is the ID of the player casting the spell
    /// string ID is the unique gameObject name of the card being targeted
    /// </summary>
    public void ResolveSpell(string targetID,int PlayerID)
    {
        CardDisplay _card = GameObject.Find(currentSpell).GetComponent<CardDisplay>();
        GameObject.Find(currentSpell).GetComponent<DragDrop>().beingCast = false;
        UIManager.UpdatePlayerText();
        

            SpellManager = GameObject.Find("SpellManager").GetComponent<SpellManager>();
            SpellManager.Cast(currentSpell, targetID);
            CmdGraveyard(currentSpell);
            spellcasting = false;
        

    }

    [Command]
    public void CmdBotTargetSpell(string ID, string TargetID)
    {
        RpcResolveBotTargetedSpell(ID, TargetID);
    }
    /// <summary>
    /// Method <c>RpcResolveBotTargetedSpell</c> is how bots cast and resolve targeted spells
    /// string ID is the unique gameObject name of the card being cast
    /// string TargetID is the unique gameObject name of the card being targeted
    /// </summary>
    [ClientRpc]
    public void RpcResolveBotTargetedSpell(string ID, string TargetID)
    {
        SpellManager = GameObject.Find("SpellManager").GetComponent<SpellManager>();
        SpellManager.Cast(ID, TargetID);
        GameObject.Find(ID).GetComponent<CardDisplay>().Graveyard();
    }
    /// <summary>
    /// Method <c>RpcResolveSpell</c> resolves an untargeted spell.
    /// int PlayerID is the ID of the player casting the spell
    /// string SpellID is the unique gameObject name of the card being cast
    /// </summary>
    [ClientRpc]
    public void RpcResolveSpell(int PlayerID, string SpellID)
    {
        int localID = NetworkClient.connection.identity.GetComponent<PlayerManager>().MyID;
        CardDisplay _card = GameObject.Find(SpellID).GetComponent<CardDisplay>();
        if (0 == localID)
        {
            UIManager.UpdatePlayerText();
            SpellManager = GameObject.Find("SpellManager").GetComponent<SpellManager>();
            SpellManager.Cast(SpellID, "NoTarget");
            CmdGraveyard(SpellID);
            spellcasting = false;
        }

    }
    /// <summary>
    /// Method <c>ZoneSorterOuter</c> takes the 'target' player's ID and returns an int that corresponds to which 'seat' at the table is relevant.
    /// IE: You are 0, the player playing after you is 1, and so on.
    /// </summary>
    public int ZoneSorterOuter(int PlayerID)
    {
        int localID = NetworkClient.connection.identity.GetComponent<PlayerManager>().MyID;
        var gribbit = GameManager.PlayerID - localID;
        var gaboodle = gribbit + PlayerID;
        var grobbit = Util.mod(gaboodle, GameManager.PlayerID);
        return grobbit;
    }
    public string cardname;
    public int currentCost;
  
    /// <summary>
    /// Method <c>RpcTableCard</c> puts the card on the board, flips it up, triggers enter play effects.
    /// string ID is the unique gameObject name of the card being played
    /// int PlayerID is the ID of the plyaer playing the card
    /// </summary>
    [ClientRpc]
    void RpcTableCard(string ID, int PlayerID)
    {
        card = GameObject.Find(ID);
        CardDisplay _card = card.GetComponent<CardDisplay>();
        _card.zone = "Board";
        _card.HighLightSummoningSick();
        int targetZone = ZoneSorterOuter(PlayerID);
        GameObject TargetZone = GameObject.Find(Boards[targetZone]);
        card.transform.SetParent(TargetZone.transform, false);
        card.transform.rotation = TargetZone.transform.rotation;
        _card.FlipFaceUp();
        _card.CheckOverflow(_card.owner);
        GameManager.GetTotalPTs();

        UIManager.UpdatePlayerText();

    }
    /// <summary>
    /// Method <c>CastCreature</c> puts the card on the board, flips it up, triggers enter play effects on the server, then tells clients to do the same.
    /// int PlayerID is the ID of the player playing the card
    /// Command subcommand
    /// </summary>
    void CastCreature(string ID, int PlayerID)
    {

        card = GameObject.Find(ID);
        CardDisplay _card = card.GetComponent<CardDisplay>();
        _card.zone = "Board";
        _card.HighLightSummoningSick();
        int targetZone = ZoneSorterOuter(PlayerID);
        GameObject TargetZone = GameObject.Find(Boards[targetZone]);
        card.transform.SetParent(TargetZone.transform, false);
        _card.cETB();
        RpcTableCard(ID, PlayerID);
    }
    /// <summary>
    /// Method <c>SpawnToken</c> sends the SpawnToken effect to the server.
    ///Command subcommand
    ///string cardName is the game name of the token being spawned
    ///int owner is the ID of the player spawning a token
    /// </summary>
    public void _SpawnToken(string cardName, int owner)
    {

        
            GameObject cardObj = Instantiate(CardMain, new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(cardObj);
            LeustoffCard cardData = masterCardList.masterCards.Find(x => x.cardName == cardName);
            CardDisplay _card = cardObj.GetComponent<CardDisplay>();
            _card.owner = PlayerID;
            _card.UpdateCard(cardData);
            _card.zone = "Board";
            _card.HighLightSummoningSick();
            int targetZone = ZoneSorterOuter(PlayerID);
            GameObject TargetZone = GameObject.Find(Boards[targetZone]);
            card.transform.SetParent(TargetZone.transform, false);
            _card.owner = PlayerID;
            _card.cETB();
            RpcSpawnToken(cardObj.name, cardName, owner);
 
    }
    /// <summary>
    /// Method <c>RpcSpawnToken</c> finds the empty token spawned by the server and populates it with its stats.
    ///string ID is the gameObjects name
    ///string cardName is the game name of the token
    ///int PlayerID is the token's controller
    /// </summary>
    [ClientRpc]
    public void RpcSpawnToken(string ID, string cardName, int PlayerID)
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        int localID = NetworkClient.connection.identity.GetComponent<PlayerManager>().MyID;

        GameObject cardObj = GameObject.Find(ID);
        //search the deck for the card name and grab it
        LeustoffCard cardData = masterCardList.masterCards.Find(x => x.cardName == cardName);


        //build a new card
        CardDisplay _card = cardObj.GetComponent<CardDisplay>();
        _card.owner = PlayerID;

        _card.UpdateCard(cardData);
        //Name the GameObject in the scene

        cardObj.name = GameManager.ID.ToString();
        GameManager.ID++;



        if (localID == PlayerID)
        {
            _card.GainControl();
        }
        _card.zone = "Board";
        _card.HighLightSummoningSick();
        int targetZone = ZoneSorterOuter(PlayerID);
        GameObject TargetZone = GameObject.Find(Boards[targetZone]);
        cardObj.transform.SetParent(TargetZone.transform, false);
        cardObj.transform.rotation = TargetZone.transform.rotation;
        _card.FlipFaceUp();
        _card.CheckOverflow(_card.owner);
        UIManager.UpdatePlayerText();

    }

    [Command]
    public void CmdGrantDividend()
    {
        GameManager.RpcGrantDividend(MyID);
    }


    [Command]
    public void CmdHighlightCard(string cardID)
    {

        GameManager.RpcHighlightCard(cardID);
    }


    [Command]
    public void CmdUpdateUI()
    {
        UIManager.RpcUpdateUI();
    }

    [Command]
    void CmdTableEnergyCard(string ID, int PlayerID)
    {
        RpcTableEnergyCard(ID, PlayerID);
    }


    /// <summary>
    /// Method <c>RpcTableEnergyCard</c> take an Energy card from a players hand and creates a energy crystal from it.
    /// string ID is the unique ID of the card being played
    /// int PlayerID is the player playing an energy card
    /// </summary>
    [ClientRpc]
    void RpcTableEnergyCard(string ID, int PlayerID)
    {

        card = GameObject.Find(ID);
        GameManager.AddAMana(PlayerID);
        GameManager.EnergyPlayForTurn[PlayerID]--;
        Destroy(card);
        UIManager.UpdatePlayerText();
    }

    [Command]
    public void CmdCancelCast(string ID)
    {
        RpcCancelCast(ID);
  
 
    }
    /// <summary>
    /// Method <c>RpcCancelCast</c> allows the casting process to be cancelled; the card is returned to hand
    ///string ID is the gameObjects name
    /// </summary>
    [ClientRpc]
    public void RpcCancelCast(string ID)
    {
        spellcasting = false;
        GameObject card = GameObject.Find(ID);
        CardDisplay _card = card.GetComponent<CardDisplay>();
        int zonenumber = ZoneSorterOuter(_card.owner);
        _card.zone = "Hand";
        GameObject TargetZone = GameObject.Find(Hands[zonenumber]);
        card.transform.SetParent(TargetZone.transform, false);
        card.transform.rotation = TargetZone.transform.rotation;

    }

    /// <summary>
    /// Method <c>ZoomCopy</c> returns a blank card so the CardZoom class can create a zoomed in card for display
    /// </summary>
    public GameObject ZoomCopy()
    {

        GameObject card = Instantiate(CardMain, new Vector2(Input.mousePosition.x, Input.mousePosition.y), Quaternion.identity);
        return card;
    }
    public GameObject textObj;
    /*
    public GameObject ReminderText()
    {
        GameObject rText = Instantiate(textObj, new Vector2(Input.mousePosition.x, Input.mousePosition.y), Quaternion.identity);
        return rText;
    }
    */


    public Deck selectedDeck;
    /// <summary>
    /// Method <c>SelectDeck</c> sends a deckname to the server so the server can assign the player's deck
    /// string myDeckName is the name of the deck to be assigned
    /// RpcSubcommand
    /// </summary>
    public void SelectDeck(Faction myFaction,int playerID)
    {
        CmdSelectDeck(myFaction, playerID);
        haveDeck = true;
    }
    [Command]
    public void CmdSelectDeck(Faction myFaction, int PlayerID)
    {
        GameManager.RpcSelectDeck(myFaction, PlayerID);
    }

    [Command]
    public void CmdPassTurn()
    {
        GameManager.PassTurn();
    }

 
    [Command]
    public void CmdDealDeficit(int targetID, int TheTurn)
    {
        GameManager.RpcDealDeficit(targetID, TheTurn);
    }


    [Command]
    public void CmdGraveyard(string GyID)
    {
        GameManager.RpcGraveyard(GyID);

    }
    [Command]
    public void CmdBoardwipe()
    {
        GameManager.RpcBoardwipe();
    }
    [Command]
    public void CmdHaste()
    {
        GameManager.RpcHaste();
    }
    [Command]
    public void CmdLotusPetal(int PlayerID)
    {
        GameManager.RpcLotusPetal(PlayerID);
    }
    [Command]
    public void CmdDisrupt(string TargetID, int x)
    {
        GameManager.RpcDisrupt(TargetID, x);


    }

    [Command]
    public void CmdDampen(string TargetID, int x)
    {
        GameManager.RpcDampen(TargetID, x);
    }
    [Command]
    public void CmdBounce(string TargetID)
    {
        GameManager.RpcBounce(TargetID);
    }
    /// <summary>
    /// Method <c>CmdEdict</c> is a command to invoke _edict on the server
    /// Used by the spell manager, which is handled locally up to this point, unlike permanents, which are already on the server and call _edict() directly
    /// </summary>
    [Command]
    public void CmdEdict()
    {
        _edict();
    }
    /// <summary>
    /// Method <c>_edict</c> destroys each player's smallest creature
    /// Used by permanents entering the battlefield, which is already on the server, unlike spells, which use CmdEdict()
    /// </summary>
    public void _edict()
    {

        foreach (string board in Boards)
        {
            GameObject aBoard = GameObject.Find(board);
            int smallvalue = 100;
            int children = aBoard.transform.childCount;
            string theCard = "Fish";
            List<string> targetcards = new List<string>();
            for (int i = 0; i < children; i++)
            {
                CardDisplay _card = aBoard.transform.GetChild(i).GetComponent<CardDisplay>();
                int value = int.Parse(_card.attackText.text) + int.Parse(_card.defenseText.text);
                //If the sum power and toughness are lower, pick this card
                if (value < smallvalue)
                {
                    smallvalue = value;
                    theCard = aBoard.transform.GetChild(i).name;
                    targetcards.Clear();
                    targetcards.Add(theCard);
                }
                //if that's a tie...
                else if (value == smallvalue)
                {
                    //see if the power is lower
                    if (int.Parse(_card.attackText.text) < int.Parse(GameObject.Find(theCard).GetComponent<CardDisplay>().attackText.text))
                    {
                        theCard = aBoard.transform.GetChild(i).name;
                        targetcards.Clear();
                        targetcards.Add(theCard);
                    }
                    //if that too is a tie, add both to the list
                    else if (int.Parse(_card.attackText.text) == int.Parse(GameObject.Find(theCard).GetComponent<CardDisplay>().attackText.text))
                    {
                        theCard = aBoard.transform.GetChild(i).name;
                        targetcards.Add(theCard);
                    }
                }
            }
            //randomly pick one from the list.  This is usually going to be just one item.
            if (targetcards.Count > 0)
            {
                string deadcard = targetcards[UnityEngine.Random.Range(0, targetcards.Count)];
                RpcGraveyard(deadcard);

            }
        }
    }
    /// <summary>
    /// Method <c>CmdBehead</c> destroys each player's largest creature
    /// </summary>
    [Command]
    public void CmdBehead()
    {
        foreach (string board in Boards)
        {
            GameObject aBoard = GameObject.Find(board);
            int largevalue = 0;
            int children = aBoard.transform.childCount;
            string theCard = "Fish";
            List<string> targetcards = new List<string>();
            for (int i = 0; i < children; i++)
            {
                CardDisplay _card = aBoard.transform.GetChild(i).GetComponent<CardDisplay>();
                int value = int.Parse(_card.attackText.text) + int.Parse(_card.defenseText.text);
                //If the sum power and toughness are higher, pick this card
                if (value > largevalue)
                {
                    largevalue = value;
                    theCard = aBoard.transform.GetChild(i).name;
                    targetcards.Clear();
                    targetcards.Add(theCard);
                }
                //if that's a tie...
                else if (value == largevalue)
                {
                    //see if the power is higher
                    if (int.Parse(_card.attackText.text) > int.Parse(GameObject.Find(theCard).GetComponent<CardDisplay>().attackText.text))
                    {
                        theCard = aBoard.transform.GetChild(i).name;
                        targetcards.Clear();
                        targetcards.Add(theCard);
                    }
                    //if that too is a tie, add both to the list
                    else if (int.Parse(_card.attackText.text) == int.Parse(GameObject.Find(theCard).GetComponent<CardDisplay>().attackText.text))
                    {
                        theCard = aBoard.transform.GetChild(i).name;
                        targetcards.Add(theCard);
                    }
                }
            }
            //randomly pick one from the list.  This is usually going to be just one item.
            if (targetcards.Count > 0)
            {
                string deadcard = targetcards[UnityEngine.Random.Range(0, targetcards.Count)];
                RpcGraveyard(deadcard);

            }
        }
    }
    /// <summary>
    /// Method <c>RpcGraveyard</c> kills the card with the gameObject name given
    /// </summary>
    [ClientRpc]
    public void RpcGraveyard(string card)
    {
        GameObject.Find(card).GetComponent<CardDisplay>().Graveyard();
    }

    [Command]
    public void CmdTrial()
    {
        Trial();
    }
    /// <summary>
    /// Method <c>Trial</c> kills all of each player's creatures, except the largest that player controls. 
    /// </summary>
    public void Trial()
    {


        List<string> survivors = new List<string>();
        int bigValue = -1;
        string theCard = "Fish";
        List<string> targetcards = new List<string>();
        List<string> deadcards = new List<string>();
        for (int i = 0; i < GameManager.PlayerID; i++)
        {
             bigValue = -1;
            foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
            {
                CardDisplay _card = card.GetComponent<CardDisplay>();

                if (_card.zone == "Board" & _card.typeText.text.ToString() == "Matter" & _card.owner == i)
                {

                    int value = int.Parse(_card.attackText.text) + int.Parse(_card.defenseText.text);
                    //If this value is biggest, make this the targetr card
                    if (value > bigValue)
                    {
                        bigValue = value;
                        theCard = card.name;
                        targetcards.Clear();
                        targetcards.Add(card.name);
                    }
                    //if that's a tie...
                    else if (value == bigValue)
                    {
                        //see if the power is higher
                        if (int.Parse(_card.attackText.text) > int.Parse(GameObject.Find(theCard).GetComponent<CardDisplay>().attackText.text))
                        {

                            targetcards.Clear();
                            targetcards.Add(card.name);
                        }
                        //if that too is a tie, add both to the list
                        else if (int.Parse(_card.attackText.text) == int.Parse(GameObject.Find(theCard).GetComponent<CardDisplay>().attackText.text))
                        {

                            targetcards.Add(card.name);
                        }
                    }
                }
            }

            if (targetcards.Count > 0)
            {
                string survivecard = targetcards[UnityEngine.Random.Range(0, targetcards.Count)];
                survivors.Add(survivecard);

            }
        }



        //randomly pick one from the list.  This is usually going to be just one item.

        foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardDisplay _card = card.GetComponent<CardDisplay>();
            if (!survivors.Contains(card.name) & _card.zone == "Board" & _card.typeText.text.ToString()=="Matter") 
            {
                deadcards.Add(card.name);

            }

        }
        foreach (string card in deadcards)
        {

            RpcGraveyard(card);
        }
    }


 
    
    [Command]
    public void CmdGlasses(int PlayerID)
    {
        GameManager.RpcGlasses(PlayerID);
    }
    
    [Command]
    public void CmdNoGlasses(int PlayerID)
    {
        GameManager.RpcNoGlasses(PlayerID);
    }
    [Command]
    public void CmdBuildDamageList(int turn)
    {
        GameManager.RpcBuildDamageList(turn);
    }
    [Command]
    public void CmdAutoSetup()
    {
        RpcAutoSetup();

    }
    /// <summary>
    /// Method <c>RpcAutoSetup</c> sets four human player's deckname and factions.
    /// Used for testing multiplayer quickly
    /// </summary>
    [ClientRpc]
    public void RpcAutoSetup()
    {
        List<Faction> DeckNames = new List<Faction> { Faction.Materials, Faction.Planets, Faction.Media, Faction.Biomes };
        int i = 0;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.GetComponent<PlayerManager>().SetID(DeckNames[i]);
            i++;
        }
    }
    /// <summary>
    /// Method <c>SetID</c> is a nonRPC equivalent of RpcSetID used for RpcAutoSetup()
    /// </summary>
    public void SetID(Faction decklistFaction)
    {
        
    GameManager.addPlayer(gameObject.name,false);
    GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    MyID = GameManager.PlayerID;
    GameManager.PlayerID++;
    
    GameManager.PlayerFactionList[MyID] = decklistFaction;
    UIManager.UpdatePlayerText();
    PlayerManager PM = NetworkClient.connection.identity.GetComponent<PlayerManager>();     
    GameManager.SelectDeck(decklistFaction, MyID);
        haveDeck = true;
        

    }
    /// <summary>
    /// Method <c>CmdSetBotID</c> tells the server to set a bots deck and ID
    /// string decklistName is the name of the deck the botwill use
    /// string BotName is the gameObject name of the Bot's NonplayerManager.
    /// NonPlayerManager inherits from PlayerManager
    /// </summary>
    [Command]
    public void CmdSetBotID(Faction myFaction, string BotName)
    {

        GameObject Bot = GameObject.Find(BotName);
        NonPlayerManager BotManager = Bot.GetComponent<NonPlayerManager>();
        BotManager.RpcSetID(myFaction);



    }
    /// <summary>
    /// Method <c>CmdSacSmallest</c> destroys the given player's smallest creature
    /// Called when the player has played a sixth creature
    /// The maximum number of creatures is five
    /// RPCSubcommand
    /// </summary>
    [Command]
    public void CmdSacSmallest(int player)
    {
  
        int smallvalue = 100;
        List<string> targetcards = new List<string>();
        foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardDisplay _card = card.GetComponent<CardDisplay>();
            if (_card.zone == "Board" & _card.typeText.text.ToString() == "Matter" & _card.owner == player)
            {
                int value = int.Parse(_card.attackText.text) + int.Parse(_card.defenseText.text);
                //If the sum power and toughness are lower, pick this card
                if (value < smallvalue)
                {
                    smallvalue = value;                  
                    targetcards.Clear();
                    targetcards.Add(_card.gameObject.name);
                }
                //if that's a tie...
                else if (value == smallvalue)
                {
                    //see if the power is lower
                    if (int.Parse(_card.attackText.text) < int.Parse(GameObject.Find(targetcards[0]).GetComponent<CardDisplay>().attackText.text))
                    {
                        targetcards.Clear();
                        targetcards.Add(_card.gameObject.name);
                    }
                    //if that too is a tie, add both to the list
                    else if (int.Parse(_card.attackText.text) == int.Parse(GameObject.Find(targetcards[0]).GetComponent<CardDisplay>().attackText.text))
                    {
                       
                        targetcards.Add(_card.gameObject.name);
                    }
                }
            }
        }
        //randomly pick one from the list.  This is usually going to be just one item.
        if (targetcards.Count > 0)
        {
            string deadcard = targetcards[UnityEngine.Random.Range(0, targetcards.Count)];
            RpcGraveyard(deadcard);
        }
        
    }
  
    [Command]
    public void CMDAttack()
    {
        GameObject.Find("AttackButton").GetComponent<Attack>().DoAttack();
    }
    [Command]
    public void CmdReinforce(int PlayerID)
    {
        GameManager.Reinforce(PlayerID);
    }
    [Command]
    public void CmdBuyCombo(int PlayerID)
    {
        ShowCard("Chunk", PlayerID);
    }
    [Command]
    public void CmdSpendDividend(int PlayerID, int Cost)
    {
        RpcSpendDividend(PlayerID, Cost);
    }
    [ClientRpc]
    public void RpcSpendDividend(int PlayerID, int Cost)
    {
        GameManager.playerDividendList[PlayerID] -= Cost;
        UIManager.UpdatePlayerText();
    }
    [Command]
    public void CmdGainDividend()
    {
        GameManager.RpcGrantDividend(MyID);
    }
}
