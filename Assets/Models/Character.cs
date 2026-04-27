using System;
using System.Collections.Generic;
using UnityEngine;
public class Character : MonoBehaviour
{
      
    public Stats BaseStats{get;set;}
    public Stats Stats{get;set;}

    public List<Buff> Buffs{get;set;}

    public List<Move> Moves{get;set;}


    public virtual void TakeDamage(int amount)
    {
        Stats.Health-=amount;
           
    }
    public virtual void ApplyBuff(Buff buff)
    {
        Buffs.Add(buff);
        switch(buff.Stat)
        {
            case "Attack":
            Stats.Attack += buff.Delta;
            break;
             case "Defense":
            Stats.Defense += buff.Delta;
            break;
             case "Health":
            Stats.Health += buff.Delta;
            break;
             case "Magic":
            Stats.Magic += buff.Delta;
            break;

        }
    }
    public virtual void BuffExpire()
    {
        foreach(Buff b in Buffs)
        {
            if(b.Stat == "Health")
                continue;
            b.Duration--;
            if(b.Duration <= 0){
                 Buffs.Remove(b);
                switch(b.Stat)
                {
                    case "Attack":
                    Stats.Attack -= b.Delta;
                    break;
                    case "Defense":
                    Stats.Defense -= b.Delta;
                    break;
                    case "Magic":
                    Stats.Magic -= b.Delta;
                    break;

                }
            }
        }
    }

}