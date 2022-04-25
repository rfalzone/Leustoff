using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Graveyard : MonoBehaviour
{
    public GameObject ZoomYard;
    public string ZoneName;
    /// <summary>
    /// Method <c>OnClick</c> expands the graveyard for visual inspection
    /// </summary>
    public void OnClick()
    {
        GameObject Canvas = GameObject.Find("Main Canvas");
        GameObject Yardscreen = Instantiate(ZoomYard, new Vector3(0, 0, 0), Quaternion.identity);
        Yardscreen.transform.SetParent(Canvas.transform, false);
        Transform Window = Yardscreen.transform.GetChild(0).transform.GetChild(0);
        string name = gameObject.name;
        Window.gameObject.transform.parent.gameObject.GetComponent<closewindow>().TargetYard = name;
        int children = gameObject.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            gameObject.transform.GetChild(0).GetComponent<CardDisplay>().isPoppedOut = true;
            gameObject.transform.GetChild(0).transform.SetParent(Window.transform, false);
        }
        GameObject.Find("ZoomText").GetComponent<TextMeshProUGUI>().text = ZoneName;
    }
}
