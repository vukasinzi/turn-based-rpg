using System.Collections.Generic;
using System.Linq;
public class HeroDTO
{
    public int id { get; set; }
    public string name { get; set; }
    public int level { get; set; }
    public int xp { get; set; }
    public Stats stats { get; set; }
    public List<Move> allMoves { get; set; }
    public List<int> equippedMoveIds { get; set; }
    public List<Move> equippedMoves { get; set; }

    public void MapIdsToMoves() => equippedMoves = allMoves.Where(x => equippedMoveIds.Contains(x.Id)).ToList();

    public void AddXpAndLevelUp(int amount)
    {
        xp += amount;

        while (xp >= level * 100)
        {
            level += 1;
            stats.Attack += 1;
            stats.Defense += 1;
            stats.Health += 3;
            stats.Magic += 1;
        }
    }
}