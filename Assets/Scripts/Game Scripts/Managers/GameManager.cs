using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LeustoffNamespace;
using Mirror;
using System;
using System.Threading;
using System.Timers;

public class GameManager : NetworkBehaviour
{

    public PlayerManager PlayerManager;

    public UIManager UIManager;
    public int TurnOrder = 0;

    public string phase = "Main";
    public string subphase = "Main";

    public Deck deckList1;
    public Deck deckList2;
    public GameObject aTimer;


    public List<int> playerRevenueList = new List<int>();
    public List<int> playerManaList = new List<int>();
    public List<int> currentManaList = new List<int>();
    public List<int> playerDeficitList = new List<int>();
    public List<int> playerDividendList = new List<int>();
    public List<int> EnergyPlayForTurn = new List<int>();
    public List<int> playerTookDamageList = new List<int>();
    public List<int> playerRevenueThresholdList = new List<int>();
    public List<int> playerDeficitThresholdList = new List<int>();
    public List<int> playerComboPowerList = new List<int>();
    public List<int> tempComboPower = new List<int>();
    public List<int> playerComboDamageList = new List<int>();
    public List<int> playerHasGlassesList = new List<int>();
    public List<string> declaredAttackers = new List<string>();
    public List<deckList> playersDeckLists = new List<deckList>();
    public List<string> shouldBeFaceDown = new List<string>();
    public List<int> thisTurnPoints = new List<int>();
    public List<int> LastDamageDealt = new List<int>();
    public List<int> TotalAttack = new List<int>();
    public List<int> CurrentAttack = new List<int>();
    public List<int> TotalDefense = new List<int>();
    public List<int> PlayerScoreList = new List<int>();
    public List<int> PermanentAttackBuffs = new List<int>();
    public List<int> PermanentDefenseBuffs = new List<int>();
    public List<Faction> PlayerFactionList = new List<Faction>();
    public List<int> FortifyEnhanceList = new List<int>();
    public deckListList DeckListList;
    public Deck masterCardList;
    public GameObject DeckMain;
    public List<int> taxTotalList = new List<int>();
    public ArtManager ArtManager;
    public int ID = 0;
    public int PlayerID = 0;
    public List<bool> BotTurns = new List<bool>();
    public int turn = 0;
    public int globalturn = 0;
    public int StartingRevenueThreshold;
    public int StartingDeficitThreshhold;
    public bool TimeWalk = false;
    public GameObject ComboPoint;
    public GameObject ManaCrystal;

