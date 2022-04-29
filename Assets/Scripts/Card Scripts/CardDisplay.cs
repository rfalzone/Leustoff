using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using LeustoffNamespace;

public class CardDisplay : NetworkBehaviour
{

    public Text nameText;
    public Text typeText;
    public cardType myType;
    public Text attackText;
    public Text defenseText;
    public Text costText;
    public Image art;
    public LeustoffCard sourceCard;
    public bool isMine = false;
    public Image image;
    public bool isFaceUp = true;
    public string zone;
    public bool isHighlighted = false;
    public GameManager GameManager;
    public GameObject PlayerYard;
    public GameObject EnemyYard;
    public bool SummoningSick;
    public int owner;
    public int baseAttack;
    public int baseDefense;
    public string inCombatWith = "-1";
    public List<Ability> Abilities = new List<Ability>();
    public bool Shielded = false;
    public bool isCountered = false;
    public BuffHandler BuffHandler;
    Rigidbody m_Rigidbody;
    public GameObject abilityIcon;
    public bool isPoppedOut = false;


    void Start()
    {
        //Fetch the Rigidbody component from the GameObject
        m_Rigidbody = gameObject.GetComponent<Rigidbody>();
        //Ignore the collisions between layer 0 (default) and layer 8 (custom layer you set in Inspector window)
        Physics.IgnoreLayerCollision(7, 7);
    }
    public int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    public int ZoneSorterOuter(int PlayerID)
    {
        int localID = NetworkClient.connection.identity.GetComponent<PlayerManager>().MyID;
        var gribbit = GameManager.PlayerID - localID;
        var gaboodle = gribbit + PlayerID;
        var grobbit = mod(gaboodle, GameManager.PlayerID);
        return grobbit;
    }
    /// <summary>
    /// Method <c>UpdateCard</c> fills out a card's stats
    /// LeustoffCard _card is the card data object that will be used to fill out the card
    /// Rpc subfunction
    /// </summary>
    public void UpdateCard(LeustoffCard _card)
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        BuffHandler = gameObject.GetComponent<BuffHandler>();
        ArtManager ArtManager = GameManager.GetComponent<ArtManager>();
        sourceCard = _card;

        nameText.text = _card.cardName;
        myType = _card._cardType;
        switch (myType)
        {
            case cardType.Matter:
                typeText.text = "Matter";
                break;
            case cardType.Energy:
                typeText.text = "Energy";
                break;
            case cardType.Combo:
                typeText.text = "Combo";
                break;
            case cardType.Spell:
                typeText.text = "Spell";
                break;     
        }
        if (!string.IsNullOrEmpty(_card.power)) { baseAttack = int.Parse(_card.power); }
        if (!string.IsNullOrEmpty(_card.defense)) { baseDefense = int.Parse(_card.defense); }
           
        attackText.text = _card.power.ToString();
        defenseText.text = _card.defense.ToString();
        costText.text = _card.cost.ToString();
        if (ArtManager.active)
        {
            art.sprite = _card.altArt;
        }
        else { art.sprite = _card.art; }
       
        Abilities.Clear();
        ClearIcons();
        
         
            
