using UnityEngine;
using Mirror;
using LeustoffNamespace;

public class CardZoom : MonoBehaviour
{
    //Canvas is located at runtime during Awake(), whereas ZoomCard is a simple card prefab located in the inspector
    public GameObject Canvas;
    public GameObject zoomCard;
    private GameObject zoomCardObj;
    public PlayerManager PlayerManager;
    public GameObject reminderText;
    public Animator m_Animator;





    public void Awake()
    {
        Canvas = GameObject.Find("Main Canvas");
        PlayerManager = FindObjectOfType<PlayerManager>();
        m_Animator = gameObject.GetComponent<Animator>();
    }

    /// <summary>
    /// Method <c>OnHoverEnter</c> is called by the pointer enter event trigger component on this gameObject
    /// It is used to spawn the zoomed in copy of the card
    /// </summary>
    public void OnHoverEnter()
    {

        if (gameObject.GetComponent<CardDisplay>().isFaceUp == true)
        {
            zoomCardObj = PlayerManager.ZoomCopy();

            //zoomCardObj.transform.SetAsLastSibling();
            zoomCardObj.GetComponent<CardDisplay>().UpdateCard(gameObject.GetComponent<CardDisplay>().sourceCard);
            zoomCardObj.transform.SetParent(Canvas.transform, true);
            // zoomCardObj.transform.localScale = Vector3.one * PlayerManager.zoomfactor;
            zoomCardObj.layer = LayerMask.NameToLayer("Zoom");
            zoomCardObj.name = "ZOOMCARD";
           
            RectTransform rect = zoomCardObj.GetComponent<RectTransform>();
            if (Input.mousePosition.y <= Screen.height / 2)
            {
                rect.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y + Screen.height / 3, Input.mousePosition.z);
            }
            else if (Input.mousePosition.y > Screen.height / 2)
            {
                rect.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y - Screen.height / 3, Input.mousePosition.z);
            }
            else
            {
                Debug.Log("The screen height thing in the card zoom is broken I guess");
            }
            zoomCardObj.GetComponent<CardZoom>().m_Animator.SetBool("IsHover", true);
            rect.localScale = new Vector3(3, 3, 1);

            CardDisplay _card = gameObject.GetComponent<CardDisplay>();
            foreach (Ability ability in _card.Abilities)
            {

                AddAbility(ability);
            }
        }


    }

    /// <summary>
    /// Method <c>OnHoverEnter</c> is called by the pointer exit event trigger component on this gameObject
    /// It is used to clear the zoom copy
    /// </summary>
    public void OnHoverExit()
    {
        Destroy(zoomCardObj);
    }
    /// <summary>
    /// Method <c>AddAbility</c> adds ability icons the zoom copy as well as reminder text
    /// It uses two reminder text containers to keep the text on the screen if the zoom copy is near one edge or the other
    /// string Ability is the name of the ability to add
    /// </summary>
    public void AddAbility(Ability Ability)
    {
        GameObject newIcon = Instantiate(reminderText, new Vector3(0, 0, 0), Quaternion.identity);
        newIcon.GetComponent<ReminderTextManager>().myType = ObjType.Card;
        if (Input.mousePosition.x <= Screen.width / 2)
        {
            newIcon.transform.SetParent(zoomCardObj.transform.Find("ReminderTextContainer"), false);
        }
        else
        {
            newIcon.transform.SetParent(zoomCardObj.transform.Find("LeftReminderTextContainer"), false);
        }
     
        newIcon.GetComponent<ReminderTextManager>().SetText(Ability);
    }
    
}
