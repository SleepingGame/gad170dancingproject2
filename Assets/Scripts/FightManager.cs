using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the outcome of a dance off between 2 dancers, determines the strength of the victory form -1 to 1
/// 
/// TODO:
///     Handle GameEvents.OnFightRequested, resolve based on stats and respond with GameEvents.FightCompleted
///         This will require a winner and defeated in the fight to be determined.
///         This may be where characters are set as selected when they are in a dance off and when they leave the dance off
///         This may also be where you use the BattleLog to output the status of fights
///     This may also be where characters suffer mojo (hp) loss when they are defeated
/// </summary>
public class FightManager : MonoBehaviour
{
    public Color drawCol = Color.gray;

    public float fightAnimTime = 2;

    private void OnEnable()
    {
        GameEvents.OnFightRequested += Fight;
    }

    private void OnDisable()
    {
        GameEvents.OnFightRequested -= Fight;
    }

    public void Fight(FightEventData data)
    {
        StartCoroutine(Attack(data.lhs, data.rhs));
    }
    // In this script there is a dance off results between dancers.
    IEnumerator Attack(Character lhs, Character rhs)
    {
        lhs.isSelected = true;
        rhs.isSelected = true;
        lhs.GetComponent<AnimationController>().Dance();
        rhs.GetComponent<AnimationController>().Dance();

        float lhsresult;
        float rhsresult;
        float outcome;

        yield return new WaitForSeconds(fightAnimTime);

         lhsresult = Random.Range(-1.0f, (lhs.luck * Random.Range(-1, lhs.luck)) + (1 / (lhs.rhythm * lhs.style + 1)) + 1);
         rhsresult = Random.Range(-1.0f, (rhs.luck * Random.Range(-1, rhs.luck)) + (1 / (rhs.rhythm * rhs.style + 1)) + 1);

        Character winner = lhs, defeated = rhs;
    
        if (lhsresult > rhsresult)
        {
            winner = lhs;
            defeated = rhs;
            outcome = lhsresult;
        }
        else
        {
            winner = rhs;
            defeated = lhs;
            outcome = rhsresult;
        }


        BattleLog.Log(new DefaultLogMessage ("the winner character is " + winner.charName.GetFullCharacterName(), winner.myTeam.teamColor));
        BattleLog.Log(new DefaultLogMessage ("the loser character is " + defeated.charName.GetFullCharacterName(), defeated.myTeam.teamColor));

        //defaulting to draw 
        //Debug.LogWarning("Attack called, needs to use character stats to determine winner with win strength from 1 to -1. This can most likely be ported from previous brief work.");


        //Debug.LogWarning("Attack called, may want to use the BattleLog to report the dancers and the outcome of their dance off.");


        var results = new FightResultData(winner, defeated, outcome);

        lhs.isSelected = false;
        rhs.isSelected = false;
        GameEvents.FightCompleted(results);


        yield return null;
    }
}
