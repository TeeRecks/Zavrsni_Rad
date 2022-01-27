using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    public Text levels;
    public Text kills;
    public Text moves;
    public Text damageTaken;
    public Text damageDealt;
    public Text healthRecovered;

    public void ShowEndScreen()
    {
        levels.text = "Levels survived: " + GameValues.levelsTotal.ToString();
        kills.text = "Enemies defeated: " + GameValues.killsTotal.ToString();
        moves.text = "Moves made: " + GameValues.movesTotal.ToString();
        damageTaken.text = "Total damage taken: " + GameValues.damageTakenTotal.ToString();
        damageDealt.text = "Total damage dealt: " + GameValues.damageDealtTotal.ToString();
        healthRecovered.text = "Total health recovered: " + GameValues.healedTotal.ToString();
        gameObject.SetActive(true);
    }
}
