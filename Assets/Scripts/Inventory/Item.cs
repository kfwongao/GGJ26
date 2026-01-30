using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName; // Name of the item
    public Sprite icon; // Item's icon to display in UI
    public bool isStackable; // Can the item be stacked?
    public int stackAmount;
    public bool isFull;
    public float dropRate; // Drop rate percentage for the item
    public GameObject itemPrefab; // Prefab to instantiate for the physical item
}
