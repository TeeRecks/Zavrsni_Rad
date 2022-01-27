using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameValues
{
    public static int mapSeed;
    public static int mapRings;
    public static int levelsTotal;

    public static int maxHealth;
    public static int playerDamage;

    public static int killsTotal;
    public static int movesTotal;
    public static int damageTakenTotal;
    public static int damageDealtTotal;
    public static int healedTotal;

    public static void InintializeValues()
    {
        mapRings = 6;
        maxHealth = 100;
        playerDamage = 15;
        killsTotal = 0;
        levelsTotal = 0;
        movesTotal = 0;
        damageTakenTotal = 0;
        damageDealtTotal = 0;
        healedTotal = 0;
    }

    public static void PrepareValuesForNextLevel()
    {
        mapRings++;
        levelsTotal++;
    }
}
