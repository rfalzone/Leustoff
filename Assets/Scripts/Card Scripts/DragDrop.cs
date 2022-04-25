using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using LeustoffNamespace;

public class DragDrop : NetworkBehaviour
{
    public GameManager GameManager;
    public GameObject Canvas;
    public GameObject Graveyard;
    public GameObject DropZone;
    public PlayerManager PlayerManager;
    private bool isDragging = false;
    private bool isOverLandZone = false;
    private bool isOverGraveyard = false;
    private GameObject dropZone;
    private GameObject startParent;
    private Vector2 startPosition;
    private Quaternion startRotation;
    public int j = 100;
    public GameObject landZone;
    public GameObject playerBoard;
    public GameObject hit;
    public GameObject hitmarker;
    public GameObject SpellManager;
    public Animator m_Animator;
    public bool beingCast=false;
    private void Start()
    {
        Canvas = GameObject.Find("Main Canvas");
        landZone = GameObject.Find("LandZone");
        playerBoard = GameObject.Find("PlayerBoard");
        m_Animator = gameObject.GetComponent<Animator>();
        Collider2D myCollider = gameObject.GetComponent<Collider2D>();
        Graveyard = GameObject.Find("PlayerYard");
    }
    /// <summary>
    /// Method <c>Update</c> is called every frame
    /// Its how cards you are dragging stay with your mouse
    /// </summary>
    void Update()
    {
        if (isDragging)
        {
            gameObject.transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            gameObject.transform.SetParent(Canvas.transform, true);
        }
    }

    /// <summary>
    /// Method <c>OnCollisionEnter2D</c> is used to detect if a dragged card is over the play area
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
       
            Physics2D.IgnoreLayerCollision(7, 7);
        

        if (collision.gameObject == landZone)
        {
            isOverLandZone = true;
            dropZone = collision.gameObject;
        }
        if (collision.gameObject == Graveyard)
        {
            isOverGraveyard = true;
            gameObject.GetComponent<CardDisplay>().HighlightCard();

        }

    }

    /// <summary>
    /// Method <c>OnCollisionExit2D</c> is used to detect if a dragged card is no longer over the play area
    /// </summary>
    private void OnCollisionExit2D(Collision2D collision)
    {
        dropZone = null;
        isOverLandZone = false;
        isOverGraveyard = false;
        gameObject.GetComponent<CardDisplay>().UpdateHandStatus();

    }

    /// <summary>
    /// Method <c>StartDrag</c> is called when a card starts being dragged
    /// There are many guard statements here to prevent dragging in a number of cases
    /// </summary>
    public void StartDrag()
    {
        PlayerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Destroy(GameObject.Find("ZOOMCARD"));
        if (gameObject.GetComponent<CardDisplay>().zone != "Hand") return;
        if (PlayerManager.MyID != GameManager.turn) return;
        if (!gameObject.GetComponent<CardDisplay>().isMine) return;
        isDragging = true;
        startParent = transform.parent.gameObject;
        PlayerManager.DragDropParent = startParent.name;
        startPosition = transform.position;
        startRotation = transform.rotation;
        m_Animator.SetBool("BeingDragged", true);
    }

    /// <summary>
    /// Method <c>EndDrag</c> is called when a card stops being dragged
    /// Triggers a card being played
    /// </summary>
    public void EndDrag()
    {
        m_Animator.SetBool("BeingDragged", false);
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        Destroy(GameObject.Find("ZOOMCARD"));
        PlayerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        if (!isDragging) return;
        if (!gameObject.GetComponent<CardDisplay>().isMine) return;
        if (PlayerManager.MyID != GameManager.turn) return;
        isDragging = false;
        if (isOverLandZone)
        {
            PlayerManager.PlayCard(gameObject.name);
        }
        if (isOverGraveyard)
        {
            PlayerManager.CmdGainDividend();
            gameObject.GetComponent<CardDisplay>().Graveyard();
        }
    


        else
        {

            ReturnToSender();

        }
    }
    /// <summary>
    /// Method <c>CancelCast</c> is part of cancelling a card being cast
    /// </summary>
    public void CancelCast()
    {
        PlayerManager.CmdCancelCast(gameObject.name);
        
        PlayerManager.spellcasting = false;
        PlayerManager.CmdUnPayMana(PlayerManager.MyID);
    }
    /// <summary>
    /// Method <c>RpcReturnToSender</c> sends a card back to your hand.  Called when a card that  is not valid to be played is dragged and dropped by a player
    /// </summary>
 
    [ClientRpc]
    public void RpcReturnToSender(int player)
    {
        //The casting process needs to be validated by the server, so this is sent back to the clients as an RPC, but only effects the player actually playing the card
        if (player == NetworkClient.connection.identity.GetComponent<PlayerManager>().MyID)
        {
            PlayerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();

            startParent = GameObject.Find(PlayerManager.DragDropParent);
            ReturnToSender();
        }
    }
    /// <summary>
    /// Method <c>ReturnToSender</c> sends a card back to your hand.  This is the part that happens on the local client.
    /// </summary>
    public void ReturnToSender()
    {
    
            transform.position = startPosition;

            transform.SetParent(startParent.transform, false);
            transform.rotation = startRotation;
        
    }
    /// <summary>
    /// Method <c>OnClick</c> looks at the gamestate and finds out what clicking on a  card means given that context
    /// </summary>
    public void OnClick()
    {

        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        PlayerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        //If the targeted casting process is in effect, checks if this card is a valid target
        //If this card is the card being cast itself, cancels the cast and returns it to hand
        if (PlayerManager.spellcasting)
        {
            Spellcast();
            return;
        }
        //If the card is in the graveyard, expands that graveyard in the UI
        if (gameObject.GetComponent<CardDisplay>().zone == "Graveyard" &gameObject.GetComponent<CardDisplay>().isPoppedOut == false)
        {
            gameObject.transform.parent.GetComponent<Graveyard>().OnClick();

        }
        //If this card is in an opponents hand, expands that hand for inspection in the UI.  Only relevant for the 'glasses' ability which lets you see enemy hands
        else if (gameObject.GetComponent<CardDisplay>().zone=="Hand"&
            NetworkClient.connection.identity.GetComponent<PlayerManager>().MyID != gameObject.GetComponent<CardDisplay>().owner& gameObject.GetComponent<CardDisplay>().isPoppedOut == false)
        {
            gameObject.transform.parent.GetComponent<PlayerAreaAnimManager>().OnClick();

        }
        //Puts cards in expanded windows back to where they go
        else if (gameObject.GetComponent<CardDisplay>().isPoppedOut == true)
        {
            gameObject.transform.parent.transform.parent.GetComponent<closewindow>().OnClick();

        }




    }
    //Handles the casting of targeted spells
    public void Spellcast()
    {
        PlayerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        CardDisplay _card = gameObject.GetComponent<CardDisplay>();
        if (_card.zone == "Stack" & _card.owner == PlayerManager.MyID)
        {
            CancelCast();
        }
        if (_card.zone == "Board")
        {
            if (_card.Abilities.Contains(Ability.Hexproof))
            {
                return;

            }
            
             else            
            {
                PlayerManager.ResolveSpell(gameObject.name, PlayerManager.MyID);
            }
            
        }
    }

}
