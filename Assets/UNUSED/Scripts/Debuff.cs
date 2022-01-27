using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuff
{
    public int id;
    public string name;
    public string description;
    public Sprite spriteImg;
    public int damage;
    public int duration;

    public Debuff(int id, string name, string description, int damage, int duration)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        //this.spriteImg = ;
        this.damage = damage;
        this.duration = duration;
    }
}
