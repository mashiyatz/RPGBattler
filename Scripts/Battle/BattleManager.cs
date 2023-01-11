using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PHASE { 
    PRE_BATTLE, 
    BATTLE_START,
    TURN_START, 
    UNIT_ACTION, 
    AI_ACTION, 
    PLAYER_ACTION, 
    TARGET_SELECTION, 
    START_ANIMATION, 
    PLAYING_ANIMATION, 
    UNIT_ACTION_END, 
    TURN_END, 
    BATTLE_END 
};

public class BattleManager : MonoBehaviour
{
    public BattleMapManager battleMap;

    [SerializeField] private int turnCount;
    [SerializeField] private int enemyCount;
    [SerializeField] private int playerCount;
    

    private List<Unit> unitOrderList;
    private int orderNumber;
    private Unit activeUnit;

    // turn count event sender
    private int _turnCount = 0;
    public delegate void OnTurnCountChangeDelegate(int newVal);
    public event OnTurnCountChangeDelegate OnTurnCountChange;

    public int TurnCount
    {
        get { return _turnCount; }
        set
        {
            if (_turnCount == value) return;
            _turnCount = value;
            OnTurnCountChange?.Invoke(_turnCount);
        }
    }
    //

    // battle phase sender
    private PHASE _battlePhase;
    public delegate void OnBattlePhaseChangeDelegate(PHASE newPhase);
    public event OnBattlePhaseChangeDelegate OnBattlePhaseChange;

    public PHASE BattlePhase
    {
        get { return _battlePhase; }
        set
        {
            if (_battlePhase == value) return;
            _battlePhase = value;
            OnBattlePhaseChange?.Invoke(_battlePhase);
        }
    }
    //


    void Start()
    {
        BattlePhase = PHASE.PRE_BATTLE;
    }

    private void Update()
    {
        Debug.LogAssertion(BattlePhase.ToString());
        switch (BattlePhase)
        {
            case PHASE.PRE_BATTLE:
                if (Sortie.EnemyUnitDetails == null || Sortie.PlayerUnitDetails == null) BattlePhase = PHASE.BATTLE_END;
                else
                {
                    battleMap.SetUnitPositions();
                    unitOrderList = GetUnitOrderList(false); // fix only alive condition
                    BattlePhase = PHASE.BATTLE_START;
                }
                break;
            case PHASE.BATTLE_START:
                // activate highlight manager?
                // some kind of text or animation? 
                Debug.Log("The enemy approaches!");
                BattlePhase = PHASE.TURN_START;
                break;
            case PHASE.TURN_START:
                TurnCount += 1;
                orderNumber = 0;
                BattlePhase = PHASE.UNIT_ACTION;
                break;
            case PHASE.UNIT_ACTION:
                activeUnit = unitOrderList[orderNumber];
                activeUnit.StopDefend();
                if (activeUnit.HP == 0)
                {
                    // Debug.Log(string.Format("{0} can no longer fight!", activeUnit.name));
                    BattlePhase = PHASE.UNIT_ACTION_END;
                }
                else if (activeUnit.align != Alignment.Player) BattlePhase = PHASE.AI_ACTION; 
                // change player action condition to behavior, and then add player control behavior
                // player character behavior can be chosen via menu, a la DQ11, or allow auto battle through button
                else BattlePhase = PHASE.PLAYER_ACTION;
                break;
            case PHASE.AI_ACTION:
                activeUnit.BattleActionAI(battleMap);
                BattlePhase = PHASE.START_ANIMATION;
                break;
            case PHASE.PLAYER_ACTION:
                if (Input.GetKeyDown(KeyCode.A)) { AttackSelected(); }
                else if (Input.GetKeyDown(KeyCode.D)) { DefendSelected(); }
                break;
            case PHASE.TARGET_SELECTION:
                if (Input.GetMouseButtonDown(0))
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                    {
                        GameObject target = hit.collider.gameObject;
                        if (target.GetComponent<Unit>().HP == 0) break;
                        if (target.GetComponent<Unit>() != unitOrderList[orderNumber]) 
                            activeUnit.Attack(target.GetComponent<Unit>());
                        BattlePhase = PHASE.START_ANIMATION;
                    }
                }
                break;
            case PHASE.START_ANIMATION:
                StartCoroutine(DelayForAnimation());
                BattlePhase = PHASE.PLAYING_ANIMATION;
                break;
            case PHASE.PLAYING_ANIMATION:
                break;
            case PHASE.UNIT_ACTION_END:
                orderNumber += 1;
                if (IsBattleOver()) BattlePhase = PHASE.BATTLE_END;
                else if (orderNumber >= unitOrderList.Count) BattlePhase = PHASE.TURN_END;
                else BattlePhase = PHASE.UNIT_ACTION;
                break;
            case PHASE.TURN_END:
                unitOrderList = GetUnitOrderList(false); // fix only alive condition
                BattlePhase = PHASE.TURN_START;
                break;
            case PHASE.BATTLE_END:
                battleMap.UpdateSortie();
                SceneManager.LoadScene("CampScene");
                break;
        }
    }

    IEnumerator DelayForAnimation()
    {
        yield return new WaitForSeconds(1.0f);
        BattlePhase = PHASE.UNIT_ACTION_END;
    }

    bool IsBattleOver()
    {
        if (battleMap.GetEnemyCount() == 0 || battleMap.GetPlayerCount() == 0) { return true; } else { return false; };
    }

    public void AttackSelected()
    {
        BattlePhase = PHASE.TARGET_SELECTION;
    }

    public void DefendSelected()
    {
        activeUnit.Defend();
        BattlePhase = PHASE.START_ANIMATION;
    }

    public Unit GetActiveUnit()
    {
        if (activeUnit != null) return activeUnit;
        else return null;
    }

    public List<Unit> GetUnitOrderList(bool onlyAlive)
    {
        List<Unit> unitOrderList = new List<Unit>();
        foreach (Unit unit in battleMap.GetUnitList(Alignment.Player, onlyAlive)) { unitOrderList.Add(unit); }
        foreach (Unit unit in battleMap.GetUnitList(Alignment.Enemy, onlyAlive)) { unitOrderList.Add(unit); }
        unitOrderList.Sort((a, b) => a.SPD.CompareTo(b.SPD));
        unitOrderList.Reverse();
        return unitOrderList;
    }

    public List<Unit> GetUnitOrderList()
    {
        return unitOrderList;
    }

    public List<Unit> GetUnitList()
    {
        return battleMap.GetUnitList(onlyAlive: false);
    }
}
