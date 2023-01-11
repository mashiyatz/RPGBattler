using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* TODO:
 * [ ] Create a database for loading character models using Name -> load model from Sortie when instantiating GO for SetUnitPositions() 
 */

public class BattleMapManager : MonoBehaviour
{
    public Transform battleMap;
    public Transform playerPositionGameObject;
    public Transform enemyPositionGameObject;
    public GameObject unitPrefab; // temporary, will use personalized prefab later

    public List<Transform> PlayerPositions { get; set; }
    public List<Transform> EnemyPositions { get; set; }

    public void InitializePositions()
    {
        PlayerPositions = new List<Transform>();
        EnemyPositions = new List<Transform>();
        foreach (Transform enemy in enemyPositionGameObject)
        {
            EnemyPositions.Add(enemy);
        }

        foreach (Transform player in playerPositionGameObject)
        {
            PlayerPositions.Add(player);
        }
    }

    public void SetUnitPositions()
    {
        InitializePositions();
        for (int i = 0; i < Sortie.EnemyUnitDetails.Count; i++)
        {
            // use of unitPrefab is temporary, replace with Profile.Model 
            // problem is: how to access Model when instantiating? Save to Sortie?
            // Solution: Separate database/dictionary for loading based on Sortie.UnitName / Sortie.UnitJob 
            GameObject unitGameObject = Instantiate(unitPrefab, EnemyPositions[i].position, EnemyPositions[i].rotation, battleMap.transform);
            Unit.CreateUnit(unitGameObject, Sortie.EnemyUnitDetails[i]);
            unitGameObject.GetComponent<Renderer>().material.color = Color.red;
        }

        for (int i = 0; i < Sortie.PlayerUnitDetails.Count; i++)
        {
            GameObject unitGameObject = Instantiate(unitPrefab, PlayerPositions[i].position, PlayerPositions[i].rotation, battleMap.transform);
            Unit.CreateUnit(unitGameObject, Sortie.PlayerUnitDetails[i]);
            unitGameObject.GetComponent<Renderer>().material.color = Color.blue;
        }
    }

    public void UpdateSortie()
    {
        foreach (Unit unit in GetUnitList(Alignment.Player, false)) 
        { 
            Sortie.PlayerUnitDetails.Add(UnitDetails.GetUnitDetailsFromUnit(unit)); 
        }
    }

    public List<Unit> GetUnitList(Alignment alignment = Alignment.ALL, bool onlyAlive = true)
    {
        var unitList = new List<Unit>();

        foreach (Transform child in battleMap.transform)
        {
            var unit = child.GetComponent<Unit>();
            if (onlyAlive) if (unit.HP == 0) continue;

            if (alignment == Alignment.ALL)
            {
                unitList.Add(unit);
            }
            else if (unit.align == alignment)
            {
                unitList.Add(unit);
            }
        }

        return unitList;
    }

    public int GetPlayerCount(bool onlyAlive = true)
    {
        return GetUnitList(Alignment.Player, onlyAlive).Count;
    }

    public int GetEnemyCount(bool onlyAlive = true)
    {
        return GetUnitList(Alignment.Enemy, onlyAlive).Count;
    }

}
