using MaskMYDrama.Cards;
using MaskMYDrama.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : Singleton<LevelData>
{
    public int currentLevel { get; set; }
    public int maxLevel { get; set; }
    public string playerName { get; set; }
    public int health { get; set; }
    public Dictionary<int, List<Enemy>> levelEnemies { get; set; }
    public List<Card> cardsPool { get; set; }
    public List<Card> rouguelikeCardsPool { get; set; }
    public Player player { get; set; }

    //
    public int totalCoin = 0;
    public int totalDiamond = 0;

    //
    public bool isGameWin = false;

    // Encore (°²¿É) tracking - persists across scenes
    [Header("Encore Status")]
    [Tooltip("Whether Encore status is active (no damage taken and killed enemy within X rounds)")]
    public bool isEncoreActive = false;

    [Tooltip("Number of rounds within which Encore must be achieved (X rounds)")]
    public int encoreRoundLimit = 3; // Default: must achieve within 3 rounds


    public Vector3 playerLocation { get; set; }
    public void LoadData()
    {
        //playerLevel = PlayerPrefs.GetInt("playerLevel", 1);
        //playerName = PlayerPrefs.GetString("playerName", "Moussy");
    }
    public void SaveData()
    {
        //PlayerPrefs.SetInt("playerLevel", playerLevel);
        //PlayerPrefs.SetString("playerName", playerName);


    }

    public void ClearData()
    {
        PlayerPrefs.DeleteKey("playerLevel");
        PlayerPrefs.DeleteKey("playerName");
    }

    public void SaveLocation()
    {
        PlayerPrefs.SetFloat("playerLocationX", playerLocation.x);
        PlayerPrefs.SetFloat("playerLocationY", playerLocation.y);
        PlayerPrefs.SetFloat("playerLocationZ", playerLocation.z);

    }


}

