namespace server.models;

public class Monster
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Stats Stats { get; set; }
    public List<Move> Moveset { get; set; }
}