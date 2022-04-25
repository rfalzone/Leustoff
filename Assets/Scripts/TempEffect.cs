using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEffect : MonoBehaviour
{
    public int duration;
    public int buff;
    public BuffHandler BuffHandler;
    public int RelevantIndex;
    public bool isAttackBuff;
    /// <summary>
    /// Method <c>Upkeep</c> is called each turn to keep track of the turn count and remove temporary effects when they're done
    /// int turn is the new turn
    /// </summary>
    public void Upkeep(int turn)
    {
        GameManager GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (duration == turn)
        {
            if (isAttackBuff)
            {
                BuffHandler.AttackBuffs[RelevantIndex] -= buff;

            }
            else
            {
                BuffHandler.DefenseBuffs[RelevantIndex] -= buff;
            }
            BuffHandler.ApplyModifier();
            Invoke("Cleanup", .5f);
        }
    }
    public void Cleanup()
    {
        Destroy(gameObject);
    }

}
