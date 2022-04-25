using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPopupTextManager : MonoBehaviour
{
    public Animator m_Animator;


    void Start()
    {
        //Get the Animator attached to the GameObject you are intending to animate.
        m_Animator = gameObject.GetComponent<Animator>();
    }
    public void Counterspell()
    {
        GameObject Canvas = GameObject.Find("Main Canvas");
        gameObject.transform.SetParent(Canvas.transform, false);
        m_Animator.SetFloat("Counterspell", 180);

        Invoke("Stopanim", 2f);



    }
    


    void Stopanim()
    {
        m_Animator.SetFloat("Counterspell", 0);
        GameObject Bkg = GameObject.Find("Background");
        gameObject.transform.SetParent(Bkg.transform, false);
    }
   

}

