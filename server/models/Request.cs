using System.Collections.Generic;

namespace server.models;

public class Request
{
    public int MonsterId { get; set; }
    public List<Buff> MonsterBuffs { get; set; } = new();
    public List<Buff> HeroBuffs { get; set; } = new();
}