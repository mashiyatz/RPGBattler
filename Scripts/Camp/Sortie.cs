using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public static class Sortie
{
    public static List<UnitDetails> PlayerUnitDetails { get; set; }
    public static List<UnitDetails> EnemyUnitDetails { get; set; }
}

[Serializable]
public class UnitDetails
{
    
    public UnitDetails(Name name, Title title, Alignment align, AIBehavior behavior)
    {
        UnitName = name;
        UnitTitle = title;
        Align = align;
        Behavior = behavior;
    }

    public UnitDetails(Name name, Title title, Alignment align, AIBehavior behavior, WeaponID weaponID, ArmorID armorID, AccessoryID accID)
    {
        UnitName = name;
        UnitTitle = title;
        Align = align;
        Behavior = behavior;
        WeaponID = weaponID;
        ArmorID = armorID;
        AccessoryID = accID;
    }

    public static UnitDetails GetUnitDetailsFromUnit(Unit unit)
    {
        return new UnitDetails(unit.name, unit.title, unit.align, unit.behavior, unit.weaponID, unit.armorID, unit.accID);
    }

    public Title UnitTitle { get; set; }
    public Name UnitName { get; set; }
    public Alignment Align { get; set; }
    public AIBehavior Behavior { get; set; }
    public WeaponID WeaponID { get; set; } 
    public ArmorID ArmorID { get; set; }
    public AccessoryID AccessoryID { get; set; }
}