    public List<string> PlayerList = new List<string>();
    //SET GOALS.  HAVE A TEN YEAR PLAN. INVEST.  WAKE UP EARLY.  CEO MINDSET.
    void Start()
    {
        PlayerID = 0;
        turn = 0;
        StartingRevenueThreshold = 6;
        StartingDeficitThreshhold = 3;


    }
    /// <summary>
    /// Method <c>OnLevelFinishedLoading</c> loads the deck selector
    /// </summary>
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        GameObject.Find("Deck Selects").SetActive(true);
    }
    /// <summary>
    /// Most variables related to player game state is stored in the GameManager as a list.  Each time a player joins each of these lists adds a blank entry to each list.
    /// The Players ID is the index of each list that has their stats.
    /// RPC subcommand
    /// </summary>

    public void addPlayer(string playerObj, bool isBot)
    {

        playerManaList.Add(1);
        currentManaList.Add(1);
        playerRevenueList.Add(0);
        playerDeficitList.Add(0);
        playerDividendList.Add(0);
        EnergyPlayForTurn.Add(0);
        playerTookDamageList.Add(0);
        playerRevenueThresholdList.Add(StartingRevenueThreshold);
        playerDeficitThresholdList.Add(StartingDeficitThreshhold);
        playerComboPowerList.Add(0);
        playerComboDamageList.Add(0);
        tempComboPower.Add(0);
        playerHasGlassesList.Add(0);
        TotalAttack.Add(0);
        CurrentAttack.Add(0);
        TotalDefense.Add(0);
        PlayerScoreList.Add(0);
        PlayerFactionList.Add(Faction.NoFaction);
        taxTotalList.Add(0);
        thisTurnPoints.Add(0);
        LastDamageDealt.Add(0);
        FortifyEnhanceList.Add(0);
        PlayerList.Add(playerObj);
        BotTurns.Add(isBot);
        PermanentDefenseBuffs.Add(0);
        PermanentAttackBuffs.Add(0);
    }
    /// <summary>
    /// Method <c>DrawCard</c> is run on the server and lets a player draw a card.  It checks if the hand has space before proceeding.
    /// int PlayerID is the ID of the player who will draw the card
    /// Command subcommand
    /// </summary>
    public void DrawCard(int PlayerID)
    {
        GameObject TargetHand = GameObject.Find(PlayerManager.Hands[ZoneSorterOuter(PlayerID)]);
        if (TargetHand.transform.childCount < 5)
        {
            TrueDrawCard(PlayerID);
         
        }
        else
        {
            RpcGrantDividend(PlayerID);
        }
      
    }
    /// <summary>
    /// Method <c>TrueDrawCard</c> is the final step on the server before the clients draw the card.
    /// This should usually not be called directly, call DrawCard() so the hand size can be checked
    /// int PlayerID is the ID of the player who will draw the card
    /// Command subcommand
    /// </summary>
    public void TrueDrawCard(int PlayerID)
    {
        if (playersDeckLists[PlayerID].CurrentDecklist.Count == 0)
        {
            NetworkClient.connection.identity.GetComponent<PlayerManager>().RpcYouWin(PlayerID);
        }
        string DrawnCard = playersDeckLists[PlayerID].GetRandomCard();
        RpcRemoveDrawnCard(PlayerID, DrawnCard);
        PlayerManager.ShowCard(DrawnCard, PlayerID);
    }
    /// <summary>
    /// Method <c>RpcRemoveDrawnCard</c> removes a card from a decklist by cardname
    /// int PlayerID is the ID of the player who will drew the card
    /// string DrawnCard is the name of the card to remove
    /// </summary>
    [ClientRpc]
    public void RpcRemoveDrawnCard(int PlayerID, string DrawnCard)
    {
        playersDeckLists[PlayerID].RemoveByName(DrawnCard);
    }

    /// <summary>
    /// Method <c>RpcSelectDeck</c> tells clients which decks each player has
    /// string myDeckName is the name of the deck
    /// int PlayerID is the ID of the player selecting a deck
    /// </summary>
    [ClientRpc]
    public void RpcSelectDeck(Faction myFaction, int PlayerID)
    {
        string myDeckName = FactionName(myFaction);
        Deck emptydeck = DeckListList.deckLists.Find(x => x.name == "emptyDeck");
        GameObject NewDeck = Instantiate(DeckMain, new Vector2(0, 0), Quaternion.identity);
        deckList NewDecklist = NewDeck.GetComponent<deckList>();
        playersDeckLists.Add(NewDecklist);
        Deck selectedDeck = DeckListList.deckLists.Find(x => x.name == myDeckName);
        playersDeckLists[PlayerID].CopyList(selectedDeck);

    }
    /// <summary>
    /// Method <c>FactionName</c> returns a string of the faction's name
    /// Faction myFaction is the LeustoffNamespace.Faction to return the name of
    /// </summary>
    public string FactionName(Faction myFaction)
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
                return "No Faction";
                break;
        }
    }
    /// <summary>
    /// Method <c>SelectDeck</c> sets a players deck locally
    /// string myDeckName is the name of the deck
    /// int PlayerID is the ID of the player selecting a deck
    ///Rpc subfunction called from the autosetup routine
    /// </summary>
    public void SelectDeck(Faction myFaction, int PlayerID)
    {
        Deck emptydeck = DeckListList.deckLists.Find(x => x.name == "emptyDeck");
        GameObject NewDeck = Instantiate(DeckMain, new Vector2(0, 0), Quaternion.identity);
        deckList NewDecklist = NewDeck.GetComponent<deckList>();
        playersDeckLists.Add(NewDecklist);

        string myDeckName = FactionName(myFaction);
        Deck selectedDeck = DeckListList.deckLists.Find(x => x.name == myDeckName);
        playersDeckLists[PlayerID].CopyList(selectedDeck);

    }

    [ClientRpc]
    public void RpcAddAp(int PlayerID)
    {
        AddAp(PlayerID);
    }
    /// <summary>
    /// Method <c>AddAp</c> gives a player a revenue then checks if they've reached the threshold to allow a card draw
    /// int PlayerID is the ID of the player gaining a revenue
    ///Rpc subfunction called from RpcAddAp()
    /// </summary>
    public void AddAp(int PlayerID)
    {
        thisTurnPoints[PlayerID]++;
        LastDamageDealt[PlayerID]++;
        playerRevenueList[PlayerID]++;
        if (playerRevenueList[PlayerID] == playerRevenueThresholdList[PlayerID])
        {



            PlayerManager.GrantPayout(PlayerID);


            playerRevenueList[PlayerID] = 0;
        }
        UIManager.UpdatePlayerText();
    }
    /// <summary>
    /// Method <c>RpcDealDeficit</c> gives a player a deficit then checks if they've reached the threshold to be dealt a penalty card
    /// int PlayerID is the ID of the player recieving a deficit
    /// int TheTurn is the turn on which the definit was dealt, so the damage number animation can display correctly
    ///Rpc subfunction called from RpcAddAp()
    /// </summary>
    [ClientRpc]
    public void RpcDealDeficit(int targetID, int TheTurn)
    {
        PlayerManager PM = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        PM.CalculateComboPower(targetID);
        theturnthatitwas = TheTurn;
        thisTurnPoints[targetID]++;
        playerDeficitList[targetID]++;
        playerTookDamageList[targetID]++;
        if (playerDeficitList[targetID] == playerDeficitThresholdList[targetID])
        {
            if (playerComboPowerList[targetID] > 0 & tempComboPower[targetID]>0)
            {
                BreakChunk(targetID);
                tempComboPower[targetID]--;
                UIManager.UpdatePlayerText();
            }
            else
            {
                AddTaxCard(targetID);

            }
            playerDeficitList[targetID] = 0;
            PM.TaxAnim(targetID);
        }
        PM.CalculateComboPower(targetID);

        UIManager.UpdatePlayerText();
        UIManager.UpdateDeckcounter();
        Invoke("ShowDamageNumbers", .1f);
        Invoke("DelayedUpdateText", .1f);

    }
    public void DelayedUpdateText()
    {
        UIManager.UpdatePlayerText();
    }

    public int theturnthatitwas;
    void ShowDamageNumbers()
    {
        UIManager.ShowDamageNumbers(theturnthatitwas);
    }

    /// <summary>
    /// Method <c>AddTaxCard</c> adds a penalty card to a players deck
    /// int targetID is the ID of the player recieving a penalty card
    /// RPC subcommand
    /// </summary>
    public void AddTaxCard(int targetID)
    {
        playersDeckLists[targetID].AddTempCard("Entropy");
        taxTotalList[targetID]++;
        UpdateBloodthirsty();

    }
    /// <summary>
    /// Method <c>BreakChunk</c> takes one combo point and puts it in the enemy deck
    /// int targetID is the ID of the player recieving a penalty card
    /// RPC subcommand
    /// </summary>
    public void BreakChunk(int targetID)
    {
        playersDeckLists[targetID].AddTempCard("Chunk");
        playerComboDamageList[targetID]++;

    }
    /// <summary>
    /// Method <c>UpdateBloodthirsty</c> updates creatures with the Bloodthirsty ability
    /// this ability relates to the number of penalty cards in opponents deck, and is not currently in use due to being overpowered
    /// RPC subcommand
    /// </summary>
    public void UpdateBloodthirsty()
    {
        foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardDisplay _card = card.GetComponent<CardDisplay>();

            if (_card.zone == "Board" & _card.typeText.text == "Matter")
            {

                card.GetComponent<BuffHandler>().UpdateBloodthirsty();
            }
        }
    }
    [ClientRpc]
    public void RpcGrantDividend(int PlayerID)
    {
        GrantDividend(PlayerID);
    }
    public void GrantDividend(int PlayerID)
    {
        playerDividendList[PlayerID]++;
        UIManager.UpdatePlayerText();
    }

    /// <summary>
    /// Method <c>PassTurn</c> moves the turn forwards on the server
    /// TimeWalk is how extra turns are handled
    /// Command subcommand
    /// </summary>
    public void PassTurn()
    {
        int oldturn = turn;
        if (TimeWalk)
        {
            turn --;
            TimeWalk = false;
        }
        
        turn++;
        if (turn > PlayerID - 1)
        {
            turn = 0;
            globalturn++;
        }
        RpcPassTurn(turn,oldturn, globalturn);
           



        if (BotTurns[turn] == true)
        {
            GameObject Bot = GameObject.Find("Bot" + turn);
            Bot.GetComponent<NonPlayerManager>().RpcTakeTurn();
        }

        NextPlayerDraws(turn);
        


    }
    /// <summary>
    /// Method <c>RpcPassTurn</c> moves the turn forwards on the clients
    /// </summary>
    [ClientRpc]
    public void RpcPassTurn(int newTurn, int oldturn, int globalTurn)
    {
        turn = newTurn;
        globalturn = globalTurn;
       EndOfTurnCreatureTriggers(oldturn);
       RefreshMana(newTurn);
       RemoveSummoningSick(newTurn);
       Upkeep(newTurn);

        UIManager.UpdatePlayerText();
    }
    /// <summary>
    /// Method <c>Upkeep</c> triggers start of turn effects on all Effect objects.
    /// These are buffs and debufs that are temporary on creatures
    /// Rpc Subfunction
    /// </summary>
    public void Upkeep(int turn)
    {
        foreach (GameObject effect in GameObject.FindGameObjectsWithTag("TempEffect"))
        {
            TempEffect _effect = effect.GetComponent<TempEffect>();
            _effect.Upkeep(turn);
        }
    }
    /// <summary>
    /// Method <c>RefreshMana</c> gives players their energy back at the start of the turn
    /// Also allows play of aditional energy on later turns
    /// int newTurn is the ID of the player who turn it is
    /// Rpc Subfunction
    /// </summary>
    public void RefreshMana(int newTurn)
    {

        currentManaList[newTurn] = playerManaList[newTurn];
        if (globalturn > 0)
        {
            EnergyPlayForTurn[newTurn] = 1;
        }
        phase = "Main";
        subphase = "Main";
        UIManager.UpdatePlayerText();
    }
    /// <summary>
    /// Method <c>RemoveSummoningSick</c> allows creatures to attack after they've been out for a turn cycle
    /// int newTurn is the ID of the player who turn it is
    /// Rpc Subfunction
    /// </summary>
    public void RemoveSummoningSick(int newTurn)
    {
        PlayerManager PM = NetworkClient.connection.identity.GetComponent<PlayerManager>();

        GameObject TargetZone = GameObject.Find(PM.Boards[ZoneSorterOuter(newTurn)]);
        int children = TargetZone.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            CardDisplay _card = TargetZone.transform.GetChild(i).GetComponent<CardDisplay>();
            _card.RemoveSummoningSick();

        }

    }
    /// <summary>
    /// Method <c>DisplayEventText</c> shows a given string in big letters at the center of the screen
    /// string EventText is the text to be shown
    /// Only used for counterspells, which are not currently in the game
    /// Rpc Subfunction
    /// </summary>
    public void DisplayEventText(string EventText)
    {
        UIManager.DisplayEventText(EventText);
    }
    /// <summary>
    /// Method <c>NextPlayerDraws</c> allows the next player to draw a card
    /// if the players hand is full, rather than granting a dividend, the game grants the player an additional energy
    /// if the player has an energy card in hand, it is played and a new card is drawn to replace it automatically
    /// Command subcommand
    /// </summary>
    public void NextPlayerDraws(int newTurn)
    {
        if (globalturn < 1) { return; }
        PlayerManager PM = NetworkClient.connection.identity.GetComponent<PlayerManager>();

        GameObject TargetHand = GameObject.Find(PM.Hands[ZoneSorterOuter(newTurn)]);

        if (TargetHand.transform.childCount < 5)
        {
            DrawCard(newTurn);

        }
        else
        {
            for (int i = 0; i < TargetHand.transform.childCount; i++)
            {
                if (TargetHand.transform.GetChild(i).GetComponent<CardDisplay>().typeText.text == "Energy")
                {
                    Destroy(TargetHand.transform.GetChild(i).gameObject);
                    TrueDrawCard(newTurn);
                    RpcDrawOverflow(newTurn);
                    UIManager.UpdatePlayerText();
                    return; 
                }
               
              }
            RpcDrawOverflow(newTurn);



            UIManager.UpdatePlayerText();
        }
    }
    /// <summary>
    /// Method <c>NextPlayerDraws</c> grants a player an energy, and removes their energy play for that turn
    /// this is called when drawing a new card at the start of the turn with a full hand
    /// int newTurn is the ID of the player gaining an energy
    /// </summary>
    [ClientRpc]
    public void RpcDrawOverflow(int newTurn)
    {
     
    
        
        playerManaList[newTurn]++;
        currentManaList[newTurn]++;
        EnergyPlayForTurn[newTurn]--;
    }

    /// <summary>
    /// Method <c>AddAMana</c> grants a player an energy
    /// this is called when playing an energy card
    /// int PlayerID is the ID of the player gaining an energy
    /// Rpc subfunction
    /// </summary>
    public void AddAMana(int PlayerID)
    {
        playerManaList[PlayerID]++;
        currentManaList[PlayerID]++;
    }
    /// <summary>
    /// Method <c>HaveMana</c> checks if a player has enough energy
    /// int currentCost is the cost to check
    /// int PlayerID is the player who is checking
    /// returns a bool of if they have enough or not
    /// </summary>
    public bool HaveMana(int currentCost, int PlayerID)
    {
        return (currentCost <= currentManaList[PlayerID]);
    }
    public int lastPaid;
    /// <summary>
    /// Method <c>RpcPayMana</c> deducts the cost of a spell
    /// int currentCost is the cost to pay
    /// int PlayerID is the player who is casting
    /// </summary>
    [ClientRpc]
    public void RpcPayMana(int currentCost, int PlayerID)
    {
        currentManaList[PlayerID] = currentManaList[PlayerID] - currentCost;
        UIManager.UpdatePlayerText();
        lastPaid = currentCost;
    }
    /// <summary>
    /// Method <c>RpcUnPayMana</c> refunds the cost of a spell
    /// int PlayerID is the player who is cancelling
    /// Used in the targeted spell flow
    /// </summary>
    [ClientRpc]
    public void RpcUnPayMana(int PlayerID)
    {
        currentManaList[PlayerID] = currentManaList[PlayerID] + lastPaid;
        lastPaid = 0;
        UIManager.UpdatePlayerText();
    }
    /// <summary>
    /// Method <c>GetTotalPTs</c> sums the attack and defense of creatures in play 
    /// </summary>
    public void GetTotalPTs()
    {
        for (int i = 0; i < PlayerID; i++)
        {
          TotalAttack[i] = PermanentAttackBuffs[i];
          TotalDefense[i] = PermanentDefenseBuffs[i];
          CurrentAttack[i] = PermanentAttackBuffs[i];
        }
        foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardDisplay _card = card.GetComponent<CardDisplay>();
            if (_card.zone == "Board" & _card.typeText.text.ToString() == "Matter")
            {
                TotalAttack[_card.owner] += int.Parse(_card.attackText.text);
                TotalDefense[_card.owner] += int.Parse(_card.defenseText.text);
                if (!_card.SummoningSick)
                {
                    CurrentAttack[_card.owner] += int.Parse(_card.attackText.text);
                }
            }
        }
    }
    /// <summary>
    /// Method <c>RpcHighlightCard</c> finds a card and tells it to highlight
    /// </summary>
    [ClientRpc]
    public void RpcHighlightCard(string cardID)
    {
        GameObject targetCard = GameObject.Find(cardID);
        targetCard.GetComponent<CardDisplay>().HighlightCard();
    }

    /// <summary>
    /// Method <c>ZoneSorterOuter</c> takes the 'target' player's ID and returns an int that corresponds to which 'seat' at the table is relevant.
    /// IE: You are 0, the player playing after you is 1, and so on.
    /// </summary>
    public int ZoneSorterOuter(int relevantID)
    {
        int localID = NetworkClient.connection.identity.GetComponent<PlayerManager>().MyID;
        var gribbit = PlayerID - localID;
        var gaboodle = gribbit + relevantID;
        var grobbit = Util.mod(gaboodle, PlayerID);
        return grobbit;
    }
    /// <summary>
    /// Method <c>RpcGraveyard</c> sends a card to the graveyard
    /// string TargetID is the gameObject name of the card to send to the graveyard
    /// </summary>
    [ClientRpc]
    public void RpcGraveyard(string TargetID)
    {
        GameObject _card = GameObject.Find(TargetID);
        _card.GetComponent<CardDisplay>().FlipFaceUp();
        _card.GetComponent<CardDisplay>().Graveyard();
    }
    /// <summary>
    /// Method <c>PlagueWind</c> destroys all creatures not controlled by a player
    /// int PlayerID is the player whos creatures survive
    /// Rpc subfunction
    /// </summary>
    public void PlagueWind(int PlayerID)
    {
        foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardDisplay _card = card.GetComponent<CardDisplay>();
            if (_card.zone == "Board" & _card.typeText.text.ToString() == "Matter" & _card.owner != PlayerID)
            {
                card.GetComponent<CardDisplay>().Graveyard();
            }
        }
    }
    /// <summary>
    /// Method <c>RpcBoardwipe</c> destroys all creatures 
    /// </summary>
    [ClientRpc]
    public void RpcBoardwipe()
    {
        foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardDisplay _card = card.GetComponent<CardDisplay>();
          
            if (_card.zone == "Board"& _card.typeText.text.ToString() == "Matter")
            {
                _card.Graveyard();
            }
        }
    }
    /// <summary>
    /// Method <c>RpcHaste</c> removes summoning sickness from all creatures of the player whos turn it is
    /// This  allows attacks on the same turn
    /// </summary>
    [ClientRpc]
    public void RpcHaste()
    {
        foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardDisplay _card = card.GetComponent<CardDisplay>();

            if (_card.zone == "Board" & _card.typeText.text.ToString() == "Matter" & _card.owner==turn)
            {

                _card.RemoveSummoningSick(); 
            }
        }
        
    }
    /// <summary>
    /// Method <c>RpcLotusPetal</c> adds a temporary mana and deducts a dividend
    /// int PlayerID is the player spending the dividend
    /// </summary>
    [ClientRpc]
    public void RpcLotusPetal(int PlayerID)
    {
        currentManaList[PlayerID]++;
        playerDividendList[PlayerID]--;
        UIManager.UpdatePlayerText();
    }
    /// <summary>
    /// Method <c>EndOfTurnCreatureTriggers</c> tells creatures to use their end of turn effects
    /// int turn is the turn that is ending
    /// Rpc Subfunction
    /// </summary>
    public void EndOfTurnCreatureTriggers(int turn)
    {
        foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardDisplay _card = card.GetComponent<CardDisplay>();
            _card.EoT(turn);         
        }
       

    }
    /// <summary>
    /// Method <c>RpcGlasses</c> shows enemy hands to the player
    /// int PlayerID is the player gaining this ability
    /// </summary>
    [ClientRpc]
    public void RpcGlasses(int PlayerID)
    {
        playerHasGlassesList[PlayerID]++;
        if (NetworkClient.connection.identity.GetComponent<PlayerManager>().MyID == PlayerID)
        {
            GameObject enemyHands = GameObject.Find("EnemyAreas");
            int hands = enemyHands.transform.childCount;
            for (int j = 0; j < hands; j++)
            {
                int children = enemyHands.transform.GetChild(j).transform.childCount;
                for (int i = 0; i < children; i++)
                {

                    CardDisplay _card = enemyHands.transform.GetChild(j).transform.GetChild(i).GetComponent<CardDisplay>();
                    _card.FlipFaceUp();
                }
            }

        }
    }
    /// <summary>
    /// Method <c>RpcNoGlasses</c> removes one instance the glasses effect
    /// if no instances remain, the cards in enemy hands are hidden
    /// int PlayerID is the player losing this ability
    /// </summary>
    [ClientRpc]
    public void RpcNoGlasses(int PlayerID)
    {
        playerHasGlassesList[PlayerID]--;
        if (PlayerID == NetworkClient.connection.identity.GetComponent<PlayerManager>().MyID)
        {
            if (playerHasGlassesList[PlayerID] == 0)
            {
                foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
                {
                    CardDisplay _card = card.GetComponent<CardDisplay>();

                    if (_card.zone == "Hand" & _card.owner != NetworkClient.connection.identity.GetComponent<PlayerManager>().MyID)
                    {

                        _card.FlipFaceDown();
                    }
                }
            }
        }
   
    }

    /// <summary>
    /// Method <c>RpcBuildDamageList</c> resets the trackers for the last damage dd=ealt
    /// int turn is the turn that this is happening on
    /// </summary>
    [ClientRpc]
    public void RpcBuildDamageList(int turn)
    {
        LastDamageDealt[turn] = 0;

        thisTurnPoints.Clear();
        for (int i = 0; i < 4; i++)
        { 
            thisTurnPoints.Add(0);
        }

        //new turn nobody has taken damage now
        for (int i = 0; i < playerTookDamageList.Count; i++)
        {
            playerTookDamageList[i] = 0;

        }
        for (int i = 0; i < PlayerID; i++)
        {
            tempComboPower[i] = playerComboPowerList[i];
        }
    }
    [ClientRpc]
    public void RpcDisrupt(string TargetID, int x)
    {
        GameObject.Find(TargetID).GetComponent<BuffHandler>().Disrupt(x);
    }

    [ClientRpc]
    public void RpcDampen(string TargetID, int x)
    {
        GameObject.Find(TargetID).GetComponent<BuffHandler>().Dampen(x);
    }

    [ClientRpc]
    public void RpcBounce(string TargetID)
    {
        GameObject.Find(TargetID).GetComponent<CardDisplay>().Bounce();
    }
    

    public bool isMP = false;
    /// <summary>
    /// Method <c>MPToggle</c> handles the button in the deck select screen that toggles the multiplayer
    /// The only real funcitonality of this at this time is that bots wont spawn
    /// </summary>
    public void MPToggle()
    {
        isMP = !isMP;
        if (isMP)
        {
            GameObject.Find("MPToggleText").GetComponent<Text>().text = "Multiplayer (under development)";
        }
        else
        {
            GameObject.Find("MPToggleText").GetComponent<Text>().text = "Single Player";
        }
    }
    public bool isCheats = false;
    public GameObject Debugs;
    /// <summary>
    /// Method <c>CheatsToggle</c> enables debug tools
    /// </summary>
    public void CheatsToggle()
    {
       
        Debugs.SetActive(!isCheats);
        isCheats = !isCheats;
        if (isCheats)
        {
            GameObject.Find("Cheats Text").GetComponent<Text>().text = "Debug Tools On";
        }
        else
        {
            GameObject.Find("Cheats Text").GetComponent<Text>().text = "Debug Tools Off";
        }
    }
    /// <summary>
    /// Method <c>Reinforce</c> adds permanent attack and defense to the player
    /// This is called by the dividends manager
    /// int PlayerID is the player gaining the bonus
    /// </summary>
    public void Reinforce(int PlayerID)
    {
        PermanentAttackBuffs[PlayerID]++;
        PermanentDefenseBuffs[PlayerID]++;
    }

}

