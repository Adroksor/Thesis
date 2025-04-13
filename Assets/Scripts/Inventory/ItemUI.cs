using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public Sprite itemIcon;
    public int itemCount;
    public Image itemIconOBJ;
    public TextMeshProUGUI itemCountOBJ;

    private void Start()
    {
        UpdateItemUI();
    }

    public void UpdateItemUI()
    {
        itemIconOBJ.sprite = itemIcon;
        itemCountOBJ.text = itemCount.ToString();
    }
}
