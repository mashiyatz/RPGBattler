using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthBarUpdate : MonoBehaviour
{
    private Unit unit;
    private TextMeshProUGUI HPTextbox;
    
    public void SetupHPBar(Unit selectedUnit)
    {
        unit = selectedUnit;
        unit.OnHPChange += UpdateHealth;
        HPTextbox = GetComponent<TextMeshProUGUI>();
        HPTextbox.rectTransform.position = Camera.main.WorldToScreenPoint(unit.transform.position) + new Vector3(75, 0, 0);
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        HPTextbox.text = string.Format("{0}\n{1}/{2}", unit.name, unit.HP, unit.GetMaxHP());
    }
}
