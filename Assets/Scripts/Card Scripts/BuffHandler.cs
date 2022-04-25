using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LeustoffNamespace;
public class BuffHandler : MonoBehaviour
{

    // Every method herein is an RPC subfunction

    public GameManager GameManager;
    public List<int> AttackBuffs = new List<int>();
    public List<int> DefenseBuffs = new List<int>();
    public int currentAttack;
    public int currentDefense;
    public CardDisplay CardDisplay;
    public int owner;
    public GameObject TempEffect;
    /// <summary>
    /// Method <c>Setup</c> creates an empty BuffHandler to go with each card
    /// </summary>
    public void Setup()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        CardDisplay = gameObject.GetComponent<CardDisplay>();
        owner = CardDisplay.owner;
    }
    /// <summary>
    /// Method <c>ApplyModifier</c> sums up current buffs and applies them to the card
    /// This is called at the end of almost every function in this class, so their buff actually updates the card
    /// </summary>
    public void ApplyModifier()
    {
        CardDisplay = gameObject.GetComponent<CardDisplay>();
        currentAttack = int.Parse(CardDisplay.attackText.text.ToString());
        currentAttack = CardDisplay.baseAttack;

        foreach (int attackBuff in AttackBuffs)
        {
            currentAttack += attackBuff;
        }


        if (currentAttack < 0) { currentAttack = 0; }



       
        currentDefense = CardDisplay.baseDefense;

        foreach (int defenseBuff in DefenseBuffs)
        {
            currentDefense += defenseBuff;
        }
        if (currentDefense < 0) { currentDefense = 0; }
        CardDisplay.RecieveModifier();
    }
    /// <summary>
    /// Method <c>ResetAllBuffs</c> zeroes out the  buff list, used when a card leaves the battelfield
    /// </summary>
    public void ResetAllBuffs()
    {
        for (int i = 0; i < AttackBuffs.Count;i++)
        {
            AttackBuffs[i] = 0;
        }
        for (int i = 0; i < DefenseBuffs.Count; i++)
        {
            DefenseBuffs[i] = 0;
        }
    }
    //The BuffHandler uses a system of global variables and saved indexes
    //Whenever a buff is applied, its checked if there is an index for that buff yet (if not the index will be -1)
    //It sets the index, then for attack and defense there is a list of ints
    //The index of that list is where the total for that type of buff goes
    //ApplyModifier() above goes through the list of buffs and sums it up to find the current total attack and defense
    public int BloodthirstyIndex = -1;
    /// <summary>
    /// Method <c>UpdateBloodthirsty</c> calculates an attack buff based on damage cards in opponents decks
    /// This has been found to be OP and is not currently in the game
    /// </summary>
    public void UpdateBloodthirsty()
    {
        if (BloodthirstyIndex == -1)
        {
            BloodthirstyIndex = AttackBuffs.Count;
            AttackBuffs.Add(0);
        }
        if (CardDisplay.Abilities.Contains(Ability.Bloodthirsty))
        {
            int sum = 0;
            foreach (int taxTotal in GameManager.taxTotalList)
            {
                sum += taxTotal;
            }
            sum -= GameManager.taxTotalList[owner];
            sum = sum / 3;
            AttackBuffs[BloodthirstyIndex] = sum;
            ApplyModifier();
        }

    }
    /// <summary>
    /// Method <c>RemoveBloodthirsty</c> sets the bloodthirsty buff to 0; used if the creatures loses bloodthirsty
    /// </summary>
    public void RemoveBloodthirsty()
    {
        CardDisplay.Abilities.Remove(Ability.Bloodthirsty);
        AttackBuffs[BloodthirstyIndex] = 0;
        ApplyModifier();
        CardDisplay.ResetIcons();
    }
    public int AnthemAttackIndex = -1;
    public int AnthemDefenseIndex = -1;
    /// <summary>
    /// Method <c>ApplyAnthem</c> is called on all creatures when a creature with anthem enters.  Anthem gives all of a players creatures plus one attack and defense
    /// </summary>
    public void ApplyAnthem(string SourceID)
    {
        if (gameObject.name != SourceID)
        {
            GainAnthem();
        }
    }

    /// <summary>
    /// Method <c>RecieveAnthem</c> is called on creatures that are entering after there is already an Anthem in play, to get the +1/+1
    /// </summary>
    public void RecieveAnthem()
    {
        foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardDisplay _card = card.GetComponent<CardDisplay>();
            if (_card.zone == "Board" & _card.typeText.text.ToString() == "Matter" & _card.owner == owner & _card.Abilities.Contains(Ability.Anthem))
            {
                GainAnthem();
            }
        }
    }



    /// <summary>
    /// Method <c>GainAnthem</c> is called by both ApplyAnthem and RecieveAnthem, and actually grants the +1/+1
    /// </summary>
    public void GainAnthem()
    {

        if (AnthemAttackIndex == -1)
        {
            AnthemAttackIndex = AttackBuffs.Count;
            AnthemDefenseIndex = DefenseBuffs.Count;
            AttackBuffs.Add(0);
            DefenseBuffs.Add(0);
        }
        AttackBuffs[AnthemAttackIndex]++;
        DefenseBuffs[AnthemDefenseIndex]++;
        ApplyModifier();
    }


    /// <summary>
    /// Method <c>RemoveAnthem</c> is called when a card with anthem leaves, removing the +1/+1
    /// </summary>
    public void RemoveAnthem(string SourceID)
    {
        if (gameObject.name != SourceID)
        {

            AttackBuffs[AnthemAttackIndex]--;
            DefenseBuffs[AnthemDefenseIndex]--;
            ApplyModifier();
        }
    }
    public int TeamworkIndexA = -1;
    public int TeamworkIndexD = -1;

    /// <summary>
    /// Method <c>InitiateTeamwork</c> is called when a card with the teamwork ability enters, setting it up for later
    /// Teamwork creatures get +1/+1 when new creatures enter under their owners control
    /// </summary>
    public void InitiateTeamwork()
    {
        if (TeamworkIndexA == -1)
        {
            TeamworkIndexA = AttackBuffs.Count;
            TeamworkIndexD = DefenseBuffs.Count;
            AttackBuffs.Add(0);
            DefenseBuffs.Add(0);
        }

    }

    /// <summary>
    /// Method <c>GainAnotherCreature</c> is called when a card when a creature enters and you have a creature with Teamwork
    /// </summary>
    public void GainAnotherCreature()
    {
        AttackBuffs[TeamworkIndexA]++;
        DefenseBuffs[TeamworkIndexD]++;
        ApplyModifier();
    }
    public int RhythmIndex = -1;
    /// <summary>
    /// Method <c>UpdateRhythm</c> is called when a card with Rhythm enters
    /// If the cards owner dealt damage last turn, the cards attack is doubled
    /// </summary>
    public void UpdateRhythm()
    {
        if (RhythmIndex == -1)
        {
            RhythmIndex = AttackBuffs.Count;
            AttackBuffs.Add(0);
        }
        if (GameManager.LastDamageDealt[owner] > 0)
        {
            AttackBuffs[RhythmIndex] = CardDisplay.baseAttack;
        }
        else
        {
            AttackBuffs[RhythmIndex] = 0;
        }
        ApplyModifier();
    }

    public int DisruptIndex = -1;
    public int DampenIndex = -1;
    /// <summary>
    /// Method <c>Disrupt</c> lowers the creatures attack until the player who disrupted it's next turn
    /// int x is the ammount that the attack is lowered
    /// </summary>
    public void Disrupt(int x)
    {

   
        x = -x;
        GameObject tempEffect = Instantiate(TempEffect, new Vector2(0, 0), Quaternion.identity);
        TempEffect _tempEffect = tempEffect.GetComponent<TempEffect>();
        _tempEffect.buff = x;
        _tempEffect.BuffHandler = this;
        _tempEffect.duration = GameManager.turn;
        _tempEffect.isAttackBuff = false;
        if (DisruptIndex == -1)
        {
            DisruptIndex = DefenseBuffs.Count;
            DefenseBuffs.Add(0);
        }
        DefenseBuffs[DisruptIndex] = _tempEffect.buff;
        _tempEffect.RelevantIndex = DisruptIndex;
        ApplyModifier();

    }
    /// <summary>
    /// Method <c>Dampen</c> lowers the creatures defense until the player who disrupted it's next turn
    /// int x is the ammount that the defense is lowered
    /// </summary>
    public void Dampen(int x)
    {
      
        x = -x;
        GameObject tempEffect = Instantiate(TempEffect, new Vector2(0, 0), Quaternion.identity);
        TempEffect _tempEffect = tempEffect.GetComponent<TempEffect>();
        _tempEffect.buff = x;
        _tempEffect.BuffHandler = this;
        _tempEffect.duration = GameManager.turn;
        _tempEffect.isAttackBuff = true;
        if (DampenIndex == -1)
        {
            DampenIndex = AttackBuffs.Count;
            AttackBuffs.Add(0);

        }
        AttackBuffs[DampenIndex] = _tempEffect.buff;
        _tempEffect.RelevantIndex = DampenIndex;
        ApplyModifier();

    }
    int FortifyIndex = -1;
    /// <summary>
    /// Method <c>Fortify</c> increases the  creatures defense on any turn its owner takes damage
    /// And decreases its defense on each opponent turn where its controller doesn't take damage
    /// int turn is the turn that it is
    /// </summary>
    public void Fortify(int turn)
    {
        
            if (FortifyIndex == -1)
            {
                FortifyIndex = DefenseBuffs.Count;
                DefenseBuffs.Add(0);
            }
            if (owner == turn)
            {
                return;
            }
            else if (GameManager.playerTookDamageList[owner] != 0 & owner != turn)
            {
                DefenseBuffs[FortifyIndex]++;
                if (GameManager.FortifyEnhanceList[owner] > 0)
                {
                    DefenseBuffs[FortifyIndex]++;
                }
                ApplyModifier();
            }
            else if (GameManager.playerTookDamageList[owner] == 0 & owner != turn)
            {
                if (FortifyIndex == -1)
                {
                    FortifyIndex = DefenseBuffs.Count;
                    DefenseBuffs.Add(0);
                }
                if (DefenseBuffs[FortifyIndex] > 0)
                {
                    DefenseBuffs[FortifyIndex]--;
                    ApplyModifier();
                }
            }
        
    }
}
