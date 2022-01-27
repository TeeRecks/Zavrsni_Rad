using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpider : Unit
{
    public UnitSpider(Tile t, GameObject GO)
    {
        name = "Spider";
        health = Mathf.RoundToInt(20 * levelBoost);
        damage = Mathf.RoundToInt(5 * levelBoost);
        tile = t;
        unitGO = GO;
    }

    override public void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
        tile.tileMap.gameMaster.SpiderHit();
    }

    override public void Kill()
    {
        tile.tileMap.gameMaster.SpiderDeath();
        base.Kill();
    }
}
