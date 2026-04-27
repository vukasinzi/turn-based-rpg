using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Stats Stats { get; set; }
    public List<Buff> Buffs { get; set; }
    public List<Move> Moveset { get; set; }

    public event Action Death;

    public virtual void ExecuteCorrectMove(Move m, Character target)
    {
        switch (m.Kind)
        {
           case "damage":
            {
                int stat = m.Scale?.ToLowerInvariant() == "magic" ? Stats.Magic : Stats.Attack;
                target.TakeDamage(m, stat);
                break;
            }
            case "damage_debuff":
            {
                int stat = m.Scale?.ToLowerInvariant() == "magic" ? Stats.Magic : Stats.Attack;
                target.TakeDamage(m, stat);
                target.ApplyBuff(new Buff { Stat = m.Stat, Delta = m.Delta.Value, Duration = m.Duration.Value });
                break;
            }
            case "damage_heal":
                target.TakeDamage(m, Stats.Magic);
                Heal(m, Stats.Magic);
                break;
            case "buff":
                ApplyBuff(new Buff { Stat = m.Stat, Delta = m.Delta.Value, Duration = m.Duration.Value });
                break;
            case "debuff":
                target.ApplyBuff(new Buff { Stat = m.Stat, Delta = m.Delta.Value, Duration = m.Duration.Value });
                break;
            case "buff_cost_hp":
                ApplyBuff(new Buff { Stat = m.Stat, Delta = m.Delta.Value, Duration = m.Duration.Value });
                Stats.Health -= m.HpCost.Value;
                break;
            case "heal":
                Heal(m, Stats.Magic);
                break;
        }
    }

    public virtual void Heal(Move move, int statLevel)
    {
        float calc = move.Power * (1 + (float)statLevel / 10f);
        int healAmount = Mathf.RoundToInt(calc);
        Stats.Health += healAmount;
    }

    public virtual void TakeDamage(Move move, int statLevel)
    {
        string scale = move.Scale?.ToLowerInvariant();

        switch (scale)
        {
            case "magic":
            {
                float calc = move.Power * (1 + (float)statLevel / 10f);
                int finalDamage = Mathf.RoundToInt(calc);
                if (finalDamage < 0)
                    break;
                

                Stats.Health -= finalDamage;
                break;
            }
            case "physical":
            {
                float calc = move.Power * (1 + (float)statLevel / 10f - Stats.Defense / 10f);
                int finalDamage = Mathf.RoundToInt(calc);
                if (finalDamage < 0)
                    break;
                

                Stats.Health -= finalDamage;
                break;
            }
           
        }

        if (Stats.Health <= 0)
        {
            Stats.Health = 0;
            Death?.Invoke();
        }
    }

    public virtual void ApplyBuff(Buff buff)
    {
        Buffs ??= new List<Buff>();
        Buffs.Add(buff);

        switch (buff.Stat)
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
        if (Buffs == null || Buffs.Count == 0)
        {
            return;
        }

        List<Buff> expired = new List<Buff>();

        foreach (Buff b in Buffs)
        {
            if (b.Stat == "Health")
                continue;
            

            b.Duration--;
            if (b.Duration <= 0)
                expired.Add(b);
            
        }

        foreach (Buff b in expired)
        {
            Buffs.Remove(b);
            switch (b.Stat)
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
