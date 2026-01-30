using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System.IO;
using Newtonsoft.Json;

[Serializable]
public class ItemDataStruct
{
    public string itemName; // Name of the item
    public int stackAmount;
}


public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    public List<Item> items = new List<Item>();
    public int space = 25; // Inventory capacity
    public GameObject inventorySlotPrefab;
    public Transform itemsParent;
    public int hp_water_count = 0;
    public int mp_water_count = 0;
    public TextMeshProUGUI itemsCountText;
    public TextMeshProUGUI spaceText;
    public Button sellBtn;
    public Button useBtn;

    private string filePath;
    public List<Item> itemMap;


    public Item selectedItem;

    private void Awake()
    {
        Instance = this;
        filePath = Path.Combine(Application.persistentDataPath, "inventory.txt");
        //if (Instance == null) Instance = this;
        //else Destroy(gameObject);
    }

    public void SaveToFile()
    {
        List<ItemDataStruct> dataStruct = new List<ItemDataStruct>();
        foreach(Item it in items)
        {
            dataStruct.Add(new ItemDataStruct { itemName = it.itemName, stackAmount = it.stackAmount});
        }

        string jsonData = JsonConvert.SerializeObject(dataStruct);
        File.WriteAllText(filePath, DataSecurity.Encode(jsonData));
        Debug.Log("Data Saved to File: " + filePath);
    }

    public void LoadFromFile()
    {
        if(File.Exists(filePath))
        {
            items.Clear();
            string jsonData = File.ReadAllText(filePath);
            List<ItemDataStruct> loadedData = JsonConvert.DeserializeObject<List<ItemDataStruct>>(DataSecurity.Decode(jsonData));
            foreach(ItemDataStruct data in  loadedData)
            {
                Item it = itemMap.Find((it) => it.itemName == data.itemName);
                it.stackAmount = data.stackAmount;
                items.Add(it);
            }
            Debug.Log("Data Loaded from File: " + jsonData);
        } 
        else
        {
            Debug.LogWarning("Save file not found in " + filePath);
        }
    }

    public void DeleteSavedFile()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    private void Start()
    {
        //UpdateUI();
        LoadFromFile();
        hp_water_count = items.Where((it) => { return it.itemName == "HP_water"; }).ToList().Sum((it) => { return it.stackAmount; });
        mp_water_count = items.Where((it) => { return it.itemName == "MP_water"; }).ToList().Sum((it) => { return it.stackAmount; });
        sellBtn.onClick.AddListener(() => SellItem(1));
        useBtn.onClick.AddListener(() => UseItem());
    }


    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Tab))
        //{
        //    itemsParent.gameObject.SetActive(!itemsParent.gameObject.activeSelf);
        //    // update the ui
        //    UpdateUI();
        //}

        hp_water_count = items.Where((it) => { return it.itemName == "HP_water"; }).ToList().Sum((it) => { return it.stackAmount; });
        mp_water_count = items.Where((it) => { return it.itemName == "MP_water"; }).ToList().Sum((it) => { return it.stackAmount; });

        itemsCountText.text = $"{items.Count}";
        spaceText.text = $"/{space}";
    }

    public bool Add(Item item, bool updateUI = false)
    {
        if(item.isStackable)
        {
            List<Item> it = items.Where((it) => { return it.itemName == item.itemName && it.stackAmount < 9999; }).ToList();
            if(it.Count > 0)
            {
                it[0].stackAmount += 1;
            } 
            else
            {
                if (items.Count >= space)
                {
                    Debug.Log("Not enogh room");
                    return false; // full
                } 
                else
                {
                    item.stackAmount = 1;
                    items.Add(item);
                }
            }

            SaveToFile();
        } 
        else
        {
            if (items.Count >= space)
            {
                Debug.Log("Not enogh room");
                return false; // full
            }

            items.Add(item);
            SaveToFile();
        }


        // update the ui
        //if(updateUI)
        //{
        //    UpdateUI();
        //}
        UpdateUI();


        return true;
    }

    public int Remove(Item item)
    {
        if (item.isStackable)
        {
            List<Item> it = items.Where((it) => { return it.itemName == item.itemName && it.stackAmount > 1; }).ToList();
            if (it.Count > 0)
            {
                it[0].stackAmount -= 1;
                if(it[0].stackAmount <= 0)
                {
                    items.Remove(item);
                    // update the ui
                    UpdateUI();
                    SaveToFile();
                    return 0;
                }
                // update the ui
                UpdateUI();
                SaveToFile();
                return it[0].stackAmount;
            }
            return 0;
        }
        else
        {
            items.Remove(item);
            // update the ui
            UpdateUI();
            SaveToFile();
            return 0;
        }
    }

    public void UpdateUI()
    {
        foreach (Transform child in itemsParent)
        {
            Destroy(child.gameObject);
        }

        int itemsOccupy = items.Count;

        for(int i = itemsOccupy - 1; i >= 0; i--)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, itemsParent);
            InventorySlot slotComponent = slot.GetComponent<InventorySlot>();
            slotComponent.AddItem(items[i]);
        }

        // fill empty slot in remaining
        int remainSlots = space - itemsOccupy;
        for (int i = remainSlots - 1; i >= 0; i--)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, itemsParent);
            InventorySlot slotComponent = slot.GetComponent<InventorySlot>();
            slotComponent.ClearSlot();
        }

        hp_water_count = items.Where((it) => { return it.itemName == "HP_water"; }).ToList().Sum((it) => { return it.stackAmount; });
        mp_water_count = items.Where((it) => { return it.itemName == "MP_water"; }).ToList().Sum((it) => { return it.stackAmount; });
    }

    public void ShowItemDetail()
    {
        if(selectedItem != null)
        {
            //Update Show detail Panel UI Display
        }
    }

    public void UseItem()
    {
        if (selectedItem != null)
        {
            //Update Show detail Panel UI Display
            // use item logic & effect
            Remove(selectedItem);
        }

        UpdateUI();
        SaveToFile();
    }

    public void UseItem(string itemName)
    {
        List<Item> it = items.Where((it) => { return it.itemName == itemName; }).ToList();
        if (it.Count > 0)
        {
            //Update Show detail Panel UI Display
            // use item logic & effect
            Remove(it[0]);
        }
        SaveToFile();

        hp_water_count = items.Where((it) => { return it.itemName == "HP_water"; }).ToList().Sum((it) => { return it.stackAmount; });
        mp_water_count = items.Where((it) => { return it.itemName == "MP_water"; }).ToList().Sum((it) => { return it.stackAmount; });
    }

    public void SellItem(int howManyItemToSell = 1)
    {
        if (selectedItem != null)
        {
            //Update Show detail Panel UI Display

            for (int i = howManyItemToSell; i > 0; i--)
            {
                // sell item logic & effect
                if (Remove(selectedItem) == 0)
                {
                    break;
                }
            }
        }

        UpdateUI();
        SaveToFile();
    }
}
