using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGolem : Unit
{
    public UnitGolem(Tile t, GameObject GO)
    {
        name = "Golem";
        health = Mathf.RoundToInt(60 * levelBoost);
        damage = Mathf.RoundToInt(15 * levelBoost);
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
        tile.tileMap.gameMaster.GolemDeath();
        base.Kill();
    }
}
