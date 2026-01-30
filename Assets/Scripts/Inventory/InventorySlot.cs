using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Image icon; // Icon to display the item
    public Image backgroundImg; // Icon to display the item
    public TextMeshProUGUI stackAmountText;
    //public Button removeButton; // Button to remove the item
    public Button slotBtn;

    Item item; // Current item in the slot

    private void Start()
    {
        //removeButton.onClick.AddListener(() =>
        //{
        //    RemoveItem();
        //});
    }

    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
        stackAmountText.text = $"{newItem.stackAmount}";
        stackAmountText.enabled = true;
        //removeButton.interactable = true; // Enable the remove button
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        stackAmountText.text = $"0";
        stackAmountText.enabled = false;
        //removeButton.interactable = false; // Disable the remove button
    }

    public void RemoveItem()
    {
        Inventory.Instance.Remove(item); // Remove the item from inventory
    }

    public void OnSlotItemClick()
    {
        if(item != null)
        {
            Inventory.Instance.selectedItem = item;
            // Display item detail
        }
    }
}
