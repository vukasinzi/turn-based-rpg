
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
            Stats.Attack += 3;
            Stats.Defense += 3;
            Stats.Health += 5;
            Stats.Magic += 3;
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

    public List<Move> AllMoves { get; set; }
    void Awake()
    {
        Stats = new Stats
        {
            Health = 50,
            Attack = 10,
            Defense = 6,
            Magic = 6
        };

        List<Move> Moveset = new List<Move>
        {
            new Move { Id = 1, Name = "Slash", Kind = "damage", Scale = "physical", Power = 7.0f, Target = "enemy" },
            new Move { Id = 2, Name = "Shield Up", Kind = "buff", Scale = "none", Target = "self", Stat = "Defense", Delta = 10, Duration = 2 },
            new Move { Id = 3, Name = "Battle Cry", Kind = "buff", Scale = "none", Target = "self", Stat = "Attack", Delta = 10, Duration = 2 },
            new Move { Id = 4, Name = "Second Wind", Kind = "heal", Scale = "magic", Power = 3.0f, Target = "self" }
        };
        AllMoves = new List<Move>(Moveset);
        Buffs = new List<Buff>();
    }
   

}