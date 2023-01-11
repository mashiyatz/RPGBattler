using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* How to do equipment and inventory...
 * - Shared inventory makes most sense, least annoying. 
 * - Do I need an Equipment GameObject to manage everything? Need a way to prevent overlapping equipment (no layering armor)
 * - Maybe make Weapon, Armor, Accessory 1, Accessory 2 of Unit, add buffs through dictionary-like lookup,
 *   which would mean saving the equipment information somewhere.
 */
public enum EquipType { Weapon, Armor, Accessory }
public enum WeaponID { Branch, IronSword, SteelBow }
public enum ArmorID { Clothes, Iron, Leather, Fur }
public enum AccessoryID { Band, GoldRing, Glasses }


public abstract class Equipment 
{
    public Equipment() {}

/*    public Equipment(string name)
    {
        Create(name);
    }*/

    public Stats EquipStats { get; protected set; }
    
    public string Name { get; protected set; }

    /*public abstract Equipment CreateFromName(string name);*/
}

public class Armor : Equipment 
{
    public Armor(ArmorID name)
    {
        Create(name);
    }

    public Armor(string name, int hp, int atk, int def, int spd) {
        Name = name;
        EquipStats = new Stats(hp, atk, def, spd);
    }

    public static Armor Create(ArmorID name)
    {
        // need a database for weapons
        return name switch
        {
            ArmorID.Iron => new Armor("Iron Armor", 5, 0, 6, -2),
            ArmorID.Leather => new Armor("Hunter's Gear", 0, 0, 4, 0),
            ArmorID.Fur => new Armor("Wizard's Robes", 0, 2, 3, 0),
            _ => new Armor("Clothes", 0, 0, 0, 0),
        };
    }
}

public class Weapon : Equipment
{
    public Weapon(WeaponID name)
    {
        Create(name);
    }

    public Weapon(string name, int hp, int atk, int def, int spd) {
        Name = name;
        EquipStats = new Stats(hp, atk, def, spd);
    }

    public static Weapon Create(WeaponID name)
    {
        return name switch
        {
            WeaponID.IronSword => new Weapon("Iron Sword", 0, 8, 0, 0),
            WeaponID.SteelBow => new Weapon("Steel Bow", 0, 5, 0, 2),
            _ => new Weapon("Branch", 0, 1, 0, 0),
        };
    }
}

public class Accessory : Equipment 
{
    public Accessory(AccessoryID name)
    {
        Create(name);
    }

    public Accessory(string name, int hp, int atk, int def, int spd) {
        Name = name;
        EquipStats = new Stats(hp, atk, def, spd);
    }

    public static Accessory Create(AccessoryID name)
    {
        return name switch
        {
            AccessoryID.Glasses => new Accessory("Glasses", 0, 0, 0, 0),
            AccessoryID.GoldRing => new Accessory("Gold Ring", 0, 0, 0, 0),
            _ => new Accessory("Band", 0, 0, 0, 0),
        };
    }
}
