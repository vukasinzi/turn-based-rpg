using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Monster : Character
{
    [SerializeField] private int id;
    public int Id { get => id; set => id = value; }
    public string Name { get; set; }
}
