using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CloseInventoryButton : MonoBehaviour
{
    public Button button;

    private void Start()
    {
        button.onClick.AddListener(Clicked);
    }

    public void Clicked()
    {
        InventoryManager.instance.playerInventory.CloseInventory();
    }
}
