using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDemon : Unit
{
    public UnitDemon(Tile t, GameObject GO)
    {
        name = "Demon";
        health = Mathf.RoundToInt(45 * levelBoost);
        damage = Mathf.RoundToInt(10 * levelBoost);
        tile = t;
        unitGO = GO;
    }

    override public void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
        tile.tileMap.gameMaster.GenericHit();
    }

    override public void Kill()
    {
        tile.tileMap.gameMaster.DemonDeath();
        base.Kill();
    }
}
