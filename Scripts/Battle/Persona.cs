using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Name { Ali, Layla, Musa, Sahara }; 
public enum Title { Warrior, Rogue, Mage };
public enum Alignment { Player, Ally, Enemy, ALL };
public enum AIBehavior { Aggressive, Strategic }

public class Stats 
{
    public Stats(int hp, int atk, int def, int spd)
    {
        HP = hp;
        ATK = atk;
        DEF = def;
        SPD = spd;
    }
    public int HP { get; set; }
    public int ATK { get; set; }
    public int DEF { get; set; }
    public int SPD { get; set; }

    public Stats Copy()
    {
        return new Stats(HP, ATK, DEF, SPD);
    }

    public static Stats operator +(Stats a, Stats b) => Add(a, b);
    public static Stats operator -(Stats a, Stats b) => Sub(a, b);

    public static Stats Add(Stats a, Stats b)
    {
        a.HP += b.HP;
        a.ATK += b.ATK;
        a.DEF += b.DEF;
        a.SPD += b.SPD;
        return a;
    }
    public static Stats Sub(Stats a, Stats b)
    {
        a.HP -= b.HP;
        a.ATK -= b.ATK;
        a.DEF -= b.DEF;
        a.SPD -= b.SPD;
        return a;
    }
}

public abstract class Persona
{
    public Persona() {}

    public Stats STATS { get; protected set; }
}

public abstract class Job : Persona
{
    public Job(Title title)
    {
        GetJob(title);
    }

    public Job(int hp, int atk, int def, int spd)
    {
        STATS = new Stats(hp, atk, def, spd);
    }

    public abstract int AttackFormula(Stats actorStats, Stats targetStats);
    public abstract int DefendFormula(Stats actorStats);

    public static Job GetJob(Title title)
    {
        return title switch
        {
            Title.Warrior => new Warrior(),
            Title.Rogue => new Rogue(),
            Title.Mage => new Mage(),
            _ => new Warrior(),
        };
    }
}

public class Warrior : Job
{
    public Warrior(): base(hp: 8, atk: 4, def: 6, spd: 1) { }

    public override int AttackFormula(Stats actorStats, Stats targetStats) 
    {
        var result = actorStats.ATK - (targetStats.DEF);
        if (result > 0) return result; else return 0; 
    }
    public override int DefendFormula(Stats actorStats) 
    { 
        return actorStats.DEF; // should only be true for one turn; change state
    }
}

public class Rogue : Job
{
    public Rogue() : base(hp: 7, atk: 3, def: 4, spd: 4) { }

    public override int AttackFormula(Stats actorStats, Stats targetStats)
    {
        var result = (actorStats.ATK - (targetStats.DEF)) * Mathf.Min((int)Mathf.Floor(actorStats.SPD / (targetStats.SPD)), 4); // when target speed is 0??
        if (result > 0) return result; else return 0;
    }

    public override int DefendFormula(Stats actorStats)
    {
        return actorStats.SPD / 2; // should only be true for one turn; change state
    }
}

public class Mage : Job
{
    public Mage() : base(hp: 6, atk: 5, def: 3, spd: 2) { }

    public override int AttackFormula(Stats actorStats, Stats targetStats)
    {
        var result = actorStats.ATK - (targetStats.DEF / 2);
        if (result > 0) return result; else return 0;
    }
    public override int DefendFormula(Stats actorStats)
    {
        return actorStats.ATK / 2; // should only be true for one turn; change state
    }
}

public class Profile : Persona
{
    public Profile(Name name)
    {
        GetProfile(name);
    }

    public Profile(int hp, int atk, int def, int spd)
    {
        STATS = new Stats(hp, atk, def, spd);
    }

    public Profile(int hp, int atk, int def, int spd, string portrait)
    {
        Portrait = portrait;
        STATS = new Stats(hp, atk, def, spd);
    }

    public string Portrait { get; set; } 

    public static Profile GetProfile(Name name)
    {
        return name switch
        {
            Name.Ali => new Profile(10, 3, 3, 4, "Sprites/Portraits/hero"),
            Name.Layla => new Profile(12, 5, 4, 2, "Sprites/Portraits/layla"),
            Name.Musa => new Profile(16, 5, 6, 1, "Sprites/Portraits/musa"),
            Name.Sahara => new Profile(11, 4, 2, 3, "Sprites/Portraits/sahara"),
            _ => new Profile(10, 4, 4, 4, "Sprites/Portraits/hero")
        };
    }

/*    public static string GetPathToPortrait(Name name)
    {
        return name switch
        {
            Name.Ali => "Sprites/Portraits/hero",
            Name.Layla => "Sprites/Portraits/layla",
            Name.Musa => "Sprites/Portraits/musa",
            Name.Sahara => "Sprites/Portraits/sahara",
            _ => "Sprites/Portraits/hero"
        };
    }*/
}
