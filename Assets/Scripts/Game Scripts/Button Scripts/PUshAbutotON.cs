using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System;
using System.Threading;
using UnityEngine.SceneManagement;
/// <summary>
/// This is a debugging class that's tied to a button hidden in the scene
/// mostly just dummy testing code in here pay it no mind
/// </summary>
public class PUshAbutotON : NetworkBehaviour
{
    public GameObject CardMain;
    NetworkManager NetworkManager;
    [SyncVar]
    public int buttonnumber = 0;
    public string buttontext;
    public List<int> dalist = new List<int>();
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.GetChild(0).GetComponent<Text>().text = buttonnumber.ToString();
        dalist.Add(0);
       
    }

    
    public void OnClick()
    {
        int LocalID = NetworkClient.connection.identity.GetComponent<PlayerManager>().MyID;
        GameManager GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        GameObject ManaHolder = GameObject.Find("ManaHolder");
        int CurrentMana = GameManager.currentManaList[LocalID];
        for (int l = 0; l < CurrentMana; l++)
        {
            ManaHolder.transform.GetChild(l).gameObject.GetComponent<ManaCrystal>().Charge();
        }


    }

    [Command(requiresAuthority = false)]
    public void numberplus()
    {
        buttonnumber++;
        
        spaghet();
        
    }
   
    [ClientRpc]
    void spaghet()
    {
        Debug.Log(buttonnumber);
        Thread.Sleep(100);
        gameObject.transform.GetChild(0).GetComponent<Text>().text = buttonnumber.ToString();
    }

}
