using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerAreaAnimManager : MonoBehaviour
{
    public GameObject ZoomYard;

    public Animator m_Animator;
    void Start()
    {
        //Get the Animator attached to the GameObject you are intending to animate.
        m_Animator = gameObject.GetComponent<Animator>();
    }
    public void TookTax()
    {
        Stopanim();
        m_Animator.SetBool("TookTax", true);
        Invoke("Stopanim", .5f);


    }


    public void GainedPayout()
    {
        Stopanim();
        m_Animator.SetBool("GainedPayout", true);

        Invoke("Stopanim", .5f);



    }
    public void Stopanim()
    {
        m_Animator.SetBool("GainedPayout", false);
        m_Animator.SetBool("TookTax", false);
    }
    public bool Mine;
    public string ZoneName;
    public void OnClick()
    {
        if (Mine) return;
        GameObject Canvas = GameObject.Find("Main Canvas");
        GameObject Yardscreen = Instantiate(ZoomYard, new Vector3(0, 0, 0), Quaternion.identity);
        Yardscreen.transform.SetParent(Canvas.transform, false);
        Transform Window = Yardscreen.transform.GetChild(0).transform.GetChild(0);
        string name = gameObject.name;
        Window.gameObject.transform.parent.gameObject.GetComponent<closewindow>().TargetYard = name;

        int children = gameObject.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            gameObject.transform.GetChild(0).GetComponent<CardDisplay>().isPoppedOut=true;
            gameObject.transform.GetChild(0).transform.SetParent(Window.transform, false);
        }
        GameObject.Find("ZoomText").GetComponent<TextMeshProUGUI>().text = ZoneName;

    }



}
