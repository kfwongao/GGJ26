using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CharacterSkillManager : MonoBehaviour
{
    public Button BtnSkill01_E0;
    public Button BtnSkill02_E0;
    public Button BtnSkill03_E0;
    public Button BtnSkill04_E0;

    public Button BtnSkill01_E1;
    public Button BtnSkill02_E1;
    public Button BtnSkill03_E1;
    public Button BtnSkill04_E1;

    public Button BtnSkill01_E2;
    public Button BtnSkill02_E2;
    public Button BtnSkill03_E2;
    public Button BtnSkill04_E2;

    public Button BtnSkill01_E3;
    public Button BtnSkill02_E3;
    public Button BtnSkill03_E3;
    public Button BtnSkill04_E3;

    public Sprite SpriteSkill01_E0;
    public Sprite SpriteSkill02_E0;
    public Sprite SpriteSkill03_E0;
    public Sprite SpriteSkill04_E0;

    public Sprite SpriteSkill01_E1;
    public Sprite SpriteSkill02_E1;
    public Sprite SpriteSkill03_E1;
    public Sprite SpriteSkill04_E1;

    public Sprite SpriteSkill01_E2;
    public Sprite SpriteSkill02_E2;
    public Sprite SpriteSkill03_E2;
    public Sprite SpriteSkill04_E2;

    public Sprite SpriteSkill01_E3;
    public Sprite SpriteSkill02_E3;
    public Sprite SpriteSkill03_E3;
    public Sprite SpriteSkill04_E3;

    public SkillInfo currentInfo = new SkillInfo();
    public SkillType currentType;
    public Sprite currentSprite;

    public Button UpgradeBtn;

    //UI Info
    public Image selectedSkill_Image;
    public TextMeshProUGUI SkillNameText;
    public TextMeshProUGUI SkillLevelText;
    public TextMeshProUGUI SkillDescText;
    public TextMeshProUGUI DamageFormula;
    public TextMeshProUGUI MPConsumeText;
    public TextMeshProUGUI CoolingTimeText;
    public TextMeshProUGUI NextDamageFormula;
    public TextMeshProUGUI NextMPConsumeText;
    public TextMeshProUGUI NextCoolingTimeText;
    public TextMeshProUGUI RemainSkillPointsText;

    // Start is called before the first frame update
    void Start()
    {
        UpgradeBtn.enabled = false;
        UpgradeBtn.gameObject.SetActive(false);
        PlayerData.Instance.LoadData();
        //
        BtnSkill01_E0.onClick.AddListener(() => BindBtnAction(SkillType.Skill01_E0, SpriteSkill01_E0, PlayerData.Instance.skill01LV, 0, 10));
        BtnSkill02_E0.onClick.AddListener(() => BindBtnAction(SkillType.Skill02_E0, SpriteSkill02_E0, PlayerData.Instance.skill02LV, 0, 10));
        BtnSkill03_E0.onClick.AddListener(() => BindBtnAction(SkillType.Skill03_E0, SpriteSkill03_E0, PlayerData.Instance.skill03LV, 0, 10));
        BtnSkill04_E0.onClick.AddListener(() => BindBtnAction(SkillType.Skill04_E0, SpriteSkill04_E0, PlayerData.Instance.skill04LV, 0, 10));

        BtnSkill01_E1.onClick.AddListener(() => BindBtnAction(SkillType.Skill01_E1, SpriteSkill01_E1, PlayerData.Instance.skill01LV, 10, 20));
        BtnSkill02_E1.onClick.AddListener(() => BindBtnAction(SkillType.Skill02_E1, SpriteSkill02_E1, PlayerData.Instance.skill02LV, 10, 20));
        BtnSkill03_E1.onClick.AddListener(() => BindBtnAction(SkillType.Skill03_E1, SpriteSkill03_E1, PlayerData.Instance.skill03LV, 10, 20));
        BtnSkill04_E1.onClick.AddListener(() => BindBtnAction(SkillType.Skill04_E1, SpriteSkill04_E1, PlayerData.Instance.skill04LV, 10, 20));

        BtnSkill01_E2.onClick.AddListener(() => BindBtnAction(SkillType.Skill01_E2, SpriteSkill01_E2, PlayerData.Instance.skill01LV, 20, 50));
        BtnSkill02_E2.onClick.AddListener(() => BindBtnAction(SkillType.Skill02_E2, SpriteSkill02_E2, PlayerData.Instance.skill02LV, 20, 50));
        BtnSkill03_E2.onClick.AddListener(() => BindBtnAction(SkillType.Skill03_E2, SpriteSkill03_E2, PlayerData.Instance.skill03LV, 20, 50));
        BtnSkill04_E2.onClick.AddListener(() => BindBtnAction(SkillType.Skill04_E2, SpriteSkill04_E2, PlayerData.Instance.skill04LV, 20, 50));

        BtnSkill01_E3.onClick.AddListener(() => BindBtnAction(SkillType.Skill01_E3, SpriteSkill01_E3, PlayerData.Instance.skill01LV, 50, 999999));
        BtnSkill02_E3.onClick.AddListener(() => BindBtnAction(SkillType.Skill02_E3, SpriteSkill02_E3, PlayerData.Instance.skill02LV, 50, 999999));
        BtnSkill03_E3.onClick.AddListener(() => BindBtnAction(SkillType.Skill03_E3, SpriteSkill03_E3, PlayerData.Instance.skill03LV, 50, 999999));
        BtnSkill04_E3.onClick.AddListener(() => BindBtnAction(SkillType.Skill04_E3, SpriteSkill04_E3, PlayerData.Instance.skill04LV, 50, 999999));
        //
        UpgradeBtn.onClick.AddListener(UpgradeSkill);
        UpdateUI();
    }

    private void BindBtnAction(SkillType type, Sprite sprite, int lv, int minLv, int maxLv)
    {
        {
            PlayerData.Instance.LoadData();
            currentType = type;
            if (GameSettingDataSingleton.Instance.localization_index == 0)
            {
                // English
                currentInfo = PlayerData.Instance.skillDict[currentType];
            }
            else
            {
                // 简体中文
                //currentInfo = PlayerData.Instance.skillDict_Chinese[currentType];
                currentInfo = PlayerData.Instance.skillDict[currentType];
            }
            currentSprite = sprite;
            if (lv > minLv && lv <= maxLv)
            {
                UpgradeBtn.enabled = true;
                UpgradeBtn.gameObject.SetActive(true);
            }
            else
            {
                UpgradeBtn.enabled = false;
                UpgradeBtn.gameObject.SetActive(false);
            }
            UpdateUI();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        UpdateUI();
    }

    public void UpgradeSkill()
    {
        int currentRemainSkillPoint = PlayerData.Instance.remaindBonusSkillPoint;
        if(currentRemainSkillPoint > 0)
        {
            switch (currentType)
            {
                case SkillType.Skill01_E0:
                    PlayerData.Instance.skill01LV += 1;
                    if(PlayerData.Instance.skill01LV > 10)
                    {
                        UpgradeBtn.enabled = false;
                        UpgradeBtn.gameObject.SetActive(false);
                    }
                    break;
                case SkillType.Skill01_E1:
                    PlayerData.Instance.skill01LV += 1;
                    if (PlayerData.Instance.skill01LV > 20)
                    {
                        UpgradeBtn.enabled = false;
                        UpgradeBtn.gameObject.SetActive(false);
                    }
                    break;
                case SkillType.Skill01_E2:
                    PlayerData.Instance.skill01LV += 1;
                    if (PlayerData.Instance.skill01LV > 50)
                    {
                        UpgradeBtn.enabled = false;
                        UpgradeBtn.gameObject.SetActive(false);
                    }
                    break;
                case SkillType.Skill01_E3:
                    PlayerData.Instance.skill01LV += 1;
                    break;
                case SkillType.Skill02_E0:
                    PlayerData.Instance.skill02LV += 1;
                    if (PlayerData.Instance.skill02LV > 10)
                    {
                        UpgradeBtn.enabled = false;
                        UpgradeBtn.gameObject.SetActive(false);
                    }
                    break;
                case SkillType.Skill02_E1:
                    PlayerData.Instance.skill02LV += 1;
                    if (PlayerData.Instance.skill02LV > 20)
                    {
                        UpgradeBtn.enabled = false;
                        UpgradeBtn.gameObject.SetActive(false);
                    }
                    break;
                case SkillType.Skill02_E2:
                    PlayerData.Instance.skill02LV += 1;
                    if (PlayerData.Instance.skill02LV > 50)
                    {
                        UpgradeBtn.enabled = false;
                        UpgradeBtn.gameObject.SetActive(false);
                    }
                    break;
                case SkillType.Skill02_E3:
                    PlayerData.Instance.skill02LV += 1;
                    break;
                case SkillType.Skill03_E0:
                    PlayerData.Instance.skill03LV += 1;
                    if (PlayerData.Instance.skill03LV > 10)
                    {
                        UpgradeBtn.enabled = false;
                        UpgradeBtn.gameObject.SetActive(false);
                    }
                    break;
                case SkillType.Skill03_E1:
                    PlayerData.Instance.skill03LV += 1;
                    if (PlayerData.Instance.skill03LV > 20)
                    {
                        UpgradeBtn.enabled = false;
                        UpgradeBtn.gameObject.SetActive(false);
                    }
                    break;
                case SkillType.Skill03_E2:
                    PlayerData.Instance.skill03LV += 1;
                    if (PlayerData.Instance.skill03LV > 50)
                    {
                        UpgradeBtn.enabled = false;
                        UpgradeBtn.gameObject.SetActive(false);
                    }
                    break;
                case SkillType.Skill03_E3:
                    PlayerData.Instance.skill03LV += 1;
                    break;
                case SkillType.Skill04_E0:
                    PlayerData.Instance.skill04LV += 1;
                    if (PlayerData.Instance.skill04LV > 10)
                    {
                        UpgradeBtn.enabled = false;
                        UpgradeBtn.gameObject.SetActive(false);
                    }
                    break;
                case SkillType.Skill04_E1:
                    PlayerData.Instance.skill04LV += 1;
                    if (PlayerData.Instance.skill04LV > 20)
                    {
                        UpgradeBtn.enabled = false;
                        UpgradeBtn.gameObject.SetActive(false);
                    }
                    break;
                case SkillType.Skill04_E2:
                    PlayerData.Instance.skill04LV += 1;
                    if (PlayerData.Instance.skill04LV > 50)
                    {
                        UpgradeBtn.enabled = false;
                        UpgradeBtn.gameObject.SetActive(false);
                    }
                    break;
                case SkillType.Skill04_E3:
                    PlayerData.Instance.skill04LV += 1;
                    break;
            }
            PlayerData.Instance.remaindBonusSkillPoint -= 1;
            PlayerData.Instance.SaveData();
            PlayerData.Instance.LoadData();
            if(GameSettingDataSingleton.Instance.localization_index == 0)
            {
                // English
                currentInfo = PlayerData.Instance.skillDict[currentType];
            } else
            {
                // 简体中文
                //currentInfo = PlayerData.Instance.skillDict_Chinese[currentType];
                currentInfo = PlayerData.Instance.skillDict[currentType];
            }
            
        }
    }

    public void UpdateUI()
    {
        selectedSkill_Image.sprite = currentSprite;
        SkillNameText.text = currentInfo.skillName;
        SkillLevelText.text = currentInfo.skillLevel;
        SkillDescText.text = currentInfo.skillDesc;
        DamageFormula.text = currentInfo.skillDamageFormula;
        MPConsumeText.text = currentInfo.MPConsume;
        CoolingTimeText.text = currentInfo.CoolDown;
        NextDamageFormula.text = currentInfo.skillNextDamageFormula;
        NextMPConsumeText.text = currentInfo.NextMPConsume;
        NextCoolingTimeText.text = currentInfo.NextCoolDown;
        RemainSkillPointsText.text = $"{PlayerData.Instance.remaindBonusSkillPoint}";
    }
}
