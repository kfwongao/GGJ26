using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    None = -1,
    Skill01_E0 = 0,
    Skill02_E0,
    Skill03_E0,
    Skill04_E0,
    Skill01_E1,
    Skill02_E1,
    Skill03_E1,
    Skill04_E1,
    Skill01_E2,
    Skill02_E2,
    Skill03_E2,
    Skill04_E2,
    Skill01_E3,
    Skill02_E3,
    Skill03_E3,
    Skill04_E3,
}

public class SkillInfo
{
    public string skillName;
    public string skillLevel;
    public string skillDesc;
    public string skillDamageFormula;
    public string MPConsume;
    public string CoolDown;
    public string skillNextDamageFormula;
    public string NextMPConsume;
    public string NextCoolDown;
}


public class PlayerData :Singleton<PlayerData>
{
    public int playerLevel { get; set; }
    public string playerName { get; set; }
    public int playerExp { get; set; }

    public float currentHP { get; set; }
    public float maxHP { get; set; }

    public float currentMP { get; set; }
    public float maxMP { get; set; }

    public float baseAttack { get; set; }
    public float atkBasePoint { get; set; }
    public float defBasePoint { get; set; }
    public float conBasePoint { get; set; }
    public float magicBasePoint { get; set; }
    public int remaindBonusBasePoint = 0;

    public int skill01LV = 1;
    public int skill02LV = 1;
    public int skill03LV = 1;
    public int skill04LV = 1;
    public int remaindBonusSkillPoint = 0;
    public int skillBuffPoints = 0;

    //
    public int totalCoin = 0;
    public int totalDiamond = 0;

    public Dictionary<SkillType, SkillInfo> skillDict = new Dictionary<SkillType, SkillInfo>();
    public Dictionary<SkillType, SkillInfo> skillDict_Chinese = new Dictionary<SkillType, SkillInfo>();


    //
    public bool isGameWin = false;
    
    // Encore (安可) tracking - persists across scenes
    [Header("Encore Status")]
    [Tooltip("Whether Encore status is active (no damage taken and killed enemy within X rounds)")]
    public bool isEncoreActive = false;
    
    [Tooltip("Number of rounds within which Encore must be achieved (X rounds)")]
    public int encoreRoundLimit = 3; // Default: must achieve within 3 rounds


    public Vector3 playerLocation { get; set; }

    public float CalculateAttack(float defenceValue)
    {
        float calculation = GetLevelBaseAttack() - defenceValue;
        return calculation > 0 ? calculation :0;
    }

    public float CalculateReceieveDamage(float attackValue)
    {
        float calculation = attackValue - GetLevelBaseDef() * (defBasePoint + conBasePoint + magicBasePoint) * 0.01f;
        return calculation > 0 ? calculation :0;
    }

