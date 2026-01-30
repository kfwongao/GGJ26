using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item; // Reference to the item data (Scriptable Object)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Pickup();
        }
    }

    void Pickup()
    {
        Debug.Log($"Picked up {item.itemName}");
        if (Inventory.Instance.Add(item))
        {
            Destroy(gameObject);
        }
    }

    //private void Start()
    //{
    //    Invoke(nameof(Pickup), 1f);
    //}
}
