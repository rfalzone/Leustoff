using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using LeustoffNamespace;

public class Attack : NetworkBehaviour
{
    public PlayerManager PlayerManager;
    public GameManager GameManager;
    private string phase;
    private string subphase;

    //I'm just doing this coding stuff until I can get my startup off the ground.  I'm trying to break into potato farming logistics.
    private void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    private int neighborID;
    public int ZoneSorterOuter(int PlayerID)
    {
        int localID = NetworkClient.connection.identity.GetComponent<PlayerManager>().MyID;
        var gribbit = GameManager.PlayerID - localID;
        var gaboodle = gribbit + PlayerID;
        var grobbit = mod(gaboodle, GameManager.PlayerID);
        return grobbit;
    }
    public int mod(int x, int m)
    {
        return (x % m + m) % m;
    }
    /// <summary>
    /// Method <c>OnClick</c> is called by clicking the attack button
    /// it checks that it is your turn then starts the attack process
    /// </summary>
    public void OnClick()
    {

        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        PlayerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();
       
        if (PlayerManager.MyID != GameManager.turn) { return; }

        PlayerManager.CMDAttack();
    }
    /// <summary>
    /// Method <c>DoAttack</c> is the meat of the attack function
    /// Command subcommand
    /// </summary>
    public void DoAttack()
    {

        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        PlayerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        //The players IDs match  the turn numbers 1:1
        int MyID = GameManager.turn;
        //When players are assigned an ID, they copy GameManager.PlayerID then increment it by one.  This means that player 1 ID is 0, and so on.
        //This also means that at any time GameManager.PlayerID is equal to the number of connected players.
        int numPlayers = GameManager.PlayerID;
        GameObject TargetHand = GameObject.Find(PlayerManager.Hands[ZoneSorterOuter(GameManager.turn)]);
        int HandSize = TargetHand.transform.childCount;
        PlayerManager.MaxCarddraw = 5 - HandSize;
        

        List<int> Targets = new List<int>();

        int bigHit = -1;
        PlayerManager.CmdBuildDamageList(GameManager.turn);

        for (int i = 0; i < numPlayers - 1; i++)
        {
            int TargetID = (i + MyID + 1) % numPlayers;
            int Hit = GameManager.CurrentAttack[MyID] - GameManager.TotalDefense[TargetID];

            if (Hit > bigHit)
            {
                bigHit = Hit;
                Targets.Clear();
                Targets.Add(TargetID);
            }
            else if (Hit == bigHit)
            {
                Targets.Add(TargetID);
            }
        }
            for(int j = 0; j< bigHit; j++)
            {
                GameManager.RpcAddAp(MyID);
                foreach(int target in Targets)
                {
                GameManager.RpcDealDeficit(target, GameManager.turn);
                }
            }
        PlayerManager.CmdPassTurn();
        PlayerManager.CmdUpdateUI();
    }
        
}
