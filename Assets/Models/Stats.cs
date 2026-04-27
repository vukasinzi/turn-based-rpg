using UnityEngine;
[System.Serializable]
public class Stats
{
    public int Health { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int Magic { get; set; }

    public Stats Clone()
    {
        Stats novi = new();
        novi.Health = Health;
        novi.Attack = Attack;
        novi.Magic = Magic;
        novi.Defense = Defense;
        return novi;
    }
}