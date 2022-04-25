using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System;
using System.Threading;
using TMPro;
public class UIManager : NetworkBehaviour
{
    public PlayerManager PlayerManager;
    public GameManager GameManager;
    public GameObject Button;
    public GameObject PlayerText;
    
    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
       
    }
    int mod(int x, int m)
    {
        return (x % m + m) % m;
    }
    /// <summary>
    /// Method <c>RpcUpdateUI</c> calls update player text after a moment, to let clients catch up with the host
    /// </summary>
    [ClientRpc]
    public void RpcUpdateUI()
    {
        Invoke("UpdatePlayerText", .1f);
    }
    /// <summary>
    /// Method <c>UpdatePlayerText</c> pulls info from the game manager to update many UI elements
    /// Many of these texed based elements have now been removed from the UI
    /// This function still calls many smaller functions that handle individual UI elements
    /// Rpc Subfunction
    /// </summary>
    public void UpdatePlayerText()
    {
   
        GameObject turntracker = GameObject.Find("Turn");
        
        GameObject EnemyTexts = GameObject.Find("EnemyTexts");
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        PlayerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        turntracker.GetComponent<Text>().text = "It is player "+GameManager.turn.ToString()+"'s turn";
        if (PlayerManager.MyID == GameManager.turn)
        {
            
            turntracker.GetComponent<Text>().color = Color.green;
        }
        else
        {
            turntracker.GetComponent<Text>().color = Color.red;
        }
        int playersID = PlayerManager.MyID;
        int numPlayers = GameManager.PlayerID;
       if (playersID < 0)
        {
            playersID = 0;
        }
        PlayerText.GetComponent<Text>().text =
            GameManager.playerRevenueList[playersID] + "/"+ GameManager.playerRevenueThresholdList[playersID] +"\n"
            + GameManager.playerDeficitList[playersID]+ "/" + GameManager.playerDeficitThresholdList[playersID] + "\n"
            + GameManager.currentManaList[playersID] + "/"+ GameManager.playerManaList[playersID] + "\n"
            +GameManager.playerDividendList[playersID]
            +"\nID: " + playersID;
        if (numPlayers > 1)
        {


           
            int texts = EnemyTexts.transform.childCount;

            for (int j = 0; j < texts; j++)
            {
                
                var relevantID = (playersID + j +1)% numPlayers;
                EnemyTexts.transform.GetChild(j).GetComponent<Text>().text =
                GameManager.playerRevenueList[relevantID] + "/" + GameManager.playerRevenueThresholdList[relevantID]+ "\n"
                + GameManager.playerDeficitList[relevantID] +"/" + GameManager.playerDeficitThresholdList[relevantID]+ "\n"
                + GameManager.currentManaList[relevantID] + "/" + GameManager.playerManaList[relevantID] +
                "\n" + GameManager.playerDividendList[relevantID];
            }
       
        }
        GameObject.Find("DividendText").GetComponent<TextMeshProUGUI>().text = GameManager.playerDividendList[playersID].ToString();
            DisplayPTSums();
            DisplayManaCrystals();
        if (GameManager.globalturn > 0)
        {
            UpdateDeckcounter();
        }
        FillPointGauges();
        SetComboPips();
        SetRevenueButtonColor();
    }
    public void FillPointGauges()
    {
        float fillpercentage = 1.00f / GameManager.playerRevenueThresholdList[PlayerManager.MyID];
        GameObject.Find("PlayerRevenueFill").GetComponent<Image>().fillAmount = GameManager.playerRevenueList[PlayerManager.MyID] * fillpercentage;
        GameObject.Find("PlayerRevenueBar").GetComponent<CombatGauges>().SetSprite(6-GameManager.playerRevenueThresholdList[PlayerManager.MyID], true);
        GameObject.Find("PlayerDamageBar").GetComponent<CombatGauges>().SetSprite((GameManager.playerDeficitThresholdList[PlayerManager.MyID] - 3), false);
        fillpercentage = 1.00f / GameManager.playerDeficitThresholdList[PlayerManager.MyID];
        GameObject.Find("PlayerDamageFill").GetComponent<Image>().fillAmount = GameManager.playerDeficitList[PlayerManager.MyID] * fillpercentage;
        List<string> EnemyRevenues = new List<string> { "RevenueBar0", "RevenueBar1", "RevenueBar2", "EnemyFill0", "EnemyFill1", "EnemyFill2", "DamageBar0", "DamageBar1", "DamageBar2", "DamageFill0", "DamageFill1", "DamageFill2" };

        int numPlayers = GameManager.PlayerID;

        for (int i = 0; i < GameManager.PlayerID-1; i++)
        {
           var relevantID = (PlayerManager.MyID + i + 1) % numPlayers;
            if (relevantID < numPlayers)
            {
                GameObject.Find(EnemyRevenues[i]).GetComponent<CombatGauges>().SetSprite(6 - GameManager.playerRevenueThresholdList[relevantID], true);
                 fillpercentage = 1.00f / GameManager.playerRevenueThresholdList[relevantID];
                GameObject.Find(EnemyRevenues[i+3]).GetComponent<Image>().fillAmount = GameManager.playerRevenueList[relevantID] * fillpercentage;
                GameObject.Find(EnemyRevenues[i+6]).GetComponent<CombatGauges>().SetSprite((GameManager.playerDeficitThresholdList[relevantID]-3), false);
                fillpercentage = 1.00f / GameManager.playerDeficitThresholdList[relevantID];
                GameObject.Find(EnemyRevenues[i + 9]).GetComponent<Image>().fillAmount = GameManager.playerDeficitList[relevantID] * fillpercentage;
            }
        }
    }
    /// <summary>
    /// Method <c>DisplayManaCrystals</c> spawns and refils mana crystal objects
    /// RPC subfunction
    /// </summary>
    public void DisplayManaCrystals()
    {
        int LocalID = NetworkClient.connection.identity.GetComponent<PlayerManager>().MyID;
        GameObject ManaHolder = GameObject.Find("ManaHolder");
        int TotalMana = GameManager.playerManaList[LocalID];
        int CurrentMana = GameManager.currentManaList[LocalID];
        int i = ManaHolder.transform.childCount;
        for (int j = 0; j < i; j++)
        {
            Destroy(ManaHolder.transform.GetChild(j).gameObject);
        }
        for (int k = 0; k < TotalMana; k++)
        {
            GameObject FreshCrystal = Instantiate(GameManager.ManaCrystal, new Vector2(0, 0), Quaternion.identity);
            FreshCrystal.transform.SetParent(ManaHolder.transform, false);
            FreshCrystal.GetComponent<ManaCrystal>().Empty();
            if (k < CurrentMana)
            {
                FreshCrystal.GetComponent<ManaCrystal>().Charge();
            }
        }
      
    }
    /// <summary>
    /// Method <c>SetComboPips</c> adds the green pips to the tracker on the left that shows the player victory points
    /// int ComboTotal is how many points the player has
    /// int PlayerID is the players ID
    /// RPCSubcommand
    /// </summary>
    public void SetComboPips()
    {
      
            //This sets up the vertical stack style UI element.
            /*
            GameObject ComboCounter = GameObject.Find("ComboCounter");
            int i = ComboCounter.transform.childCount;
            for(int j = 0; j < i; j++)
            {
                Destroy(ComboCounter.transform.GetChild(j).gameObject);
            }
            for(int k = 0; k < ComboTotal; k++)
            {
                GameObject NewPoint = Instantiate(GameManager.ComboPoint, new Vector2(0, 0), Quaternion.identity);
                NewPoint.transform.SetParent(ComboCounter.transform,false);
            }
            if (GameManager.PlayerScoreList[PlayerID] > 0)
            {
                for (int k = 0; k < 7; k++)
                {
                    GameObject NewPoint = Instantiate(GameManager.ComboPoint, new Vector2(0, 0), Quaternion.identity);
                    NewPoint.transform.SetParent(ComboCounter.transform, false);
                }
            }
            */
        GameObject PointsSpinner = GameObject.Find("PointsSpinner");
        GameObject PointsSpinner2 = GameObject.Find("PointsSpinner2");
        PointsSpinner.transform.eulerAngles = Vector3.forward * (180-30* GameManager.playerComboPowerList[PlayerManager.MyID]);
        PointsSpinner2.transform.eulerAngles = Vector3.forward * (30 * GameManager.playerComboPowerList[PlayerManager.MyID]);

        if (GameManager.PlayerScoreList[PlayerManager.MyID] > 0)
        {
            PointsSpinner.transform.eulerAngles = Vector3.forward * (0);
            PointsSpinner2.transform.eulerAngles = Vector3.forward * (180);
            GameObject WinnerSpinner = GameObject.Find("WinnerSpinner");
            GameObject WinnerSpinner2 = GameObject.Find("WinnerSpinner2");
            WinnerSpinner2.transform.eulerAngles = Vector3.forward * (30 * GameManager.playerComboPowerList[PlayerManager.MyID]);
            WinnerSpinner.transform.eulerAngles = Vector3.forward * (180 - 30 * GameManager.playerComboPowerList[PlayerManager.MyID]);
            GameObject PowerCore = GameObject.Find("PowerCore");
            GameObject PowerCore2 = GameObject.Find("PowerCore2");
            Image image;
            image = PowerCore.GetComponent<Image>();
            var tempColor = image.color;
            tempColor.a = 1f;
            image.color = tempColor;
            image = PowerCore2.GetComponent<Image>();
            tempColor = image.color;
            tempColor.a = 1f;
            image.color = tempColor;
        }
        for (int i =0; i < GameManager.PlayerID-1; i++)
        {
            
            int relevantID = (PlayerManager.MyID + i + 1) % GameManager.PlayerID;
            if (GameManager.PlayerScoreList[relevantID] == 0)
            {
                GameObject.Find("E" + i + "ComboFiller1").GetComponent<Image>().fillAmount = 1.00f * GameManager.playerComboPowerList[relevantID] / 7;
                GameObject.Find("E" + i + "ComboFiller2").GetComponent<Image>().fillAmount = 0f;
            }
            else
            {
                GameObject.Find("E" + i + "ComboFiller1").GetComponent<Image>().fillAmount = 1.00f;
                GameObject.Find("E" + i + "ComboFiller2").GetComponent<Image>().fillAmount = 1.00f * GameManager.playerComboPowerList[relevantID] / 7;
            }
        }

    }
    /// <summary>
    /// Method <c>DisplayPTSums</c> shows each player's total power and toughness
    /// RPCSubcommand
    /// </summary>
    public void DisplayPTSums()
    {
        GameManager.GetTotalPTs();
        GameObject EPTText = GameObject.Find("EnemyPTtexts");
        GameObject MyPT = GameObject.Find("TotalPower0");
        int playersID = PlayerManager.MyID;
        if (playersID < 0)
        {
            playersID = 0;
        }
        int numPlayers = GameManager.PlayerID;
        MyPT.GetComponent<Text>().text = GameManager.TotalAttack[playersID] + "/" + GameManager.TotalDefense[playersID];
        
        var relevantID = 0;
        for (int i = 0; i < numPlayers-1; i++)
        {
             relevantID = (playersID + i + 1) % numPlayers;
            
            if (relevantID < numPlayers)
            {
                Text _text = EPTText.transform.GetChild(i * 2).GetComponent<Text>();
                _text.text = GameManager.TotalAttack[relevantID].ToString();
                _text = EPTText.transform.GetChild(i*2+1).GetComponent<Text>();
                _text.text = GameManager.TotalDefense[relevantID].ToString();
            }

        }
        //New UI Functions
        GameObject.Find("PlayerAttackText").GetComponent<TextMeshProUGUI>().text = "End\n" + GameManager.TotalAttack[playersID];
        GameObject OpDefs = GameObject.Find("OpDefs");
        GameObject OpAtks = GameObject.Find("OpAtks");
        GameObject ThreatHighlight = GameObject.Find("ThreatHighlight");
        for (int i = 0; i < numPlayers-1; i++)
        {
            relevantID = (playersID + i + 1) % numPlayers;
            if (relevantID < numPlayers)
            {
                OpDefs.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = GameManager.TotalDefense[relevantID].ToString();
                OpAtks.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = GameManager.TotalAttack[relevantID].ToString();
            }
        }
   
        Image image;
        int predictedDmg=0;
        for (int i = 0; i < GameManager.PlayerID-1; i++)
        {

            bool amLowest = true;

            for (int j =0; j < GameManager.PlayerID-1; j++)
            {
                if (j != i)
                {
         
                   
                    relevantID = (playersID + j + 1) % numPlayers;
                    if (GameManager.TotalDefense[playersID] > GameManager.TotalDefense[relevantID])
                    {
                        amLowest = false;
                    }
                }
            }
            if (amLowest)
            {
                relevantID = (playersID + i + 1) % numPlayers;
                if ((GameManager.TotalAttack[relevantID] - GameManager.TotalDefense[playersID]) > 0)
                {
                    predictedDmg += (GameManager.TotalAttack[relevantID] - GameManager.TotalDefense[playersID]);
                }
                image =ThreatHighlight.transform.GetChild(i).GetComponent<Image>();
                var tempColor = image.color;
                tempColor = Color.red;
                image.color = tempColor;
            }
            else
            {
                 image=ThreatHighlight.transform.GetChild(i).GetComponent<Image>();
                var tempColor = image.color;
                tempColor.a = 0f;
                image.color = tempColor;
            }
        }


        ShowHotseat();
        ShowDamageCalc(predictedDmg);
    }
    /// <summary>
    /// Method <c>ShowHotseat</c> puts a hotseat icon near the player or players with the lowest defense
    /// Rpc Subfunction
    /// </summary>
    public void ShowHotseat()
    {
        GameObject Hotseats = GameObject.Find("HotSeats");
        List<int> HotseatIDs = new List<int>();
        int lowval = 10000;
        for(int i = 0; i < GameManager.PlayerID; i++)
        {
            Hotseats.transform.GetChild(i).gameObject.SetActive(false);
            if (GameManager.TotalDefense[i] < lowval)
            {
                lowval = GameManager.TotalDefense[i];
                HotseatIDs.Clear();
                HotseatIDs.Add(i);
            }
            else if(GameManager.TotalDefense[i] == lowval)
            {
                HotseatIDs.Add(i);
            }
        }
        

        foreach (int j in HotseatIDs)
        {
            Hotseats.transform.GetChild(j).gameObject.SetActive(true);
        }
    }
    public void ShowDamageCalc(int predictedDmg)
    {
        GameObject CalcAttk = GameObject.Find("PlayerAttackText");
        GameObject.Find("PlayerDefenseText").GetComponent<TextMeshProUGUI>().text =predictedDmg.ToString();
        int playersID = PlayerManager.MyID;
        int damageToDeal = 0;
        for (int i = 0; i < GameManager.PlayerID; i++)
        {
            if (i != playersID)
            {
                int aAttack = GameManager.CurrentAttack[playersID] - GameManager.TotalDefense[i];
               if (aAttack > damageToDeal)
                {
                    damageToDeal = aAttack;
                }
            }
        }
        if (damageToDeal > 0) {
            CalcAttk.GetComponent<TextMeshProUGUI>().text = "Generate Revenue\n" + damageToDeal;
        }
        else
        {
            CalcAttk.GetComponent<TextMeshProUGUI>().text = "Pass Turn";
        }


    }
    [ClientRpc]
    public void RpcDisplayPointsSums()
    {
        DisplayPointsSums();
    }
    /// <summary>
    /// Method <c>DisplayPointsSums</c> shows all players total combo points
    /// Rpc Subfunction
    /// </summary>
    public void DisplayPointsSums()
    {
        int playersID = PlayerManager.MyID;

        GameManager.GetTotalPTs();
        GameObject EPTText = GameObject.Find("EnemyPoints");
        GameObject MyPT = GameObject.Find("Points0");
        if (playersID < 0)
        {
            playersID = 0;
        }
        int numPlayers = GameManager.PlayerID;
        MyPT.GetComponent<Text>().text = GameManager.playerComboPowerList[playersID].ToString();

        int children = EPTText.transform.childCount;

        for (int i = 0; i < children; i++)
        {
            var relevantID = (playersID + i + 1) % numPlayers;
            Text _text = EPTText.transform.GetChild(i).GetComponent<Text>();
            if (relevantID < numPlayers)
            {
                int totalpoints;
               totalpoints  = GameManager.playerComboPowerList[relevantID];
                totalpoints += GameManager.PlayerScoreList[relevantID] * 7;
                _text.text = totalpoints.ToString();
            }

        }
        int localID = NetworkClient.connection.identity.GetComponent<PlayerManager>().MyID;
        SetComboPips();
        Invoke("SetComboPips", .25f);

    }
    public void UpdateButtonText(string gameState)
    {
        Button = GameObject.Find("Button");
        Button.GetComponentInChildren<Text>().text = gameState;
    }
    /// <summary>
    /// Method <c>ShowDamageNumbers</c> shows the amount of damage the player dealt, and who recieved that damage
    /// int turn is the attacking player's ID
    /// Rpc Subfunction
    /// </summary>
    public void ShowDamageNumbers(int turn)
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        PlayerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        int playersID = PlayerManager.MyID;
        int numPlayers = GameManager.PlayerID;
        GameObject revenueText = GameObject.Find("RevenueText");
        GameObject damageTexts = GameObject.Find("DamageTexts");
        revenueText.GetComponent<Text>().text = GameManager.thisTurnPoints[playersID].ToString();
        if (playersID == turn)
        {
            revenueText.GetComponent<DamageNumbers>().GotPoints();
        }
        else
        {
            revenueText.GetComponent<DamageNumbers>().GotHit();
        }
        
        
        int texts = damageTexts.transform.childCount;
        for (int j = 0; j < texts; j++)
        {

            var relevantID = (playersID + j + 1) % numPlayers;
            damageTexts.transform.GetChild(j).GetComponent<Text>().text = GameManager.thisTurnPoints[relevantID].ToString();
            
           
            if (GameManager.thisTurnPoints[relevantID] > 0)
            {
                if (relevantID == turn)
                {
                    damageTexts.transform.GetChild(j).GetComponent<DamageNumbers>().GotPoints();
                }
                else
                {
                    damageTexts.transform.GetChild(j).GetComponent<DamageNumbers>().GotHit();
                }
           
            }
        }



    }
    /// <summary>
    /// Method <c>ShowDamageNumbers</c> activates the pop up text
    /// currently only used for counterspells, which are not currently in use
    /// string eventText is the text to be displayed (only accepts predetermined phrases)
    /// Rpc Subfunction
    /// </summary>
    public void DisplayEventText(string eventText)
    {
        GameObject EventText = GameObject.Find("EventPopupText");
        string text = EventText.GetComponent<Text>().text = eventText;
        switch (eventText)
        {
            case "Counterspell":
                EventText.GetComponent<EventPopupTextManager>().Counterspell();
                break;
            default:
                EventText.GetComponent<EventPopupTextManager>().Counterspell();
                break;
        }
        
    }
    [ClientRpc]
    public void RpcUpdateDeckcounter()
    {

        UpdateDeckcounter();
    }
    /// <summary>
    /// Method <c>UpdateDeckcounter</c> updates UI elements which show total deck counts, as well as number of penalty cards
    /// Rpc Subfunction
    /// </summary>
    public void UpdateDeckcounter()
    {
        
        PlayerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        int numPlayers = GameManager.PlayerID;
        int playersID = PlayerManager.MyID;

        GameObject PlayerDeckcount = GameObject.Find("CardsInDeck0");
        
        PlayerDeckcount.GetComponent<Text>().text = GameManager.playersDeckLists[playersID].CurrentDecklist.Count + "\n" + GameManager.taxTotalList[playersID];
        
        if (numPlayers > 3)
        {

            GameObject EnemyCardsInDeck = GameObject.Find("EnemyCardsInDeck");

            int playercounts = EnemyCardsInDeck.transform.childCount;

            for (int j = 0; j < playercounts; j++)
            {

                var relevantID = (playersID + j + 1) % numPlayers;
                EnemyCardsInDeck.transform.GetChild(j).GetComponent<Text>().text = GameManager.playersDeckLists[relevantID].CurrentDecklist.Count.ToString()+"\n" + GameManager.taxTotalList[relevantID];
            }
            
        }
    }
    /// <summary>
    /// Method <c>SetRevenueButtonColor</c> sets the pass turn button to yellow on your turn and blue when it is not your turn
    /// </summary>
    public void SetRevenueButtonColor()
    {
        GameObject RevenueBox = GameObject.Find("Player Attack");
        Image image = RevenueBox.GetComponent<Image>();
        var tempColor = image.color;
        if(GameManager.turn== NetworkClient.connection.identity.GetComponent<PlayerManager>().MyID)
        {
            tempColor = Color.yellow;
        }
        else
        {
            tempColor = Color.blue;
        }
        image.color = tempColor;
    }
}
