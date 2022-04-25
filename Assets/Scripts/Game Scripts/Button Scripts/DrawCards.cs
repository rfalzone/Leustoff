using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class DrawCards : NetworkBehaviour
{
    public PlayerManager PlayerManager;

  
    public void OnClick()
    {
        PlayerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        PlayerManager.DealCards();
    }
    public void GrantGlasses()
    {
        PlayerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        PlayerManager.CmdGlasses(PlayerManager.MyID);
    }

}