    public void LoadData()
    {
        playerLevel = PlayerPrefs.GetInt("playerLevel", 1);
        playerName = PlayerPrefs.GetString("playerName", "Moussy");
        playerExp = PlayerPrefs.GetInt("playerExp", 0);

        currentHP = PlayerPrefs.GetFloat("currentHP", 100f);
        maxHP = PlayerPrefs.GetFloat("maxHP", 100f);

        currentMP = PlayerPrefs.GetFloat("currentMP", 100f);
        maxMP = PlayerPrefs.GetFloat("maxMP", 100f);

        baseAttack = PlayerPrefs.GetFloat("baseAttack", 10f);
        atkBasePoint = PlayerPrefs.GetFloat("atkBasePoint", 5f);
        defBasePoint = PlayerPrefs.GetFloat("defBasePoint", 5f);
        conBasePoint = PlayerPrefs.GetFloat("conBasePoint", 5f);
        magicBasePoint = PlayerPrefs.GetFloat("magicBasePoint", 5f);
        remaindBonusBasePoint = PlayerPrefs.GetInt("remaindBonusBasePoint", 0);

        skill01LV = PlayerPrefs.GetInt("skill01LV", 1);
        skill02LV = PlayerPrefs.GetInt("skill02LV", 1);
        skill03LV = PlayerPrefs.GetInt("skill03LV", 1);
        skill04LV = PlayerPrefs.GetInt("skill04LV", 1);
        remaindBonusSkillPoint = PlayerPrefs.GetInt("remaindBonusSkillPoint", 0);

        totalCoin = PlayerPrefs.GetInt("totalCoin", 0);
        totalDiamond = PlayerPrefs.GetInt("totalDiamond", 0);

        float x = PlayerPrefs.GetFloat("playerLocationX", -30f);
        float y = PlayerPrefs.GetFloat("playerLocationY", 0f);
        float z = PlayerPrefs.GetFloat("playerLocationZ", 70f);

        playerLocation = new Vector3(x, y, z);

        skillDict.Clear();
        skillDict.Add(SkillType.Skill01_E0, new SkillInfo
        {
            skillName = "Base Buff",
            skillLevel = $"Lv.{skill01LV}",
            skillDesc = "Increase all base point by 5\ncon:+5, atk:+5, def:+5, magic:+5",
            skillDamageFormula = "last for 50 seconds",
            MPConsume = "5",
            CoolDown = "60''",
            skillNextDamageFormula = "last for 50 seconds",
            NextMPConsume = "5",
            NextCoolDown = "60''",
        });

        skillDict.Add(SkillType.Skill02_E0, new SkillInfo
        {
            skillName = "Base Poison",
            skillLevel = $"Lv.{skill02LV}",
            skillDesc = "(Boss Skill)Attack plus extra poison damage  Max:2",
            skillDamageFormula = $"{GetLevelBaseAttack() + 80f * (((atkBasePoint + magicBasePoint) * 1.2f + skillBuffPoints) + (skill02LV / 100f) * (GetLevelBaseAttack()))}",
            MPConsume = $"10(+{skill02LV}*({playerLevel})",
            CoolDown = "5''",
            skillNextDamageFormula = $"{GetLevelBaseAttack() + 80f * (((atkBasePoint + magicBasePoint) * 1.2f + skillBuffPoints) + ((skill02LV + 1) / 100f) * (GetLevelBaseAttack()))}",
            NextMPConsume = $"10(+{(skill02LV + 1)}*({playerLevel})",
            NextCoolDown = "5''",
        });

        skillDict.Add(SkillType.Skill03_E0, new SkillInfo
        {
            skillName = "Base Wind",
            skillLevel = $"Lv.{skill03LV}",
            skillDesc = "Wind Element Magic Attack (AOE)  Max:4",
            skillDamageFormula = $"{(int)(80 + (playerLevel + skill03LV) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 0.8f)}~{(int)(80 + (playerLevel + skill03LV) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 1.5f)}",
            MPConsume = $"{20f + (skill03LV - 1) * 2f}",
            CoolDown = "3''",
            skillNextDamageFormula = $"{(int)(80 + (playerLevel + (skill03LV+1)) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 0.8f)}~{(int)(80 + (playerLevel + (skill03LV+1)) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 1.5f)}",
            NextMPConsume = $"{20f + ((skill03LV+1) - 1) * 2f}",
            NextCoolDown = "3''",
        });

        skillDict.Add(SkillType.Skill04_E0, new SkillInfo
        {
            skillName = "Base Ball",
            skillLevel = $"Lv.{skill04LV}",
            skillDesc = "Single Long Distance Ball Attack with high atk and magic damage value",
            skillDamageFormula = $"{(int)(60f + (playerLevel + skill04LV) * 0.5f * ((atkBasePoint + skillBuffPoints) * 0.5f + (magicBasePoint + skillBuffPoints) * 0.5f))}",
            MPConsume = $"{10f + (skill04LV - 1) * 1f}",
            CoolDown = "1''",
            skillNextDamageFormula = $"{(int)(60f + (playerLevel + (skill04LV + 1)) * 0.5f * ((atkBasePoint + skillBuffPoints) * 0.5f + (magicBasePoint + skillBuffPoints) * 0.5f))}",
            NextMPConsume = $"{10f + ((skill04LV + 1) - 1) * 1f}",
            NextCoolDown = "1''",
        });


        //
        skillDict.Add(SkillType.Skill01_E1, new SkillInfo
        {
            skillName = "Medium Buff",
            skillLevel = $"Lv.{skill01LV}",
            skillDesc = "Increase all base point by 10\ncon:+10, atk:+10, def:+10, magic:+10",
            skillDamageFormula = "last for 50 seconds",
            MPConsume = "10",
            CoolDown = "60''",
            skillNextDamageFormula = "last for 50 seconds",
            NextMPConsume = "10",
            NextCoolDown = "60''",
        });

        skillDict.Add(SkillType.Skill02_E1, new SkillInfo
        {
            skillName = "Medium Poison",
            skillLevel = $"Lv.{skill02LV}",
            skillDesc = "(Boss Skill)Attack plus extra poison damage  Max:6",
            skillDamageFormula = $"{GetLevelBaseAttack() + 80f * (((atkBasePoint + magicBasePoint) * 1.2f + skillBuffPoints) + (skill02LV / 100f) * (GetLevelBaseAttack()))}",
            MPConsume = $"10(+{skill02LV}*({playerLevel})",
            CoolDown = "5''",
            skillNextDamageFormula = $"{GetLevelBaseAttack() + 80f * (((atkBasePoint + magicBasePoint) * 1.2f + skillBuffPoints) + ((skill02LV + 1) / 100f) * (GetLevelBaseAttack()))}",
            NextMPConsume = $"10(+{(skill02LV + 1)}*({playerLevel})",
            NextCoolDown = "5''",
        });

        skillDict.Add(SkillType.Skill03_E1, new SkillInfo
        {
            skillName = "Medium Wind",
            skillLevel = $"Lv.{skill03LV}",
            skillDesc = "Wind Element Magic Attack (AOE)  Max:8",
            skillDamageFormula = $"{(int)(80 + (playerLevel + skill03LV) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 0.8f)}~{(int)(80 + (playerLevel + skill03LV) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 1.5f)}",
            MPConsume = $"{20f + (skill03LV - 1) * 2f}",
            CoolDown = "3''",
            skillNextDamageFormula = $"{(int)(80 + (playerLevel + (skill03LV + 1)) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 0.8f)}~{(int)(80 + (playerLevel + (skill03LV + 1)) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 1.5f)}",
            NextMPConsume = $"{20f + ((skill03LV + 1) - 1) * 2f}",
            NextCoolDown = "3''",
        });

        skillDict.Add(SkillType.Skill04_E1, new SkillInfo
        {
            skillName = "Medium Ball",
            skillLevel = $"Lv.{skill04LV}",
            skillDesc = "Single Long Distance Ball Attack with high atk and magic damage value",
            skillDamageFormula = $"{(int)(60f + (playerLevel + skill04LV) * 0.5f * ((atkBasePoint + skillBuffPoints) * 0.5f + (magicBasePoint + skillBuffPoints) * 0.5f))}",
            MPConsume = $"{10f + (skill04LV - 1) * 1f}",
            CoolDown = "1''",
            skillNextDamageFormula = $"{(int)(60f + (playerLevel + (skill04LV + 1)) * 0.5f * ((atkBasePoint + skillBuffPoints) * 0.5f + (magicBasePoint + skillBuffPoints) * 0.5f))}",
            NextMPConsume = $"{10f + ((skill04LV + 1) - 1) * 1f}",
            NextCoolDown = "1''",
        });

        //
        skillDict.Add(SkillType.Skill01_E2, new SkillInfo
        {
            skillName = "Strong Buff",
            skillLevel = $"Lv.{skill01LV}",
            skillDesc = "Increase all base point by 20 + (skill LV / 5)",
            skillDamageFormula = "last for 50 seconds",
            MPConsume = "20",
            CoolDown = "60''",
            skillNextDamageFormula = "last for 50 seconds",
            NextMPConsume = "20",
            NextCoolDown = "60''",
        });

        skillDict.Add(SkillType.Skill02_E2, new SkillInfo
        {
            skillName = "Strong Poison",
            skillLevel = $"Lv.{skill02LV}",
            skillDesc = "(Boss Skill)Attack plus extra poison damage  Max:15",
            skillDamageFormula = $"{GetLevelBaseAttack() + 80f * (((atkBasePoint + magicBasePoint) * 1.2f + skillBuffPoints) + (skill02LV / 100f) * (GetLevelBaseAttack()))}",
            MPConsume = $"10(+{skill02LV}*({playerLevel})",
            CoolDown = "5''",
            skillNextDamageFormula = $"{GetLevelBaseAttack() + 80f * (((atkBasePoint + magicBasePoint) * 1.2f + skillBuffPoints) + ((skill02LV + 1) / 100f) * (GetLevelBaseAttack()))}",
            NextMPConsume = $"10(+{(skill02LV + 1)}*({playerLevel})",
            NextCoolDown = "5''",
        });

        skillDict.Add(SkillType.Skill03_E2, new SkillInfo
        {
            skillName = "Strong Wind",
            skillLevel = $"Lv.{skill03LV}",
            skillDesc = "Wind Element Magic Attack (AOE)  Max:20",
            skillDamageFormula = $"{(int)(80 + (playerLevel + skill03LV) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 0.8f)}~{(int)(80 + (playerLevel + skill03LV) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 1.5f)}",
            MPConsume = $"{20f + (skill03LV - 1) * 2f}",
            CoolDown = "3''",
            skillNextDamageFormula = $"{(int)(80 + (playerLevel + (skill03LV + 1)) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 0.8f)}~{(int)(80 + (playerLevel + (skill03LV + 1)) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 1.5f)}",
            NextMPConsume = $"{20f + ((skill03LV + 1) - 1) * 2f}",
            NextCoolDown = "3''",
        });

        skillDict.Add(SkillType.Skill04_E2, new SkillInfo
        {
            skillName = "Strong Ball",
            skillLevel = $"Lv.{skill04LV}",
            skillDesc = "Single Long Distance Ball Attack with high atk and magic damage value",
            skillDamageFormula = $"{(int)(60f + (playerLevel + skill04LV) * 0.5f * ((atkBasePoint + skillBuffPoints) * 0.5f + (magicBasePoint + skillBuffPoints) * 0.5f))}",
            MPConsume = $"{10f + (skill04LV - 1) * 1f}",
            CoolDown = "1''",
            skillNextDamageFormula = $"{(int)(60f + (playerLevel + (skill04LV + 1)) * 0.5f * ((atkBasePoint + skillBuffPoints) * 0.5f + (magicBasePoint + skillBuffPoints) * 0.5f))}",
            NextMPConsume = $"{10f + ((skill04LV + 1) - 1) * 1f}",
            NextCoolDown = "1''",
        });

        //
        skillDict.Add(SkillType.Skill01_E3, new SkillInfo
        {
            skillName = "Extreme Buff",
            skillLevel = $"Lv.{skill01LV}",
            skillDesc = $"Increase all base point by 100 + (skill LV / 4)",
            skillDamageFormula = "last for 50 seconds",
            MPConsume = "100",
            CoolDown = "60''",
            skillNextDamageFormula = "last for 50 seconds",
            NextMPConsume = "100",
            NextCoolDown = "60''",
        });

        skillDict.Add(SkillType.Skill02_E3, new SkillInfo
        {
            skillName = "Extreme Poison",
            skillLevel = $"Lv.{skill02LV}",
            skillDesc = "(Boss Skill)Attack plus extra poison damage  Max:30",
            skillDamageFormula = $"{GetLevelBaseAttack() + 80f * (((atkBasePoint + magicBasePoint) * 1.2f + skillBuffPoints) + (skill02LV / 100f) * (GetLevelBaseAttack()))}",
            MPConsume = $"10(+{skill02LV}*({playerLevel})",
            CoolDown = "5''",
            skillNextDamageFormula = $"{GetLevelBaseAttack() + 80f * (((atkBasePoint + magicBasePoint) * 1.2f + skillBuffPoints) + ((skill02LV + 1) / 100f) * (GetLevelBaseAttack()))}",
            NextMPConsume = $"10(+{(skill02LV + 1)}*({playerLevel})",
            NextCoolDown = "5''",
        });

        skillDict.Add(SkillType.Skill03_E3, new SkillInfo
        {
            skillName = "Extreme Wind",
            skillLevel = $"Lv.{skill03LV}",
            skillDesc = "Wind Element Magic Attack (AOE)  Max:40",
            skillDamageFormula = $"{(int)(80 + (playerLevel + skill03LV) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 0.8f)}~{(int)(80 + (playerLevel + skill03LV) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 1.5f)}",
            MPConsume = $"{20f + (skill03LV - 1) * 2f}",
            CoolDown = "3''",
            skillNextDamageFormula = $"{(int)(80 + (playerLevel + (skill03LV + 1)) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 0.8f)}~{(int)(80 + (playerLevel + (skill03LV + 1)) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 1.5f)}",
            NextMPConsume = $"{20f + ((skill03LV + 1) - 1) * 2f}",
            NextCoolDown = "3''",
        });

        skillDict.Add(SkillType.Skill04_E3, new SkillInfo
        {
            skillName = "Extreme Ball",
            skillLevel = $"Lv.{skill04LV}",
            skillDesc = "Single Long Distance Ball Attack with high atk and magic damage value",
            skillDamageFormula = $"{(int)(60f + (playerLevel + skill04LV) * 0.5f * ((atkBasePoint + skillBuffPoints) * 0.5f + (magicBasePoint + skillBuffPoints) * 0.5f))}",
            MPConsume = $"{10f + (skill04LV - 1) * 1f}",
            CoolDown = "1''",
            skillNextDamageFormula = $"{(int)(60f + (playerLevel + (skill04LV + 1)) * 0.5f * ((atkBasePoint+skillBuffPoints) * 0.5f + (magicBasePoint + skillBuffPoints) * 0.5f))}",
            NextMPConsume = $"{10f + ((skill04LV + 1) - 1) * 1f}",
            NextCoolDown = "1''",
        });



        //�������İ汾
        skillDict_Chinese.Clear();
        skillDict_Chinese.Add(SkillType.Skill01_E0, new SkillInfo
        {
            skillName = "��������",
            skillLevel = $"Lv.{skill01LV}",
            skillDesc = $@"����������5��\ncon:+5, atk:+5, def:+5, magic:+5",
            skillDamageFormula = "last for 50 s",
            MPConsume = "5",
            CoolDown = "60s",
            skillNextDamageFormula = "last for 50 s",
            NextMPConsume = "5",
            NextCoolDown = "60s",
        });

        skillDict_Chinese.Add(SkillType.Skill02_E0, new SkillInfo
        {
            skillName = "��������",
            skillLevel = $"Lv.{skill02LV}",
            skillDesc = $@"(Boss��)��+���� Max:2",
            skillDamageFormula = $"{GetLevelBaseAttack() + 80f * (((atkBasePoint + magicBasePoint) * 1.2f + skillBuffPoints) + (skill02LV / 100f) * (GetLevelBaseAttack()))}",
            MPConsume = $"10(+{skill02LV}*({playerLevel})",
            CoolDown = "5s",
            skillNextDamageFormula = $"{GetLevelBaseAttack() + 80f * (((atkBasePoint + magicBasePoint) * 1.2f + skillBuffPoints) + ((skill02LV + 1) / 100f) * (GetLevelBaseAttack()))}",
            NextMPConsume = $"10(+{(skill02LV + 1)}*({playerLevel})",
            NextCoolDown = "5s",
        });

        skillDict_Chinese.Add(SkillType.Skill03_E0, new SkillInfo
        {
            skillName = "�����籩",
            skillLevel = $"Lv.{skill03LV}",
            skillDesc = $@"������ħ���˺�(��Χ)  Max:4",
            skillDamageFormula = $"{(int)(80 + (playerLevel + skill03LV) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 0.8f)}~{(int)(80 + (playerLevel + skill03LV) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 1.5f)}",
            MPConsume = $"{20f + (skill03LV - 1) * 2f}",
            CoolDown = "3s",
            skillNextDamageFormula = $"{(int)(80 + (playerLevel + (skill03LV + 1)) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 0.8f)}~{(int)(80 + (playerLevel + (skill03LV + 1)) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 1.5f)}",
            NextMPConsume = $"{20f + ((skill03LV + 1) - 1) * 2f}",
            NextCoolDown = "3s",
        });

        skillDict_Chinese.Add(SkillType.Skill04_E0, new SkillInfo
        {
            skillName = "��������",
            skillLevel = $"Lv.{skill04LV}",
            skillDesc = $@"����, ��+ħ��",
            skillDamageFormula = $"{(int)(60f + (playerLevel + skill04LV) * 0.5f * ((atkBasePoint + skillBuffPoints) * 0.5f + (magicBasePoint + skillBuffPoints) * 0.5f))}",
            MPConsume = $"{10f + (skill04LV - 1) * 1f}",
            CoolDown = "1s",
            skillNextDamageFormula = $"{(int)(60f + (playerLevel + (skill04LV + 1)) * 0.5f * ((atkBasePoint + skillBuffPoints) * 0.5f + (magicBasePoint + skillBuffPoints) * 0.5f))}",
            NextMPConsume = $"{10f + ((skill04LV + 1) - 1) * 1f}",
            NextCoolDown = "1s",
        });


        //
        skillDict_Chinese.Add(SkillType.Skill01_E1, new SkillInfo
        {
            skillName = "�е�����",
            skillLevel = $"Lv.{skill01LV}",
            skillDesc = $@"����������10��\ncon:+10, atk:+10, def:+10, magic:+10",
            skillDamageFormula = "last for 50 s",
            MPConsume = "10",
            CoolDown = "60s",
            skillNextDamageFormula = "last for 50 s",
            NextMPConsume = "10",
            NextCoolDown = "60s",
        });

        skillDict_Chinese.Add(SkillType.Skill02_E1, new SkillInfo
        {
            skillName = "�еȶ���",
            skillLevel = $"Lv.{skill02LV}",
            skillDesc = $@"(Boss��)��+���� Max:6",
            skillDamageFormula = $"{GetLevelBaseAttack() + 80f * (((atkBasePoint + magicBasePoint) * 1.2f + skillBuffPoints) + (skill02LV / 100f) * (GetLevelBaseAttack()))}",
            MPConsume = $"10(+{skill02LV}*({playerLevel})",
            CoolDown = "5s",
            skillNextDamageFormula = $"{GetLevelBaseAttack() + 80f * (((atkBasePoint + magicBasePoint) * 1.2f + skillBuffPoints) + ((skill02LV + 1) / 100f) * (GetLevelBaseAttack()))}",
            NextMPConsume = $"10(+{(skill02LV + 1)}*({playerLevel})",
            NextCoolDown = "5s",
        });

        skillDict_Chinese.Add(SkillType.Skill03_E1, new SkillInfo
        {
            skillName = "�еȷ籩",
            skillLevel = $"Lv.{skill03LV}",
            skillDesc = $@"������ħ���˺�(��Χ)  Max:8",
            skillDamageFormula = $"{(int)(80 + (playerLevel + skill03LV) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 0.8f)}~{(int)(80 + (playerLevel + skill03LV) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 1.5f)}",
            MPConsume = $"{20f + (skill03LV - 1) * 2f}",
            CoolDown = "3s",
            skillNextDamageFormula = $"{(int)(80 + (playerLevel + (skill03LV + 1)) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 0.8f)}~{(int)(80 + (playerLevel + (skill03LV + 1)) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 1.5f)}",
            NextMPConsume = $"{20f + ((skill03LV + 1) - 1) * 2f}",
            NextCoolDown = "3s",
        });

        skillDict_Chinese.Add(SkillType.Skill04_E1, new SkillInfo
        {
            skillName = "�еȲ���",
            skillLevel = $"Lv.{skill04LV}",
            skillDesc = $@"����, ��+ħ��",
            skillDamageFormula = $"{(int)(60f + (playerLevel + skill04LV) * 0.5f * ((atkBasePoint + skillBuffPoints) * 0.5f + (magicBasePoint + skillBuffPoints) * 0.5f))}",
            MPConsume = $"{10f + (skill04LV - 1) * 1f}",
            CoolDown = "1s",
            skillNextDamageFormula = $"{(int)(60f + (playerLevel + (skill04LV + 1)) * 0.5f * ((atkBasePoint + skillBuffPoints) * 0.5f + (magicBasePoint + skillBuffPoints) * 0.5f))}",
            NextMPConsume = $"{10f + ((skill04LV + 1) - 1) * 1f}",
            NextCoolDown = "1s",
        });

        //
        skillDict_Chinese.Add(SkillType.Skill01_E2, new SkillInfo
        {
            skillName = "ǿЧ����",
            skillLevel = $"Lv.{skill01LV}",
            skillDesc = $@"����������20�� + (�ȼ� / 5)",
            skillDamageFormula = "last for 50 s",
            MPConsume = "20",
            CoolDown = "60s",
            skillNextDamageFormula = "last for 50 s",
            NextMPConsume = "20",
            NextCoolDown = "60s",
        });

        skillDict_Chinese.Add(SkillType.Skill02_E2, new SkillInfo
        {
            skillName = "ǿЧ����",
            skillLevel = $"Lv.{skill02LV}",
            skillDesc = $@"(Boss��)��+���� Max:15",
            skillDamageFormula = $"{GetLevelBaseAttack() + 80f * (((atkBasePoint + magicBasePoint) * 1.2f + skillBuffPoints) + (skill02LV / 100f) * (GetLevelBaseAttack()))}",
            MPConsume = $"10(+{skill02LV}*({playerLevel})",
            CoolDown = "5s",
            skillNextDamageFormula = $"{GetLevelBaseAttack() + 80f * (((atkBasePoint + magicBasePoint) * 1.2f + skillBuffPoints) + ((skill02LV + 1) / 100f) * (GetLevelBaseAttack()))}",
            NextMPConsume = $"10(+{(skill02LV + 1)}*({playerLevel})",
            NextCoolDown = "5s",
        });

        skillDict_Chinese.Add(SkillType.Skill03_E2, new SkillInfo
        {
            skillName = "ǿЧ�籩",
            skillLevel = $"Lv.{skill03LV}",
            skillDesc = $@"������ħ���˺�(��Χ)  Max:20",
            skillDamageFormula = $"{(int)(80 + (playerLevel + skill03LV) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 0.8f)}~{(int)(80 + (playerLevel + skill03LV) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 1.5f)}",
            MPConsume = $"{20f + (skill03LV - 1) * 2f}",
            CoolDown = "3s",
            skillNextDamageFormula = $"{(int)(80 + (playerLevel + (skill03LV + 1)) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 0.8f)}~{(int)(80 + (playerLevel + (skill03LV + 1)) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 1.5f)}",
            NextMPConsume = $"{20f + ((skill03LV + 1) - 1) * 2f}",
            NextCoolDown = "3s",
        });

        skillDict_Chinese.Add(SkillType.Skill04_E2, new SkillInfo
        {
            skillName = "ǿЧ����",
            skillLevel = $"Lv.{skill04LV}",
            skillDesc = $@"����, ��+ħ��",
            skillDamageFormula = $"{(int)(60f + (playerLevel + skill04LV) * 0.5f * ((atkBasePoint + skillBuffPoints) * 0.5f + (magicBasePoint + skillBuffPoints) * 0.5f))}",
            MPConsume = $"{10f + (skill04LV - 1) * 1f}",
            CoolDown = "1s",
            skillNextDamageFormula = $"{(int)(60f + (playerLevel + (skill04LV + 1)) * 0.5f * ((atkBasePoint + skillBuffPoints) * 0.5f + (magicBasePoint + skillBuffPoints) * 0.5f))}",
            NextMPConsume = $"{10f + ((skill04LV + 1) - 1) * 1f}",
            NextCoolDown = "1s",
        });

        //
        skillDict_Chinese.Add(SkillType.Skill01_E3, new SkillInfo
        {
            skillName = "��������",
            skillLevel = $"Lv.{skill01LV}",
            skillDesc = $@"����������100�� + (�ȼ� / 4)",
            skillDamageFormula = "last for 50 s",
            MPConsume = "100",
            CoolDown = "60s",
            skillNextDamageFormula = "last for 50 s",
            NextMPConsume = "100",
            NextCoolDown = "60s",
        });

        skillDict_Chinese.Add(SkillType.Skill02_E3, new SkillInfo
        {
            skillName = "���˶���",
            skillLevel = $"Lv.{skill02LV}",
            skillDesc = $@"(Boss��)��+���� Max:30",
            skillDamageFormula = $"{GetLevelBaseAttack() + 80f * (((atkBasePoint + magicBasePoint) * 1.2f + skillBuffPoints) + (skill02LV / 100f) * (GetLevelBaseAttack()))}",
            MPConsume = $"10(+{skill02LV}*({playerLevel})",
            CoolDown = "5s",
            skillNextDamageFormula = $"{GetLevelBaseAttack() + 80f * (((atkBasePoint + magicBasePoint) * 1.2f + skillBuffPoints) + ((skill02LV + 1) / 100f) * (GetLevelBaseAttack()))}",
            NextMPConsume = $"10(+{(skill02LV + 1)}*({playerLevel})",
            NextCoolDown = "5s",
        });

        skillDict_Chinese.Add(SkillType.Skill03_E3, new SkillInfo
        {
            skillName = "���˷籩",
            skillLevel = $"Lv.{skill03LV}",
            skillDesc = $@"������ħ���˺�(��Χ)  Max:40",
            skillDamageFormula = $"{(int)(80 + (playerLevel + skill03LV) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 0.8f)}~{(int)(80 + (playerLevel + skill03LV) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 1.5f)}",
            MPConsume = $"{20f + (skill03LV - 1) * 2f}",
            CoolDown = "3s",
            skillNextDamageFormula = $"{(int)(80 + (playerLevel + (skill03LV + 1)) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 0.8f)}~{(int)(80 + (playerLevel + (skill03LV + 1)) * 0.8 * ((atkBasePoint + skillBuffPoints) * 0.2 + (magicBasePoint + skillBuffPoints) * 0.8) * 1.5f)}",
            NextMPConsume = $"{20f + ((skill03LV + 1) - 1) * 2f}",
            NextCoolDown = "3s",
        });

        skillDict_Chinese.Add(SkillType.Skill04_E3, new SkillInfo
        {
            skillName = "���˲���",
            skillLevel = $"Lv.{skill04LV}",
            skillDesc = $@"����, ��+ħ��",
            skillDamageFormula = $"{(int)(60f + (playerLevel + skill04LV) * 0.5f * ((atkBasePoint + skillBuffPoints) * 0.5f + (magicBasePoint + skillBuffPoints) * 0.5f))}",
            MPConsume = $"{10f + (skill04LV - 1) * 1f}",
            CoolDown = "1s",
            skillNextDamageFormula = $"{(int)(60f + (playerLevel + (skill04LV + 1)) * 0.5f * ((atkBasePoint + skillBuffPoints) * 0.5f + (magicBasePoint + skillBuffPoints) * 0.5f))}",
            NextMPConsume = $"{10f + ((skill04LV + 1) - 1) * 1f}",
            NextCoolDown = "1s",
        });

    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("playerLevel", playerLevel);
        PlayerPrefs.SetString("playerName", playerName);
        PlayerPrefs.SetInt("playerExp", playerExp);

        PlayerPrefs.SetFloat("currentHP", currentHP);
        PlayerPrefs.SetFloat("maxHP", maxHP);

        PlayerPrefs.SetFloat("currentMP", currentMP);
        PlayerPrefs.SetFloat("maxMP", maxMP);

        PlayerPrefs.SetFloat("baseAttack", baseAttack);
        PlayerPrefs.SetFloat("atkBasePoint", atkBasePoint);
        PlayerPrefs.SetFloat("defBasePoint", defBasePoint);
        PlayerPrefs.SetFloat("conBasePoint", conBasePoint);
        PlayerPrefs.SetFloat("magicBasePoint", magicBasePoint);
        PlayerPrefs.SetInt("remaindBonusBasePoint", remaindBonusBasePoint);

        PlayerPrefs.SetInt("skill01LV", skill01LV);
        PlayerPrefs.SetInt("skill02LV", skill02LV);
        PlayerPrefs.SetInt("skill03LV", skill03LV);
        PlayerPrefs.SetInt("skill04LV", skill04LV);
        PlayerPrefs.SetInt("remaindBonusSkillPoint", remaindBonusSkillPoint);

        PlayerPrefs.SetInt("totalCoin", totalCoin);
        PlayerPrefs.SetInt("totalDiamond", totalDiamond);

        PlayerPrefs.SetFloat("playerLocationX", playerLocation.x);
        PlayerPrefs.SetFloat("playerLocationY", playerLocation.y);
        PlayerPrefs.SetFloat("playerLocationZ", playerLocation.z);


    }