        foreach (Ability ability in _card.abilities)
        {
            Abilities.Add(ability);
            AddAbility(ability);
        }
        if (typeText.text == "Matter")
        {
            BuffHandler.Setup();
        }
        if (Abilities == null)
        {
            Abilities.Add(Ability.Vanilla);
        }
      


    }
    public void ApplyModifier()
    {
        BuffHandler.ApplyModifier();
    }
    /// <summary>
    /// Method <c>RecieveModifier</c> updates a cards stats with buffs and debuffs
    /// Rpc subfunction
    /// </summary>
    public void RecieveModifier()
    {
        attackText.text = BuffHandler.currentAttack.ToString();
        defenseText.text = BuffHandler.currentDefense.ToString();
        GameObject.Find("UIManager").GetComponent<UIManager>().DisplayPTSums();
    }

    public void GainControl()
    {
        isMine = true;
    }
    /// <summary>
    /// Method <c>HighLightSummoningSick</c> puts a red border on cards that entered play this turn to indicate they cannot yet attack
    /// Rpc subfunction
    /// </summary>
    public void HighLightSummoningSick()
    {
        if (Abilities.Contains(Ability.Haste)) { return; }
        image = gameObject.transform.Find("Highlight").GetComponent<Image>();
        var tempColor = image.color;
        tempColor = Color.red;
        image.color = tempColor;
        SummoningSick = true;
    }
    /// <summary>
    /// Method <c>HighLightSummoningSick</c> removes the red border on cards at the start of the turn and allows them to attack
    /// Rpc subfunction
    /// </summary>
    public void RemoveSummoningSick()
    {
        image = gameObject.transform.Find("Highlight").GetComponent<Image>();
        var tempColor = image.color;
        tempColor.a = 0f;
        image.color = tempColor;
        SummoningSick = false;
    }
    /// <summary>
    /// Method <c>FlipFaceDown</c> hide the card from other players; used for cards in hand
    /// Rpc subfunction
    /// </summary>
    public void FlipFaceDown()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        GameManager.shouldBeFaceDown.Add(gameObject.name);
        if (GameManager.playerHasGlassesList[NetworkClient.connection.identity.GetComponent<PlayerManager>().MyID]>0)
        {
            return;
        }
        else
        {
            image = gameObject.transform.Find("Cardback").GetComponent<Image>();
            var tempColor = image.color;
            tempColor.a = 1f;
            image.color = tempColor;
            gameObject.GetComponent<CardDisplay>().isFaceUp = false;
        }
    }
    /// <summary>
    /// Method <c>FlipFaceUp</c> reveals the card to all
    /// Rpc subfunction
    /// </summary>
    public void FlipFaceUp()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        GameManager.shouldBeFaceDown.Remove(gameObject.name);
        image = gameObject.transform.Find("Cardback").GetComponent<Image>();
        var tempColor = image.color;
        tempColor.a = 0f;
        image.color = tempColor;
        gameObject.GetComponent<CardDisplay>().isFaceUp = true;
    }


    /// <summary>
    /// Method <c>HighlightCard</c> puts a red border on a card
    /// Rpc subfunction
    /// </summary>
    public void HighlightCard()
    {
        image = gameObject.transform.Find("Highlight").GetComponent<Image>();
        var tempColor = image.color;
        tempColor.a = 1f;
        tempColor = Color.red;
        image.color = tempColor;
        isHighlighted = true;
    }
    /// <summary>
    /// Method <c>Unhighlight</c> removes any border
    /// Rpc subfunction
    /// </summary>
    public void Unhighlight()
    {
        image = gameObject.transform.Find("Highlight").GetComponent<Image>();
        var tempColor = image.color;
        tempColor.a = 0f;
        image.color = tempColor;
        isHighlighted = false;
    }
    /// <summary>
    /// Method <c>Graveyard</c> sends the card to the graveyard
    /// RPC subcommand
    /// </summary>
    public void Graveyard()
    {

        if (Shielded)
        {
            GameObject AbilityIconContainer = gameObject.transform.Find("AbilityIconContainer").gameObject;
            Shielded = false;
            int children = AbilityIconContainer.transform.childCount;
            for (int i = 0; i < children; i++)
            {
                GameObject Icon = AbilityIconContainer.transform.GetChild(i).gameObject;
                if (Icon.GetComponent<IconManager>().ability == Ability.Shielded)
                {
                    Icon.GetComponent<Image>().sprite = Icon.GetComponent<IconManager>().DamagedShielded;
                }
            }
        }
        else
        {
            LTB();
            FlipFaceUp();
            zone = "Graveyard";
            GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
           

            string targetYard = NetworkClient.connection.identity.GetComponent<PlayerManager>().Yards[ZoneSorterOuter(owner)];

            RemoveSummoningSick();
            GameObject Yard = GameObject.Find(targetYard);
            gameObject.transform.SetParent(Yard.transform, false);
            gameObject.transform.rotation = Yard.transform.rotation;

            GameManager.GetTotalPTs();
            GameObject.Find("UIManager").GetComponent<UIManager>().UpdatePlayerText();



        }

    }

    /// <summary>
    /// Method <c>AddAbility</c> adds an ability icon to the card
    /// string Ability is the name of the ability to add
    /// RPC subfunction
    /// </summary>
    public void AddAbility(Ability Ability)
    {

        GameObject newIcon = Instantiate(abilityIcon, new Vector3(0, 0, 0), Quaternion.identity);
        newIcon.transform.SetParent(gameObject.transform.Find("AbilityIconContainer"), false);
        newIcon.GetComponent<IconManager>().SetIcon(Ability);
    }
    /// <summary>
    /// Method <c>ClearIcons</c> destroys all ability icons a card has
    /// RPC subfunction
    /// </summary>
    public void ClearIcons()
    {
        GameObject AbilityContainer = gameObject.transform.Find("AbilityIconContainer").gameObject;
        int children = AbilityContainer.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            Destroy(AbilityContainer.transform.GetChild(i).gameObject);
        }
    }
    /// <summary>
    /// Method <c>ResetIcons</c> destroys all ability icons a card has then replaces them
    /// RPC subfunction
    /// </summary>
    public void ResetIcons()
    {
        ClearIcons();
        foreach(Ability ability in Abilities)
        {
            AddAbility(ability);
        }

    }
    /// <summary>
    /// Method <c>Bounce</c> returns this card to its owners hand
    /// RPC subfunction
    /// </summary>
    public void Bounce()
    {
        string targetHand = NetworkClient.connection.identity.GetComponent<PlayerManager>().Hands[ZoneSorterOuter(owner)];
        GameObject Hand = GameObject.Find(targetHand);
        if (Hand.transform.childCount < 5)
        {

            if (targetHand != "PlayerArea")
            {
                FlipFaceDown();
            }
            else
            {
                FlipFaceUp();
            }
            RemoveSummoningSick();
            gameObject.transform.SetParent(Hand.transform, false);
            LTB();
            zone = "Hand";

        }
        else
        {
            GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            GameManager.GrantDividend(owner);
            //this is called twice so if the card has 'shielded' it still goes to the graveyard
            Graveyard();
            Graveyard();
        }
    }

    /// <summary>
    /// Method <c>cETB</c> activates any enter the battlefield abilities the card has
    /// Command subcommand
    /// </summary>
    public void cETB()
    {
       
            PlayerManager PlayerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();
            GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            BuffHandler.RecieveAnthem();
            GrantTeamwork(gameObject.name);
            foreach (Ability ability in Abilities)
            {
                switch (ability)
                {
                    case Ability.Shielded:
                    //Survive 1 instance of death
                    RpcShield();
                        
                        break;
                    case Ability.Draw1:
                    //draw a card on enter

                    GameManager.TrueDrawCard(owner);
                        
                        break;
                    case Ability.Insulated:

                    //Increase the ammount of decifit you can take before recieving tax card by 1
                    RpcInsulation();
              

                        break;
                    case Ability.Edict:
                        //Destroy each player's smallest creature
                       
                            PlayerManager._edict();
                        
                        break;
                    case Ability.Trial:
                        //Destroy everything but each players largest creature
                    
                            PlayerManager.Trial();
                        
                        break;
                    case Ability.Glasses:
                    //See opponents hands
                   
                            GameManager.RpcGlasses(owner);
                        
                        break;
                    case Ability.Suicide:
                        //Debugging tool
                        Invoke("Graveyard", 1f);
                        break;
                    case Ability.Bloodthirsty:
                    //Gets bonus based on tax cards in opponents decks. OP, on backburner as is.  Not on any cards.
                    RpcBloodthirsty();
                        
                        break;
                    case Ability.Anthem:
                    //All other creatures get +1/+1
                    RpcAnthem();
                        break;
                    case Ability.Rhythm:
                    //Doubles attack if you dealt damage last turn
                    RpcRhythm();
                        break;
                    case Ability.Haste:
                    //Can attack on its first turn
                    RpcRemoveSummoningSick();
                        break;

                    case Ability.Teamwork:
                    //Gets +1/+1 when you play another creature
                    RpcTeamwork();
                        break;
                    case Ability.Beat:
                    //Enables Rhytm, but doesn't actually deal damage
                    RpcBeat();
                        break;
                    case Ability.FortifyEnhance:
                    RpcFortifyEnhance();
                        break;
                    case Ability.SpawnIce:
                        PlayerManager._SpawnToken("Ice", owner);
                        break;
                    case Ability.SpawnVernalPool:
                        PlayerManager._SpawnToken("Vernal Pool", owner);
                        break;


                }
            
        }
    }
    [ClientRpc]
    public void RpcFortifyEnhance()
    {
        GameManager.FortifyEnhanceList[owner]++;
    }
    /// <summary>
    /// Method <c>RpcBeat</c> enables the use of the Rhythm ability on the same turn
    /// It is as though you dealt damage last turn
    /// </summary>
    [ClientRpc]
    public void RpcBeat()
    {

        GameManager.LastDamageDealt[owner]++;
        foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardDisplay _card = card.GetComponent<CardDisplay>();
            _card.UpdateHandStatus();
        }

    }
    [ClientRpc]
    public void RpcTeamwork()
    {
        BuffHandler.InitiateTeamwork();
    }
    [ClientRpc]
    public void RpcRemoveSummoningSick()
    {
        RemoveSummoningSick();
    }
    [ClientRpc]
    public void RpcRhythm()
    {

        BuffHandler.UpdateRhythm();

    }
    /// <summary>
    /// Method <c>RpcAnthem</c> grants all creatures you control +1/_1
    /// </summary>
    [ClientRpc]
    public void RpcAnthem()
    {

 
        foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardDisplay _card = card.GetComponent<CardDisplay>();
            if (_card.zone == "Board" & _card.typeText.text.ToString() == "Matter" & _card.owner == owner)
            {

                _card.BuffHandler.ApplyAnthem(gameObject.name);
            }
        }
    }
    [ClientRpc]
    public void RpcBloodthirsty()
    {
        BuffHandler.UpdateBloodthirsty();
    }
    [ClientRpc]
    public void RpcShield()
    {
        Shielded = true;
    }
    /// <summary>
    /// Method <c>RpcInsulation</c> grants a point of insulation
    /// This increases the ammount of damage that has to be recieved before a penalty card is given by one
    /// </summary>
    [ClientRpc]
    public void RpcInsulation()
    {
        GameManager.playerDeficitThresholdList[owner]++;
        UIManager UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        UIManager.UpdatePlayerText();
    }
    /// <summary>
    /// Method <c>GrantTeamwork</c> checks if any other creature you control has the teamwork ability, and if they do, gives it +1/+1
    /// </summary>
    public void GrantTeamwork(string Me)
    {
        foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            if (card.name != Me)
            {
                CardDisplay _card = card.GetComponent<CardDisplay>();
                if (_card.zone == "Board" & _card.typeText.text.ToString() == "Matter" & _card.owner == owner & _card.Abilities.Contains(Ability.Teamwork))
                {
                    card.GetComponent<BuffHandler>().GainAnotherCreature();
                }
            }
        }
    }




    /// <summary>
    /// Method <c>LTB</c> activates abilities that trigger when a card leaves the Board
    /// Rpc Subfuction
    /// </summary>
    public void LTB()
    {
        PlayerManager PlayerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //This is so cards don't accidentally do their abilities when moving between other zones,
        //such as from the hand directly to the graveyard, or if a card already in the graveyard is told to go into the graveyard again for whatever reason.
        if (zone != "Board") { return; }
        if (typeText.text == "Matter")
        {
            UIManager UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
            UIManager.UpdatePlayerText();
        }
     

        foreach (Ability ability in Abilities)
        {
            switch (ability)
            {

                case Ability.Insulated:
                    GameManager.playerDeficitThresholdList[owner]--;
                    break;
                case Ability.Glasses:
                    GameManager.RpcNoGlasses(owner);
                    break;
                case Ability.Anthem:
                    foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
                    {
                        CardDisplay _card = card.GetComponent<CardDisplay>();
                        if (_card.zone == "Board" & _card.typeText.text.ToString() == "Matter" & _card.owner == owner)
                        {

                            _card.GetComponent<BuffHandler>().RemoveAnthem(gameObject.name);
                        }
                    }
                    break;
                case Ability.FortifyEnhance:
                    GameManager.FortifyEnhanceList[owner]--;
                    break;


            }
        }
    
        GameObject ZoomSquare = gameObject.transform.Find("ZoomSquare").gameObject;

        GameObject ZoomAbilityContainer = ZoomSquare.transform.GetChild(1).gameObject;

        int children = ZoomAbilityContainer.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            GameObject ability = ZoomAbilityContainer.transform.GetChild(i).gameObject;
            Destroy(ability);
        }
        gameObject.GetComponent<BuffHandler>().ResetAllBuffs();
        UpdateCard(sourceCard);
        image = ZoomSquare.GetComponent<Image>();
        var tempColor = image.color;
        tempColor.a = 0f;
        image.color = tempColor;
    }
    /// <summary>
    /// Method <c>ShrinkForComboZone</c> changes the cards apperance to just be its ability icon in the combo zone
    /// </summary>

    public void ShrinkForComboZone()
    {
        GameObject ZoomSquare = gameObject.transform.Find("ZoomSquare").gameObject;

        image = ZoomSquare.GetComponent<Image>();
        var tempColor = image.color;
        tempColor.a = 1f;
        image.color = tempColor;

        GameObject ZoomAbilityContainer = ZoomSquare.transform.GetChild(1).gameObject;
        GameObject AbilityIconContainer = gameObject.transform.Find("AbilityIconContainer").gameObject;

        int children = AbilityIconContainer.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            GameObject AbilityIcon = AbilityIconContainer.transform.GetChild(i).gameObject;
            AbilityIcon.transform.SetParent(ZoomAbilityContainer.transform, false);
        }
    }
    /// <summary>
    /// Method <c>EoT</c> activates abilities at the end of the turn
    /// int turn is the turn that is ending
    ///RPC subfunction (called by GameManager.EndOfTurnCreatureTriggers())
    /// </summary>

    public void EoT(int turn)
    {
        PlayerManager PlayerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        if (zone == "Board")
        {
            foreach (Ability ability in Abilities)
            {


                switch (ability)
                {
                    case Ability.Fortify:
                        BuffHandler.Fortify(turn);
                        break;
                    case Ability.Rhythm:
                        //BuffHandler.UpdateRhythm();
                        break;
                    case Ability.Opportunist:
                        if (GameManager.LastDamageDealt[owner] > 2 & owner == GameManager.turn)
                        {

                            if (0 == NetworkClient.connection.identity.GetComponent<PlayerManager>().MyID)
                            {
                                PlayerManager.DealACard(owner);
                            }
                        }
                        break;
                    case Ability.BlueChip:
                        if (GameManager.playerTookDamageList[owner] == 0
                            & owner != GameManager.turn)
                        {
                            GameManager.AddAp(owner);
                        }

                        break;
                    case Ability.Dash:
                        PlayerManager PM = NetworkClient.connection.identity.GetComponent<PlayerManager>();
                        GameObject TargetHand = GameObject.Find(PM.Hands[ZoneSorterOuter(owner)]);
                        if (TargetHand.transform.childCount < 5)
                        { Invoke("Bounce", .1f); }
                            break;
                    default:

                        break;
                }
            }
        }
        if (zone == "Hand")
        {
            UpdateHandStatus();
        }
        
    }
    /// <summary>
    /// Method <c>HighlightAbilityAcitve</c> highlights a card yellow
    /// used by the Rhythm ability to show the card will enter with double power
    ///RPC subfunction 
    /// </summary>
    public void HighlightAbilityAcitve()
    {
        if (isFaceUp)
        {
            image = gameObject.transform.Find("Highlight").GetComponent<Image>();
            var tempColor = image.color;
            tempColor = Color.yellow;
            image.color = tempColor;
        }
    }
    /// <summary>
    /// Method <c>ShuffleAway</c> puts this card back in its owners deck
    ///RPC subfunction 
    /// </summary>
    public void ShuffleAway()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        GameManager.playersDeckLists[owner].AddTempCard(nameText.text);
        Destroy(gameObject);
    }
    /// <summary>
    /// Method <c>UpdateHandStatus</c> shows highlights of abilities that matter in the hand
    /// Only relevant to the Rhythm ability currently
    ///RPC subfunction 
    /// </summary>
    public void UpdateHandStatus()
    {
        Unhighlight();
        if (isFaceUp&zone=="Hand")
        {
            foreach (Ability ability in Abilities)
            {
                switch (ability)
                {
                    case Ability.Rhythm:
                        if (GameManager.LastDamageDealt[owner] > 0)
                        {
                            HighlightAbilityAcitve();
                        }
                        break;
                }
            }
        }
    }
    /// <summary>
    /// Method <c>CheckOverflow</c> checks if the player has too many creatures and gets rid of one if so
    ///RPC subfunction 
    /// </summary>
    public void CheckOverflow(int player)
    {
        PlayerManager PM = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        loopvar = player;
        int i = 0;
        foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardDisplay _card = card.GetComponent<CardDisplay>();
            if (_card.zone == "Board" & _card.typeText.text.ToString() == "Matter" & _card.owner == owner)
            {

                i++;
            }
        }
        if (i > 5)
        {
            if (PM.MyID == 0)
            {
                PM.CmdSacSmallest(player);
                GameManager.GrantDividend(player);
            }
            Invoke("overflowlooper", .1f);
        }
    }
     int loopvar;
    /// <summary>
    /// Method <c>overflowlooper</c> calls checkoverflow, allowing the method to loop after the client has caught up with the server
    /// </summary>
    public void overflowlooper()
    {
        CheckOverflow(loopvar);

    }
    /// <summary>
    /// Method <c>CheckConvoke</c> checks if the player controls a creature with the caster ability
    /// return bool is true if the player controls a creature with the caster ability
    /// </summary>
    public bool CheckConvoke()
    {
        int i = 0;
        foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardDisplay _card = card.GetComponent<CardDisplay>();
            if (_card.zone == "Board" & _card.typeText.text.ToString() == "Matter" & _card.owner == owner & _card.Abilities.Contains(Ability.Caster))
            { 

                i++;
                if (i > 2)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
