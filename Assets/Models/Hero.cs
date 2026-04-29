
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hero : Character
{
    public string Name { get; set; }
    private int xp;
    private int level = 1;

    public int Level
    {
        get { return level; }
        set
        {
            level = value;
            Stats.Attack += 1;
            Stats.Defense += 1;
            Stats.Health += 3;
            Stats.Magic += 1;
        }
    }


    //seter za levelapovanje
    public int XP
    {
        get { return xp; }
        set
        {
            xp = value;
            while (xp >= LevelUpXP)
            {
                Level += 1;

            }

        }

    }
    public int LevelUpXP => Level * 100;



}