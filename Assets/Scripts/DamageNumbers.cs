using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumbers : MonoBehaviour
{

    public Animator m_Animator;

 
    void Start()
    {
        //Get the Animator attached to the GameObject you are intending to animate.
        m_Animator = gameObject.GetComponent<Animator>();
    }
    public void GotHit()
    {
       
            m_Animator.SetFloat("GotHit", 180);

            Invoke("Stopanim", 4f);
        
      

    }
    public void GotPoints()
    {
        m_Animator.SetFloat("GotPoints", 180);

        Invoke("Stopanim", 4f);

    }

  

    void Stopanim()
    {
        m_Animator.SetFloat("GotHit", 0);
        m_Animator.SetFloat("GotPoints", 0);
    }

}

