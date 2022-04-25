using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using LeustoffNamespace;
public class SpellManager : NetworkBehaviour
{

    public GameObject spellCast;
    public List<Ability> cardAbilities = new List<Ability>();
    public PlayerManager PlayerManager;
    /// <summary>
    /// Method <c>Cast</c> handles resolution of spell type cards
    /// string SpellID is the gameObject name of the card being cast
    /// string TaregtID is the gameObject name of the card being targeted, if any
    /// </summary>
    public void Cast(string SpellID, string TargetID)
    {
        PlayerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        GameManager GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        spellCast = GameObject.Find(SpellID);
        CardDisplay _card = spellCast.GetComponent<CardDisplay>();
        cardAbilities = _card.Abilities;
        foreach (Ability ability in cardAbilities)
            {
            switch (ability) { 
                case Ability.Targeted:
                  break;
                case Ability.NonTargeted:
                   break;
                
                 case Ability.DestroyTarget:
                    PlayerManager.CmdGraveyard(TargetID);
                    break;
                case Ability.Draw1:
                    PlayerManager.CmdTrueDrawCard(_card.owner);
                    break;
                case Ability.Sweeper:
                    PlayerManager.CmdBoardwipe();
                    break;
                case Ability.Disrupt:
                    if (_card.CheckConvoke())
                    {
                        PlayerManager.CmdDisrupt(TargetID, int.Parse(spellCast.GetComponent<CardDisplay>().attackText.text)*2);
                    }
                    else
                    {
                        PlayerManager.CmdDisrupt(TargetID, int.Parse(spellCast.GetComponent<CardDisplay>().attackText.text));
                    }
                    break;
                case Ability.Dampen:
                    if (_card.CheckConvoke())
                    {
                        PlayerManager.CmdDampen(TargetID, int.Parse(spellCast.GetComponent<CardDisplay>().attackText.text)*2);
                    }
                    else
                    {
                        PlayerManager.CmdDampen(TargetID, int.Parse(spellCast.GetComponent<CardDisplay>().attackText.text));
                    }
                    break;
                case Ability.Bounce:
                    PlayerManager.CmdBounce(TargetID);
                    break;
                case Ability.Edict:
                    PlayerManager.CmdEdict();
                    break;
            
                case Ability.Suicide:
                    PlayerManager.CmdGraveyard(SpellID);
                    break;
                case Ability.Tax:
                    GameManager.taxTotalList[_card.owner]--;
                    GameManager.UpdateBloodthirsty();
                    break;
               
                case Ability.Hasten:
                    PlayerManager.CmdHaste();
                    break;
             
                case Ability.Trial:
                    PlayerManager.CmdTrial();
                    break;
                case Ability.Behead:
                    PlayerManager.CmdBehead();
                    break;

            }
        }





    }









}
