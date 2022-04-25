using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ArtManager : MonoBehaviour
{
    public Sprite AltWallpaper;
    public bool active;
    void Start()
    {
        active = false;
        if (active)
        {
            GameObject background = GameObject.Find("Background");
            
            background.GetComponent<Image>().sprite = AltWallpaper;
        }
    }
}
