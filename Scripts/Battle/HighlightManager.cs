using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class HighlightManager : MonoBehaviour
{
    public BattleManager battleManager;
    private PointerEventData pointerEventData;

    public Light turnLight;
    public Light inspectLight;
    public TextMeshProUGUI statDisplay;

    private GameObject unitObject;
    private GameObject spriteObject;

    private void Start()
    {
        battleManager.OnBattlePhaseChange += ResetSpotlight;
    }

    void Update()
    {
        pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        // inspection light
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity) && hit.transform.CompareTag("Unit"))
        {
            unitObject = hit.transform.gameObject;
            spriteObject = unitObject.GetComponent<Unit>().spriteObject;
            HoverOverObject(unitObject);
            spriteObject.GetComponent<Outline>().enabled = true;
            DisplayUnitStats(unitObject.GetComponent<Unit>());
        }
        else if (results.Count > 0 && results[0].gameObject.CompareTag("SpriteUI"))
        {
            spriteObject = results[0].gameObject;
            unitObject = spriteObject.GetComponent<SpriteToUnit>().unitObject;
            HoverOverObject(unitObject); // these three lines repeat
            spriteObject.GetComponent<Outline>().enabled = true;
            DisplayUnitStats(unitObject.GetComponent<Unit>());
        }
        else
        {
            inspectLight.enabled = false;
            statDisplay.enabled = false;
            if (spriteObject != null)
            {
                spriteObject.GetComponent<Outline>().enabled = false;
                spriteObject = null;
                unitObject = null;
            }
        }  
    }

    private void HoverOverObject(GameObject hit)
    {
        inspectLight.enabled = true;
        SetSpotlight(inspectLight, hit);
    }

    private void SetSpotlight(Light light, Unit unit)
    {
        light.transform.position = unit.transform.position + new Vector3(0, 2, 0);
    }

    private void SetSpotlight(Light light, GameObject unitGameObject)
    {
        light.transform.position = unitGameObject.transform.position + new Vector3(0, 2, 0);
    }

    private void ResetSpotlight(PHASE phase)
    {
        if (phase == PHASE.AI_ACTION || phase == PHASE.PLAYER_ACTION) 
            if (battleManager.GetActiveUnit() != null) 
                SetSpotlight(turnLight, battleManager.GetActiveUnit());
    }

    private void DisplayUnitStats(Unit unit)
    {
        statDisplay.enabled = true;
        statDisplay.text = string.Format(
            "{0} ({1})\n" +
            "HP: {2} ATK: {3} DEF: {4} SPD: {5}\n",
            unit.name,
            unit.title,
            unit.HP,
            unit.ATK,
            unit.DEF,
            unit.SPD
        );
        if (unit.weapon != null) statDisplay.text += string.Format("W: {0}", unit.weapon.Name);
        else statDisplay.text += "W: (empty)";
    }
}
