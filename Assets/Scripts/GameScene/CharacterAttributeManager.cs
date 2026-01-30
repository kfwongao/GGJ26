using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BasePointType
{
    Con = 1,
    Attack,
    Def,
    Magic
}


public class CharacterAttributeManager : MonoBehaviour
{
    public TextMeshProUGUI HeroNameText;
    public TextMeshProUGUI LvText;

    public Slider ConSlider;
    public TextMeshProUGUI ConText;
    public Button AddConBtn;

    public Slider AttackSlider;
    public TextMeshProUGUI AttackText;
    public Button AddAttackBtn;

    public Slider DefSlider;
    public TextMeshProUGUI DefText;
    public Button AddDefBtn;

    public Slider MagicSlider;
    public TextMeshProUGUI MagicText;
    public Button AddMagicBtn;

    public TextMeshProUGUI RemainPointsText;

    //skill
    public Image skill01IconImg;
    public Sprite[] skill01Sprites;
    public Image skill02IconImg;
    public Sprite[] skill02Sprites;
    public Image skill03IconImg;
    public Sprite[] skill03Sprites;
    public Image skill04IconImg;
    public Sprite[] skill04Sprites;

    // Start is called before the first frame update
    void Start()
    {
        PlayerData.Instance.LoadData();
        AddConBtn.onClick.AddListener(() => { AddPointTo(BasePointType.Con); });
        AddAttackBtn.onClick.AddListener(() => { AddPointTo(BasePointType.Attack); });
        AddDefBtn.onClick.AddListener(() => { AddPointTo(BasePointType.Def); });
        AddMagicBtn.onClick.AddListener(() => { AddPointTo(BasePointType.Magic); });
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        UpdateUI();
    }

    public void AddPointTo(BasePointType type)
    {
        int currentRemainBasePoint = PlayerData.Instance.remaindBonusBasePoint;
        if(currentRemainBasePoint > 0)
        {
            switch (type)
            {
                case BasePointType.Con:
                    PlayerData.Instance.conBasePoint += 1.0f;
                    break;
                case BasePointType.Attack:
                    PlayerData.Instance.atkBasePoint += 1.0f;
                    break;
                case BasePointType.Def:
                    PlayerData.Instance.defBasePoint += 1.0f;
                    break;
                case BasePointType.Magic:
                    PlayerData.Instance.magicBasePoint += 1.0f;
                    break;
            }
            PlayerData.Instance.remaindBonusBasePoint -= 1;
            PlayerData.Instance.SaveData();
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        HeroNameText.text = PlayerData.Instance.playerName;
        LvText.text = $"Lv.{PlayerData.Instance.playerLevel}";

        ConText.text = $"{(int)(PlayerData.Instance.conBasePoint + PlayerData.Instance.skillBuffPoints)}/999";
        ConSlider.value = PlayerData.Instance.conBasePoint / 999.0f;

        AttackText.text = $"{(int)(PlayerData.Instance.atkBasePoint + PlayerData.Instance.skillBuffPoints)}/999";
        AttackSlider.value = PlayerData.Instance.atkBasePoint / 999.0f;

        DefText.text = $"{(int)(PlayerData.Instance.defBasePoint + PlayerData.Instance.skillBuffPoints)}/999";
        DefSlider.value = PlayerData.Instance.defBasePoint / 999.0f;

        MagicText.text = $"{(int)(PlayerData.Instance.magicBasePoint + PlayerData.Instance.skillBuffPoints)}/999";
        MagicSlider.value = PlayerData.Instance.magicBasePoint / 999.0f;

        RemainPointsText.text = $"{PlayerData.Instance.remaindBonusBasePoint}";

        skill01IconImg.sprite = skill01Sprites[PlayerData.Instance.GetSkillIndex(1)];
        skill02IconImg.sprite = skill02Sprites[PlayerData.Instance.GetSkillIndex(2)];
        skill03IconImg.sprite = skill03Sprites[PlayerData.Instance.GetSkillIndex(3)];
        skill04IconImg.sprite = skill04Sprites[PlayerData.Instance.GetSkillIndex(4)];
    }
}
