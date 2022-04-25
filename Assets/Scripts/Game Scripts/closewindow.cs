using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class closewindow : MonoBehaviour
{

    public string TargetYard;
    /// <summary>
    /// Method <c>OnClick</c> is called when the window or any cards in it are clicked
    /// Sends the cards back to where they were
    /// </summary>
    public void OnClick()
    {
        GameObject targetYard = GameObject.Find(TargetYard);
            int children = gameObject.transform.GetChild(0).childCount;
        for (int i = 0; i < children; i++)
        {
            gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<CardDisplay>().isPoppedOut = false;
            gameObject.transform.GetChild(0).transform.GetChild(0).transform.SetParent(targetYard.transform, false);
        }

        Destroy(gameObject.transform.parent.gameObject);
    }
}
