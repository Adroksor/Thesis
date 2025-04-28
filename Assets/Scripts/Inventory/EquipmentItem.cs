using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipmentItem : ItemData
{
    public int miningLevel;
    public int damage;
    
    public abstract void Equip(ItemUser user);

    public abstract void Unequip(ItemUser user);

}
