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
            Stats.Attack += 2;
            Stats.Defense += 2;
            Stats.Health += 5;
            Stats.Magic += 1;
        }
    }
    public void CopyLevel(int newLevel,int newXP)
    {
       level = newLevel;
       xp = newXP;  
    }

    public int LevelUpXP => 50 * level * level + 50 * level;

    public int XP
    {
        get { return xp; }
        set
        {
            xp = value;
            while (xp >= LevelUpXP)
            {
                xp -= LevelUpXP; 
                Level += 1;    
            }
        }
    }
}