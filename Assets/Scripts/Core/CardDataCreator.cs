#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using MaskMYDrama.Cards;

namespace MaskMYDrama.Core
{
    /// <summary>
    /// Editor utility to create sample card data based on CSV specifications
    /// </summary>
    public class CardDataCreator : EditorWindow
    {
        [MenuItem("MaskMYDrama/Create Sample Cards")]
        public static void CreateSampleCards()
        {
            string path = "Assets/Data/Cards";
            
            // Create directory if it doesn't exist
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parentFolder = "Assets/Data";
                if (!AssetDatabase.IsValidFolder(parentFolder))
                {
                    AssetDatabase.CreateFolder("Assets", "Data");
                }
                AssetDatabase.CreateFolder("Assets/Data", "Cards");
            }
            
            // Attack Cards - Basic (0 energy)
            CreateCard("Attack_Basic", CardType.Attack, CardRarity.Basic, 0, 6, 0, "基础攻击", "造成6点伤害");
            
            // Attack Cards - Junior (1 energy)
            CreateCard("Attack_Junior", CardType.Attack, CardRarity.Junior, 1, 10, 0, "初级攻击", "造成10点伤害");
            
            // Attack Cards - Senior (2 energy)
            CreateCard("Attack_Senior", CardType.Attack, CardRarity.Senior, 2, 18, 0, "高级攻击", "造成18点伤害");
            
            // Defence Cards - Basic (0 energy)
            CreateCard("Defence_Basic", CardType.Defence, CardRarity.Basic, 0, 0, 5, "基础防御", "获得5点防御");
            
            // Defence Cards - Junior (1 energy)
            CreateCard("Defence_Junior", CardType.Defence, CardRarity.Junior, 1, 0, 8, "初级防御", "获得8点防御");
            
            // Defence Cards - Senior (2 energy)
            CreateCard("Defence_Senior", CardType.Defence, CardRarity.Senior, 2, 0, 12, "高级防御", "获得12点防御");
            
            // Strength Card
            CreateCard("Strength_Basic", CardType.Strength, CardRarity.Basic, 1, 0, 0, "力量", "本回合攻击力+2", strengthValue: 2);
            
            // Function Card Example
            CreateCard("Function_Draw", CardType.Function, CardRarity.Junior, 1, 0, 0, "抽牌", "抽一张牌", drawCard: true);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("Sample cards created in " + path);
        }
        
        private static void CreateCard(string name, CardType type, CardRarity rarity, int energy, int attack, int defence, string displayName, string description, int strengthValue = 0, bool drawCard = false)
        {
            Card card = ScriptableObject.CreateInstance<Card>();
            card.cardName = displayName;
            card.description = description;
            card.cardType = type;
            card.rarity = rarity;
            card.energyCost = energy;
            card.attackValue = attack;
            card.defenceValue = defence;
            card.strengthValue = strengthValue;
            card.drawCard = drawCard;
            
            string assetPath = $"Assets/Data/Cards/{name}.asset";
            AssetDatabase.CreateAsset(card, assetPath);
        }
    }
}
#endif

