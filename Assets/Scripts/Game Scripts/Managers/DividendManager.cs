using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class DividendManager : MonoBehaviour
{
    GameManager GameManager;
    PlayerManager PlayerManager;
    public int DrawCost=2;
    public int FortifyCost=3;
    public int ComboCost=5;
    /// <summary>
    /// Method <c>ClickDraw</c> is called when the draw dividend button is clicked
    /// Calls the DividendDraw function
    /// </summary>
    
    public void ClickDraw()
    {

        if (GameObject.Find("PlayerArea").transform.childCount >= 5)   { return; }

        PlayerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        DividendDraw(PlayerManager.MyID);
    }
    /// <summary>
    /// Method <c>ClickReinforce</c> is called when the reinforce dividend button is clicked
    /// Calls the DividendReinforce function
    /// </summary>
    public void ClickReinforce()
    {

        PlayerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        DividendReinforce(PlayerManager.MyID);
    }
    /// <summary>
    /// Method <c>ClickCombo</c> is called when the combo dividend button is clicked
    /// Calls the DividendCombo function
    /// </summary>
    public void ClickCombo()
    {
        PlayerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        if (GameObject.Find("PlayerArea").transform.childCount >= 5){ return; }
        DividendCombo(PlayerManager.MyID);
    }
    /// <summary>
    /// Method <c>DividendDraw</c> spends 2 dividends to draw a card
    /// int PlayerID is the player doing this action
    /// </summary>
    public void DividendDraw(int PlayerID)
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (GameManager.playerDividendList[PlayerID] >= DrawCost)
        {
            PlayerManager.CmdDrawCard(PlayerID);
            PlayerManager.CmdSpendDividend(PlayerID, DrawCost);
        }
    }
    /// <summary>
    /// Method <c>DividendReinforce</c> spends 4 dividends to give a permanent +1/+1
    /// int PlayerID is the player doing this action
    /// </summary>
    public void DividendReinforce(int PlayerID)
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        {
            if (GameManager.playerDividendList[PlayerID] >= FortifyCost)
            {
                PlayerManager.CmdSpendDividend(PlayerID, FortifyCost);
                PlayerManager.CmdReinforce(PlayerID);
            }
        }
    }
    /// <summary>
    /// Method <c>DividendReinforce</c> spends 6 dividends to give a combo point
    /// int PlayerID is the player doing this action
    /// </summary>
    public void DividendCombo(int PlayerID)
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        {
            if (GameManager.playerDividendList[PlayerID] >= ComboCost)
            {
                PlayerManager.CmdSpendDividend(PlayerID, ComboCost);
                PlayerManager.CmdBuyCombo(PlayerID);
            }
        }

    }
}