    public void ClearData()
    {
        PlayerPrefs.DeleteKey("playerLevel");
        PlayerPrefs.DeleteKey("playerName");
        PlayerPrefs.DeleteKey("playerExp");

        PlayerPrefs.DeleteKey("currentHP");
        PlayerPrefs.DeleteKey("maxHP");

        PlayerPrefs.DeleteKey("currentMP");
        PlayerPrefs.DeleteKey("maxMP");

        PlayerPrefs.DeleteKey("baseAttack");
        PlayerPrefs.DeleteKey("atkBasePoint");
        PlayerPrefs.DeleteKey("defBasePoint");
        PlayerPrefs.DeleteKey("conBasePoint");
        PlayerPrefs.DeleteKey("magicBasePoint");
        PlayerPrefs.DeleteKey("remaindBonusBasePoint");

        PlayerPrefs.DeleteKey("skill01LV");
        PlayerPrefs.DeleteKey("skill02LV");
        PlayerPrefs.DeleteKey("skill03LV");
        PlayerPrefs.DeleteKey("skill04LV");
        PlayerPrefs.DeleteKey("remaindBonusSkillPoint");

        PlayerPrefs.DeleteKey("playerLocationX");
        PlayerPrefs.DeleteKey("playerLocationY");
        PlayerPrefs.DeleteKey("playerLocationZ");
    }

