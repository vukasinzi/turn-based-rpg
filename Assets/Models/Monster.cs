using UnityEngine;
using System.Collections.Generic;

public class Monster : Character
{
    private int id;
    public int Id { get => id; set => id = value; }
    public string Name { get; set; }
    public int Kill_xp{get;set;}
}
