using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffDB
{
    // id, name, desc, dmg, duration
    public static List<Debuff> debuffs = new List<Debuff>
    {
        new Debuff (0, "Bleed", "You've been cut badly! You're bleeding!", 4, 5),
        new Debuff (1, "Poison", "You're poisoned and are slowly losing health!", 1, 4),
        new Debuff (2, "Burn", "You're on fire! Not in a good way!", 6, 2),
    };

    public static Debuff GetDebuff(int id)
    {
        return debuffs.Find(debuff => debuff.id == id);
    }

    public static Debuff GetDebuff(string name)
    {
        return debuffs.Find(debuff => debuff.name == name);
    }
}
