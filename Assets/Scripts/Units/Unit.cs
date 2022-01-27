using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public Tile tile;
    public GameObject unitGO;

    public Tile targetTile;

    protected string name;
    protected int health;
    protected int damage;
    protected AudioClip doDamageSound;
    protected AudioClip takeDamageSound;
    protected AudioClip deathSound;

    protected float levelBoost = (100 + (GameValues.levelsTotal * 5)) / 100;

    virtual public void Kill()
    {
        tile.tileMap.DestroyGO(unitGO, this);
        tile.unit = null;
        GameValues.killsTotal++;
    }

    public int DoDamage()
    {
        return damage;
    }

    virtual public void TakeDamage(int dmg)
    {
        string debugT = health + " - " + dmg;
        health -= dmg;
        debugT += " = " + health;
        if (health <= 0)
        {
            debugT += " (kill)";
            Kill();
        }
        Debug.Log(debugT);
    }

    public string ReturnName()
    {
        return name;
    }

    //public delegate void UnitTransitionDelegate(Tile oldTile, Tile newTile);
    //public UnitTransitionDelegate Transition;

    //public void SetTile(Tile newTile)
    //{
    //    Tile oldTile = tile;

    //    if (tile != null)
    //    {
    //        tile.RemoveUnit(this);
    //    }

    //    tile = newTile;

    //    tile.SetNewUnit(this);

    //    if (Transition != null)
    //    {
    //        Transition(oldTile, newTile);
    //    }
    //}


    //public void DoTurn()
    //{
    //    //test - jedan tile desno
    //    Tile previousTile = tile;
    //    Tile targetTile = previousTile.tileMap.GetTile(previousTile.Q + 1, previousTile.R);

    //    //tile
    //    //Debug.Log(previousTile.Q + ", " + previousTile.R + " ---> " + targetTile.Q + ", " + targetTile.R);
    //    SetTile(targetTile);
    //}
}
