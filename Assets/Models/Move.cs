using UnityEngine;
[System.Serializable]
public class Move
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Kind { get; set; }
    public string Scale { get; set; }
    public float Power { get; set; }
    public string Target { get; set; }
    public string? Stat { get; set; }
    public int? Delta { get; set; }
    public int? Duration { get; set; }
    public int? HpCost { get; set; }
}