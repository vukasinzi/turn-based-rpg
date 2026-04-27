
using System.Collections.Generic;

public class Hero : Character
{
    private int xp;
    public int Level {get;set;} = 1;
    
    //seter za levelapovanje
    public int XP
    {
        get{return xp;}
        set
        {
            xp = value;
             while(xp >= LevelUpXP)
            {
                Level+= 1;
                
            }
        }

    }
    public int LevelUpXP => Level*100;
    
    public List<Move> AllMoves{get;set;}
     void Awake()
    {
        List<Move> Moveset = new List<Move>
        {
            new Move { Id = "slash", Name = "Slash", Kind = "damage", Scale = "physical", Power = 1.0f, Target = "enemy" },
            new Move { Id = "shield_up", Name = "Shield Up", Kind = "buff", Scale = "none", Target = "self", Stat = "Defense", Delta = 4, Duration = 2 },
            new Move { Id = "battle_cry", Name = "Battle Cry", Kind = "buff", Scale = "none", Target = "self", Stat = "Attack", Delta = 4, Duration = 2 },
            new Move { Id = "second_wind", Name = "Second Wind", Kind = "heal", Scale = "magic", Power = 1.0f, Target = "self" }
        };
        AllMoves = new List<Move>(Moveset);
        Buffs = new List<Buff>();
    }
    
}