    public void SaveLocation()
    {
        PlayerPrefs.SetFloat("playerLocationX", playerLocation.x);
        PlayerPrefs.SetFloat("playerLocationY", playerLocation.y);
        PlayerPrefs.SetFloat("playerLocationZ", playerLocation.z);

    }

    public int GetLevelExpUp()
    {
        return playerLevel * (playerLevel << 1 + 100);
    }

    public float GetLevelMaxHP()
    {
        return 100f + playerLevel * (conBasePoint * 5);
    }

    public float GetLevelMaxMP()
    {
        return 100f + playerLevel * (magicBasePoint * 5);
    }

    public float GetLevelBaseAttack()
    {
        return 10f + playerLevel * 0.5f * (atkBasePoint + skillBuffPoints);
    }

    public float GetLevelBaseDef()
    {
        return playerLevel * ((defBasePoint+ skillBuffPoints) * 3);
    }

    public int GetLevelMaxBonusBasePoint(int level)
    {
        return level << 2;
    }

    public int GetLevelMaxBonusSkillPoint(int level)
    {
        return level << 1;
    }

    public void CalculateExp(int addExp)
    {
        playerExp += addExp;
        if(playerExp >= GetLevelExpUp())
        {
            playerLevel += 1;
            playerExp = 0;

            currentHP = GetLevelMaxHP();
            currentMP = GetLevelMaxMP();

            SaveData();
        }
    }

