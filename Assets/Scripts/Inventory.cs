using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventoryItem> inventoryItems;

    public void AddItem(InventoryItem item)
    {
        inventoryItems.Add(item);
    }
}
