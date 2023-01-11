using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Details")]
    public Title title;
    public new Name name;
    public Alignment align;
    public AIBehavior behavior;
    public WeaponID weaponID;
    public ArmorID armorID;
    public AccessoryID accID;

    private Job job;
    private Profile profile;
    private Stats baseStats;

    private bool isDefending = false;
    private int defBoost;

    private Stats currentStats;

    public GameObject spriteObject;

    public delegate void OnHPChangeDelegate();
    public event OnHPChangeDelegate OnHPChange;

    private int maxHP;

    public int HP
    {
        get { return currentStats.HP; }
        set
        {
            if (currentStats.HP == value) return;
            currentStats.HP = value;
            OnHPChange?.Invoke();
        }
    }

    public int ATK
    {
        get { return currentStats.ATK; }
        set
        {
            if (currentStats.ATK == value) return;
            currentStats.ATK = value;
        }
    }
    public int DEF
    {
        get { return currentStats.DEF; }
        set
        {
            if (currentStats.DEF == value) return;
            currentStats.DEF = value;
        }
    }

    public int SPD
    {
        get { return currentStats.SPD; }
        set
        {
            if (currentStats.SPD == value) return;
            currentStats.SPD = value;
        }
    }

    // Equipment

    public Armor armor; // what's the best way to select equipment? currently all setup takes place in CampUIManager
    public Weapon weapon; // include equipment in UnitDetails as well??
    public Accessory acc;

    // Unit Setup

    public static Unit CreateUnit(GameObject where, UnitDetails details)
    {
        return CreateUnit(where, details.UnitName, details.UnitTitle, details.Align, details.Behavior, details.WeaponID, details.ArmorID, details.AccessoryID);
    }

    public static Unit CreateUnit
    (
        GameObject where,
        Name unitName,
        Title unitJobTitle,
        Alignment unitAlignment,
        AIBehavior unitAIBehavior,
        WeaponID weaponID,
        ArmorID armorID,
        AccessoryID accID
    )
    {
        Unit unit = where.AddComponent<Unit>();
        unit.name = unitName;
        unit.title = unitJobTitle;
        unit.align = unitAlignment;
        unit.behavior = unitAIBehavior;

        unit.profile = Profile.GetProfile(unit.name);
        unit.job = Job.GetJob(unit.title);

        unit.baseStats = new Stats
            (
                unit.profile.STATS.HP + unit.job.STATS.HP,
                unit.profile.STATS.ATK + unit.job.STATS.ATK,
                unit.profile.STATS.DEF + unit.job.STATS.DEF,
                unit.profile.STATS.SPD + unit.job.STATS.SPD
            );

        unit.currentStats = unit.baseStats.Copy();
        unit.EquipWeapon(weaponID);
        unit.EquipArmor(armorID);
        unit.EquipAccessory(accID);

        unit.maxHP = unit.currentStats.HP;

        return unit;
    }

    // figure out a way to create a general Equip with enums
/*    public void Equip(Equipment item)
    {
        if (item is Weapon weapon) EquipWeapon(weapon);
        else if (item is Armor armor) EquipArmor(armor);
        else if (item is Accessory acc) EquipAccessory(acc);
    }*/

    public void Unequip(EquipType type)
    {
        switch (type) 
        {
            case EquipType.Weapon:
                EquipWeapon();
                break;
            case EquipType.Armor:
                EquipArmor();
                break;
            case EquipType.Accessory:
                EquipAccessory();
                break;
        }
    }

    private void EquipWeapon(WeaponID id)
    {
        EquipWeapon(Weapon.Create(id));
    }

    private void EquipWeapon(Weapon w = null) 
    {
        if (w == weapon) return;
        if (weapon != null)
            currentStats -= weapon.EquipStats;
        if (w != null)
            currentStats += w.EquipStats;
            weapon = w;
    }

    private void EquipArmor(ArmorID id)
    {
        EquipArmor(Armor.Create(id));
    }

    private void EquipArmor(Armor a = null) 
    {
        if (a == armor) return;
        if (armor != null)
            currentStats -= armor.EquipStats;
        if (a != null)
            currentStats += a.EquipStats;
            armor = a;
    }

    private void EquipAccessory(AccessoryID id)
    {
        EquipAccessory(Accessory.Create(id));
    }

    private void EquipAccessory(Accessory a = null) 
    {
        if (a == acc) return;
        if (acc != null)
            currentStats -= acc.EquipStats;
        if (a != null)
            currentStats += a.EquipStats;
            acc = a;
    }

    // Getters and Setters

    public int GetMaxHP() { return maxHP; }
    public Stats GetCurrentStats() { return currentStats; }
    public Stats GetBaseStats() { return baseStats; }
    public Profile GetUnitProfile() { return profile; }
    public Job GetUnitJob() { return job; }

    // Battle Functions

    public void Attack(Unit target)
    {
        Debug.LogFormat("{0} attacks {1}!", name, target.name);
        StartCoroutine(target.TakeDamage(job.AttackFormula(currentStats, target.currentStats)));
    }

    public void Defend()
    {
        if (!isDefending) 
        {
            Debug.LogFormat("{0} defends!", name);
            defBoost = job.DefendFormula(currentStats);
            currentStats.DEF += defBoost;
            isDefending = true;
        }
    }

    public void StopDefend()
    {
        if (isDefending)
        {
            currentStats.DEF -= defBoost;
            defBoost = 0;
            isDefending = false;
        }
    }

    IEnumerator TakeDamage(int damage)
    {
        yield return new WaitForSeconds(0.5f);
        HP -= (HP < damage) ? HP : damage; // how to add some delay between messages...
        Debug.LogFormat("{0} loses {1} HP!", name, damage);
        if (HP == 0) Debug.LogFormat("{0} can't keep up...", name);
    }

    public static Unit DetectEnemyWithLeastHP(BattleMapManager battleMap) 
    {
        var lowest = 0;
        List<Unit> targetList = battleMap.GetUnitList(Alignment.Player); // ignores enemies with low HP
        Unit enemy = targetList[0];

        foreach (Unit target in targetList)
        {
            if (lowest == 0 || target.HP < lowest)
            {
                lowest = target.HP;
                enemy = target;
            }
        }
        return enemy;
    }

    public void BattleActionAI(BattleMapManager battleMap) 
    {
        switch (behavior)
        {
            case AIBehavior.Aggressive:
                Attack(DetectEnemyWithLeastHP(battleMap));
                break;
            case AIBehavior.Strategic:
                if (GetCurrentStats().HP <= GetCurrentStats().HP / 4) Defend();
                else { Attack(DetectEnemyWithLeastHP(battleMap)); }
                break;
        } 
    }
}


