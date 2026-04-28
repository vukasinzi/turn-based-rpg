
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hero : Character
{
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


    void Awake()
    {
        Stats = new Stats
        {
            Health = 50,
            Attack = 8,
            Defense = 4,
            Magic = 6
        };

        Moveset = new List<Move>
        {
            new Move { Id = 1, Name = "Slash", Kind = "damage", Scale = "physical", Power = 6.0f, Target = "enemy" },
            new Move { Id = 2, Name = "Shield Up", Kind = "buff", Scale = "none", Target = "self", Stat = "Defense", Delta = 6, Duration = 2 },
            new Move { Id = 3, Name = "Battle Cry", Kind = "buff", Scale = "none", Target = "self", Stat = "Attack", Delta = 6, Duration = 2 },
            new Move { Id = 4, Name = "Second Wind", Kind = "heal", Scale = "magic", Power = 4f, Target = "self" }
        };



        Buffs = new List<Buff>();
    }


}