    public float GetSkillDamage(int skillid, float opponent_baseAtk = 0f)
    {
        switch (skillid)
        {
            case 1:
                if (skill01LV > 50)
                {
                    skillBuffPoints = 100 + skill01LV / 4;
                }
                else if (skill01LV > 20)
                {
                    skillBuffPoints = 50 + skill01LV / 10;
                }
                else if (skill01LV > 10)
                {
                    skillBuffPoints = 20;
                }
                else
                {
                    skillBuffPoints = 5;
                }
                return 0f;
            case 2:
                return GetLevelBaseAttack() + 80f * (((atkBasePoint + magicBasePoint) * 1.2f  + skillBuffPoints) + (skill02LV / 100f) * (GetLevelBaseAttack()));
            case 3:
                return 80f + (playerLevel + skill03LV) * 0.8f * ((atkBasePoint + skillBuffPoints) * 0.2f + (magicBasePoint + skillBuffPoints) * 0.8f) * Random.Range(0.8f, 1.5f);
            case 4:
                return 60f + (playerLevel + skill04LV) * 2f * ((atkBasePoint+ skillBuffPoints) * 2f + (magicBasePoint+ skillBuffPoints) * 0.5f);
        }

        return 0;
        
    }

    public float GetSkillUseMP(int skillid, int opponent_level = 0)
    {
        switch (skillid)
        {
            case 1:
                if(skill01LV > 50)
                {
                    return 100f;
                } 
                else if(skill01LV > 20)
                {
                    return 50f;
                } 
                else if (skill01LV > 10)
                {
                    return 20f;
                }
                return 5f;
            case 2:
                return 80f + skill02LV * (Mathf.Clamp(playerLevel, 0, 1000));
            case 3:
                return 20f + (skill03LV - 1) * 2f;
            case 4:
                return 10f + (skill04LV - 1) * 1f;
        }

        return 99999999f;
    }

