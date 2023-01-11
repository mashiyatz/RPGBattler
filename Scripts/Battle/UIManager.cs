using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public BattleManager battleManager;
    public TextMeshProUGUI turnTextBox;
    public GameObject battleMenu;
    public Transform turnOrderSprites;
    public GameObject spritePrefab;
    public Transform healthBars;
    public GameObject healthBarPrefab;

    private void Start()
    {
        battleManager.OnTurnCountChange += UpdateTurnNumber;
        battleManager.OnBattlePhaseChange += UpdateBattleMenu;
        battleManager.OnBattlePhaseChange += UpdateTurnOrderSprites;

        StartCoroutine(InitializeHealthBars());
    }

    private void UpdateTurnNumber(int turnNumber)
    {
        turnTextBox.text = string.Format("Turn {0}", turnNumber);
    }

    private void UpdateBattleMenu(PHASE phase)
    {
        if (phase == PHASE.PLAYER_ACTION) { 
            foreach (Transform button in battleMenu.transform)
            {
                button.gameObject.GetComponent<Button>().interactable = true;
            }
        }
        else 
        {
            foreach (Transform button in battleMenu.transform)
            {
                button.gameObject.GetComponent<Button>().interactable = false;
            }
        }
    }

    private void UpdateTurnOrderSprites(PHASE phase)
    {
        if (phase == PHASE.BATTLE_START) 
        {
            List<Unit> unitOrderList = battleManager.GetUnitOrderList(); 
            foreach (Unit unit in unitOrderList)
            {
                GameObject spriteObject = Instantiate(spritePrefab, turnOrderSprites);  
                spriteObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(unit.GetUnitProfile().Portrait);
                spriteObject.GetComponent<SpriteToUnit>().unitObject = unit.gameObject;
                unit.spriteObject = spriteObject;
                // two objects are referring to each other...is this bad design? 
            }
        } 
        else if (phase == PHASE.TURN_START)
        {
            List<Unit> unitOrderList = battleManager.GetUnitOrderList();
            for (int i = 0; i < unitOrderList.Count; i++)
            {
                var sprite = unitOrderList[i].spriteObject.GetComponent<Image>();
                Color tmp = sprite.color;
                tmp.a = 1.0f;
                sprite.color = tmp;
                unitOrderList[i].spriteObject.transform.SetSiblingIndex(i);
            }
        }
        else if (phase == PHASE.UNIT_ACTION_END) 
        {
            var sprite = turnOrderSprites.GetChild(0).GetComponent<Image>();
            Color tmp = sprite.color;
            tmp.a = 0.5f;
            sprite.color = tmp; 
            turnOrderSprites.GetChild(0).SetAsLastSibling();
        }
        /*else if (phase == PHASE.TURN_END)
        {
            foreach (Transform child in turnOrderSprites) Destroy(child.gameObject);
        }*/
    }

    IEnumerator InitializeHealthBars()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (Unit unit in battleManager.GetUnitList())
        {
            var hpBar = Instantiate(healthBarPrefab, healthBars);
            hpBar.GetComponent<HealthBarUpdate>().SetupHPBar(unit);
        }
    }

    public void OnAttackButtonClicked()
    {
        if (battleManager.BattlePhase == PHASE.PLAYER_ACTION) 
            battleManager.AttackSelected();
    }

    public void OnDefendButtonClicked()
    {
        if (battleManager.BattlePhase == PHASE.PLAYER_ACTION)
            battleManager.DefendSelected();
    }
}
