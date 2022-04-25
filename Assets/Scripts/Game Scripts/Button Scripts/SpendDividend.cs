using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpendDividend : NetworkBehaviour
{
    public void OnClick()
    {
        GameManager GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        PlayerManager PlayerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        if (GameManager.playerDividendList[PlayerManager.MyID] > 0)
        {
            PlayerManager.CmdLotusPetal(PlayerManager.MyID);
        }
        
    }

}