    public int GetBufferSkillPoints()
    {
        if (skill01LV > 50)
        {
            return 100 + skill01LV / 4;
        }
        else if (skill01LV > 20)
        {
            return 20 + skill01LV / 5;
        }
        else if (skill01LV > 10)
        {
            return 10;
        }
        return 5;
    }

    public int GetMaxNumberOfWindSkillAttack()
    {
        if (skill03LV > 50)
        {
            return 40;
        }
        else if (skill03LV > 20)
        {
            return 20;
        }
        else if (skill03LV > 10)
        {
            return 8;
        }
        return 4;
    }

    public int GetMaxNumberOfPoisonSkillAttack()
    {
        if (skill02LV > 50)
        {
            return 30;
        }
        else if (skill02LV > 20)
        {
            return 15;
        }
        else if (skill02LV > 10)
        {
            return 6;
        }
        return 2;
    }

    public int GetSkillIndex(int skillid)
    {
        switch (skillid)
        {
            case 1:
                if (skill01LV > 50)
                {
                    return 3;
                }
                else if (skill01LV > 20)
                {
                    return 2;
                }
                else if (skill01LV > 10)
                {
                    return 1;
                }
                return 0;
            case 2:
                if (skill02LV > 50)
                {
                    return 3;
                }
                else if (skill02LV > 20)
                {
                    return 2;
                }
                else if (skill02LV > 10)
                {
                    return 1;
                }
                return 0;
            case 3:
                if (skill03LV > 50)
                {
                    return 3;
                }
                else if (skill03LV > 20)
                {
                    return 2;
                }
                else if (skill03LV > 10)
                {
                    return 1;
                }
                return 0;
            case 4:
                if (skill04LV > 50)
                {
                    return 3;
                }
                else if (skill04LV > 20)
                {
                    return 2;
                }
                else if (skill04LV > 10)
                {
                    return 1;
                }
                return 0;

        }
        return 0;
        
    }

}
