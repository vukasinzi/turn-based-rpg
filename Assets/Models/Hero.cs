
using System.Collections.Generic;

public class Hero : Character
{
    public int Level {get;set;} = 1;
    public int XP {get;set;} = 0;
    public int LevelUpXP => Level*XP;
    
    public List<Move> AllMoves{get;set;}
}