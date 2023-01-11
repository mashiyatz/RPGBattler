using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpriteToUnit : MonoBehaviour
{
    public GameObject unitObject;
    private Unit unit;
    private Color red; 

    private void Start()
    {
        red = new Color32(0xAB, 0x40, 0x40, 0xFF);
        unit = unitObject.GetComponent<Unit>();
        GetComponent<Outline>().enabled = false;
        unit.OnHPChange += ChangeColor;
        if (unit.HP <= 0) GetComponent<Image>().color = red;
        // ChangeColor();
    }

    private void ChangeColor()
    {
        if (unit.HP <= 0) GetComponent<Image>().color = red;
        // else if (unit.HP > 0 && GetComponent<Image>().color == red) GetComponent<Image>().color = Color.white;
    }
}
