using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    public Item[] items;
    public Transform spawnPoint;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            DropItem();
        }
    }

    public void DropItem()
    {
        float totalDropRate = 0;

        // Sum up all item drop rates
        foreach (var item in items)
        {
            totalDropRate += item.dropRate;
        }

        // Generate a random number between 0 and totalDropRate
        float randomValue = Random.Range(0, totalDropRate);

        // Use the random number to determine which item drops
        foreach(var item in items)
        {
            if(randomValue < item.dropRate)
            {
                // Instantiate the prefab associated with the item at the player's position
                GameObject dropItem = Instantiate(item.itemPrefab);
                dropItem.transform.position = spawnPoint.position;
                break;
            }

            randomValue -= item.dropRate;
        }
    }

}
