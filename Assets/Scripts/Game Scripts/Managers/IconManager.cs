using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LeustoffNamespace;
public class IconManager : MonoBehaviour
{
    public Sprite FlyingIcon;
    public Sprite FortifyIcon;
    public Sprite ShieldedIcon;
    public Sprite Draw1Icon;
    public Sprite Edict;
    public Sprite Disrupt;
    public Sprite Insulation;
    public Sprite Trial;
    public Sprite Counterspell;
    public Sprite Glasses;
    public Sprite DamagedShielded;
    public Sprite Bloodthirsty;
    public Sprite Anthem;
    public Sprite Opportunist;
    public Sprite Rhythm;
    public Sprite BlueChip;
    public Sprite Hasten;
    public Sprite Dash;
    public Sprite Teamwork;
    public Sprite FortifyEnhance;
    public Sprite SpawnAssistant;
    public Sprite SpawnTeammate;
    public Sprite Behead;
    public Ability ability;

    

    public void SetIcon(Ability _ability)
    {
        ability = _ability;
        switch (_ability)
        {
            case Ability.Flying:
                gameObject.GetComponent<Image>().sprite = FlyingIcon;
                
                break;
            case Ability.Fortify:
                gameObject.GetComponent<Image>().sprite = FortifyIcon;
                break;
            case Ability.Draw1:
                gameObject.GetComponent<Image>().sprite = Draw1Icon;
                break;
            case Ability.Edict:
                gameObject.GetComponent<Image>().sprite = Edict;
                break;
            case Ability.Disrupt:
                gameObject.GetComponent<Image>().sprite = Disrupt;
                break;
            case Ability.Insulated:
                gameObject.GetComponent<Image>().sprite = Insulation;
                break;
            case Ability.Shielded:
                gameObject.GetComponent<Image>().sprite = ShieldedIcon;
                break;
            case Ability.Trial:
                gameObject.GetComponent<Image>().sprite = Trial;
                break;
            case Ability.Counterspell:
                gameObject.GetComponent<Image>().sprite = Counterspell;
                break;
            case Ability.Glasses:
                gameObject.GetComponent<Image>().sprite = Glasses;
                break;
            case Ability.Bloodthirsty:
                gameObject.GetComponent<Image>().sprite = Bloodthirsty;
                break;
            case Ability.Anthem:
                gameObject.GetComponent<Image>().sprite = Anthem;
                break;
            case Ability.Opportunist:
                gameObject.GetComponent<Image>().sprite = Opportunist;
                break;
            case Ability.Rhythm:
                gameObject.GetComponent<Image>().sprite = Rhythm;
                break;
            case Ability.BlueChip:
                gameObject.GetComponent<Image>().sprite = BlueChip;
                break;
            case Ability.Hasten:
                gameObject.GetComponent<Image>().sprite = Hasten;
                break;
            case Ability.Haste:
                gameObject.GetComponent<Image>().sprite = Hasten;
                break;
            case Ability.Dash:
                gameObject.GetComponent<Image>().sprite = Dash;
                break;
            case Ability.Teamwork:
                  gameObject.GetComponent<Image>().sprite = Teamwork;
                break;
            case Ability.FortifyEnhance:
                gameObject.GetComponent<Image>().sprite = FortifyEnhance;
                break;
            case Ability.SpawnIce:
                gameObject.GetComponent<Image>().sprite = SpawnAssistant;
                break;
            case Ability.SpawnVernalPool:
                gameObject.GetComponent<Image>().sprite = SpawnTeammate;
                break;
            case Ability.Behead:
                gameObject.GetComponent<Image>().sprite = Behead;
                break;
            default:
                Destroy(gameObject);
                break;


        }
    }
    public void ClearETBs(Ability Ability)
    {
        switch(Ability)
        {
            case Ability.Draw1:
                Destroy(gameObject);
                break;
        }
    }
}
