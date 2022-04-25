using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ManaCrystal : MonoBehaviour
{
    public Sprite ChargedCrystal;

    public Sprite DrainedCrystal;

    public void Charge()
    {
        gameObject.GetComponent<Image>().sprite = ChargedCrystal;
    }
    public void Empty()
    {
        gameObject.GetComponent<Image>().sprite = DrainedCrystal;

    }
}
