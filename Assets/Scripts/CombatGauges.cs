using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CombatGauges : MonoBehaviour
{
    public Sprite Revenue1;
    public Sprite Revenue2;
    public Sprite Revenue3;
    public Sprite Revenue4;
    public Sprite Revenue5;
    public Sprite Revenue6;
    public Sprite Damage1;
    public Sprite Damage2;
    public Sprite Damage3;
    public Sprite Damage4;
    public Sprite Damage5;
    public Sprite Damage6;
   
    public void SetSprite(int modifier, bool isRevenue)
    {
        List<Sprite> revenuesprites = new List<Sprite>();
        revenuesprites.Add(Revenue1);
        revenuesprites.Add(Revenue2);
        revenuesprites.Add(Revenue3);
        revenuesprites.Add(Revenue4);
        revenuesprites.Add(Revenue5);
        revenuesprites.Add(Revenue6);
        List<Sprite> damagesprites = new List<Sprite>();
        damagesprites.Add(Damage1);
        damagesprites.Add(Damage2);
        damagesprites.Add(Damage3);
        damagesprites.Add(Damage4);
        damagesprites.Add(Damage5);
        damagesprites.Add(Damage6);
        if (modifier > 6)
        {
            modifier = 6;
            Debug.Log("Unexpectedly large combat modifier");
        }
        if (modifier < 0)
        {
            modifier = 0;
            Debug.Log("Negative combat modifier");

        }
        //[Damage1, Damage2, Damage3, Damage4, Damage5, Damage6]
        if (isRevenue)
        {
            gameObject.GetComponent<Image>().sprite = revenuesprites[modifier];
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = damagesprites[modifier];
        }

    }
}
