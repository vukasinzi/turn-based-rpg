using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Stats Stats { get; set; }
    public List<Buff> Buffs { get; set; } = new List<Buff>();
    public List<Move> Moveset { get; set; }

    public event Action Death;

    public virtual bool ExecuteCorrectMove(Move m, Character target, out int _skaliran)
    {

        _skaliran = 0;
        Buff CreateBuff() => new Buff
        {
            Name = m.Name ?? "",
            Stat = m.Stat ?? "",
            Delta = m.Delta ?? 0,
            Duration = m.Duration ?? 0
        };

        switch (m.Kind)
        {

            case "damage":
                {
                    this.GetComponent<Animation>()?.Play("Attack");
                    int stat = m.Scale?.ToLowerInvariant() == "magic" ? Stats.Magic : Stats.Attack;
                    target.TakeDamage(m, stat, out _skaliran);

                    if (target.Stats.Health > 0)
                    {
                        target.GetComponent<Animation>()?.Play("Hit");
                    }

                    return true;
                }
            case "damage_debuff":
                {
                    this.GetComponent<Animation>()?.Play("Attack");
                    int stat = m.Scale?.ToLowerInvariant() == "magic" ? Stats.Magic : Stats.Attack;
                    target.TakeDamage(m, stat, out _skaliran);

                    if (target.Stats.Health > 0)
                    {
                        target.GetComponent<Animation>()?.Play("Hit");
                    }

                    target.ApplyBuff(CreateBuff());

                    return true;
                }
            case "damage_heal":
                this.GetComponent<Animation>()?.Play("Attack");
                target.TakeDamage(m, Stats.Magic, out _skaliran);

                if (target.Stats.Health > 0)
                {
                    target.GetComponent<Animation>()?.Play("Hit");
                }

                Heal(m, Stats.Magic);

                return true;
            case "buff":
                return ApplyBuff(CreateBuff());
            case "debuff":
                return target.ApplyBuff(CreateBuff());
            case "buff_cost_hp":
                bool applied = ApplyBuff(CreateBuff());
                if (!applied)
                    return false;
                Stats.Health -= m.HpCost.Value;
                return true;
            case "heal":
                Heal(m, Stats.Magic);
                return true;
        }
        return false;
    }

    public virtual void Heal(Move move, int statLevel)
    {
        float calc = move.Power * (1 + (float)statLevel / 10f);
        int healAmount = Mathf.RoundToInt(calc);
        Stats.Health += healAmount;
    }

    public virtual void TakeDamage(Move move, int statLevel, out int _skaliran)
    {
        string scale = move.Scale?.ToLowerInvariant();
        _skaliran = 0;
        switch (scale)
        {
            case "magic":
                {
                    float attackBonus = Mathf.Log(1 + statLevel) / Mathf.Log(11);
                    float calc = move.Power * (1 + attackBonus);
                    int finalDamage = Mathf.Max(1, Mathf.RoundToInt(calc));
                    if (finalDamage < 0) break;
                    Stats.Health -= finalDamage;
                    _skaliran = finalDamage;
                    break;
                }
            case "physical":
                {
                    float attackBonus = Mathf.Log(1 + statLevel) / Mathf.Log(11);
                    float defenseReduction = Mathf.Log(1 + Stats.Defense) / Mathf.Log(11);
                    float calc = move.Power * (1 + attackBonus - defenseReduction);
                    int finalDamage = Mathf.Max(1, Mathf.RoundToInt(calc));
                    if (finalDamage < 0) break;
                    Stats.Health -= finalDamage;
                    _skaliran = finalDamage;
                    break;
                }
        }

        if (Stats.Health <= 0)
        {
            Stats.Health = 0;
            Death?.Invoke();
        }
    }

    public virtual bool ApplyBuff(Buff buff)
    {
        Buffs ??= new List<Buff>();

        buff.Duration += 1;
        foreach (Buff b in Buffs)
        {
            if (b.Name == buff.Name)
            {
                b.Duration += buff.Duration;
                return true;
            }
        }

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
        return true;
    }

    public virtual void BuffExpire()
    {
        if (Buffs == null || Buffs.Count == 0) return;

        List<Buff> expired = new List<Buff>();

        foreach (Buff b in Buffs)
        {
            if (b.Stat == "Health") continue;

            b.Duration--;
            if (b.Duration <= 0)
                expired.Add(b);
        }

        foreach (Buff b in expired)
        {
            Buffs.Remove(b);
            switch (b.Stat)
            {
                case "Attack": Stats.Attack -= b.Delta; break;
                case "Defense": Stats.Defense -= b.Delta; break;
                case "Magic": Stats.Magic -= b.Delta; break;
            }
        }
    }
}