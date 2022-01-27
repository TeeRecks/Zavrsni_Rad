using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitKnight : Unit
{
    public UnitKnight(Tile t, GameObject GO)
    {
        name = "Knight";
        health = Mathf.RoundToInt(35 * levelBoost);
        damage = Mathf.RoundToInt(10 * levelBoost);
        tile = t;
        unitGO = GO;
    }

    override public void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
        tile.tileMap.gameMaster.KnightHit();
    }

    override public void Kill()
    {
        tile.tileMap.gameMaster.KnightDeath();
        base.Kill();
    }
}
