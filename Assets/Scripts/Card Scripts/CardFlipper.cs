using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardFlipper : MonoBehaviour
{
    
    public Sprite CardFront;
    public Sprite CardBack;

    public void Flip()
    {
        Sprite currentSprite = gameObject.transform.Find("Art").GetComponent<Image>().sprite;

        if(currentSprite == CardFront)
        {
            gameObject.transform.Find("Art").GetComponent<Image>().sprite = CardBack;
        }
        else
        {
            gameObject.transform.Find("Art").GetComponent<Image>().sprite = CardFront;
        }
    }
}
