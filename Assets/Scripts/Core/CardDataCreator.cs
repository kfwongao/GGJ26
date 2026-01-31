#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using MaskMYDrama.Cards;
using System.Collections.Generic;

namespace MaskMYDrama.Core
{
    /// <summary>
    /// Editor utility to create all card data based on MaskMYDrama - Art Design Info.csv
    /// </summary>
    public class CardDataCreator : EditorWindow
    {
        [MenuItem("MaskMYDrama/Create All Cards from CSV")]
        public static void CreateAllCardsFromCSV()
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
            
            List<Card> allCards = new List<Card>();
            List<Card> startingCards = new List<Card>();
            List<Card> roguelikeCards = new List<Card>();
            
            // ========== ATTACK CARDS ==========
            
            // Attack 0级 - 亮相/Stage pose (开局, 1能量, 5伤害)
            Card stagePose = CreateCard("Attack_StagePose", CardType.Attack, CardRarity.Basic, 1, 5, 0, 
                "亮相", "受到打击的一名对手将失去5点生命。", 
                poolType: CardPoolType.StartingPool);
            allCards.Add(stagePose);
            startingCards.Add(stagePose);
            
            // Attack 1级 - Monologue 独白 (开局, 1能量, 7伤害, 抽1张牌)
            Card monologue = CreateCard("Attack_Monologue", CardType.Attack, CardRarity.Junior, 1, 7, 0, 
                "独白", "受到打击的一名对手将失去7点生命，抽一张牌", 
                drawCardCount: 1, poolType: CardPoolType.StartingPool);
            allCards.Add(monologue);
            startingCards.Add(monologue);
            
            // Attack 1级 - Dialogue 对白 (开局, 0能量, 4伤害所有敌人)
            Card dialogue = CreateCard("Attack_Dialogue", CardType.Attack, CardRarity.Junior, 0, 4, 0, 
                "对白", "所有对手失去4点生命。", 
                poolType: CardPoolType.StartingPool);
            allCards.Add(dialogue);
            startingCards.Add(dialogue);
            
            // Attack 2级 - 即兴表演/Improvisation (Roguelike, X*4伤害, X=剩余能量)
            Card improvisation = CreateCard("Attack_Improvisation", CardType.Attack, CardRarity.Senior, 0, 0, 0, 
                "即兴表演", "受到打击的一名对手失去X*4点生命（X=剩余全部天赋点）", 
                poolType: CardPoolType.RoguelikePool);
            allCards.Add(improvisation);
            roguelikeCards.Add(improvisation);
            
            // Attack 2级 - 高潮/Climax (Roguelike, 2能量, 20伤害, 消耗所有力量牌)
            Card climax = CreateCard("Attack_Climax", CardType.Attack, CardRarity.Senior, 2, 20, 0, 
                "高潮", "随机令一名敌人失去20点生命 - 消耗手牌中所有力量牌。", 
                poolType: CardPoolType.RoguelikePool);
            allCards.Add(climax);
            roguelikeCards.Add(climax);
            
            // Attack 2级 - 主角/Protagonist (Roguelike, 3能量, 抽3张牌, 延迟反射伤害)
            Card protagonist = CreateCard("Attack_Protagonist", CardType.Attack, CardRarity.Senior, 3, 0, 0, 
                "主角", "抽3张牌，下一轮回合对方失去对你造成的同等伤害。", 
                drawCardCount: 3, poolType: CardPoolType.RoguelikePool);
            allCards.Add(protagonist);
            roguelikeCards.Add(protagonist);
            
            // Attack 2级 - 多才多艺/Versatile (Roguelike, 2能量, 9伤害, 从弃牌堆返回一张牌, 费用变0)
            Card versatile = CreateCard("Attack_Versatile", CardType.Attack, CardRarity.Senior, 2, 9, 0, 
                "多才多艺", "受到打击的对手失去9点生命，将弃牌堆的一张牌放回手牌，此张牌的天赋点变为0", 
                poolType: CardPoolType.RoguelikePool);
            allCards.Add(versatile);
            roguelikeCards.Add(versatile);
            
            // ========== DEFENCE CARDS ==========
            
            // Defence 0级 - 屏障/Block (开局, 1能量, 4防御)
            Card block = CreateCard("Defence_Block", CardType.Defence, CardRarity.Basic, 1, 0, 4, 
                "屏障", "获得4点屏障保护。", 
                poolType: CardPoolType.StartingPool);
            allCards.Add(block);
            startingCards.Add(block);
            
            // Defence 1级 - 暗场/Fade out (开局, 1能量, 6防御, 敌人20%增伤, 抽1张牌)
            Card fadeOut = CreateCard("Defence_FadeOut", CardType.Defence, CardRarity.Junior, 1, 0, 6, 
                "暗场", "获得6点屏障保护，指定的一名敌人获得20%增伤，抽一张牌。", 
                drawCardCount: 1, poolType: CardPoolType.StartingPool);
            allCards.Add(fadeOut);
            startingCards.Add(fadeOut);
            
            // Defence 1级 - 停顿/Stop (开局, 2能量, 8防御, 随机弃牌)
            Card stop = CreateCard("Defence_Stop", CardType.Defence, CardRarity.Junior, 2, 0, 8, 
                "停顿", "获得8点屏障，随机消耗一张手牌", 
                poolType: CardPoolType.StartingPool);
            allCards.Add(stop);
            startingCards.Add(stop);
            
            // Defence 2级 - 横穿舞台/Cross (Roguelike, 0能量, 延迟格挡)
            Card cross = CreateCard("Defence_Cross", CardType.Defence, CardRarity.Senior, 0, 0, 0, 
                "横穿舞台", "本轮结束后，下一轮你将获得和对敌人伤害同等数值的格挡", 
                poolType: CardPoolType.RoguelikePool);
            allCards.Add(cross);
            roguelikeCards.Add(cross);
            
            // ========== STRENGTH CARDS ==========
            
            // Strength 0级 - 潜台词Subtext (开局, 0能量, 添加2张攻击牌到手牌)
            Card subtext = CreateCard("Strength_Subtext", CardType.Strength, CardRarity.Basic, 0, 0, 0, 
                "潜台词", "将随机2张攻击牌放入你的手牌", 
                poolType: CardPoolType.StartingPool);
            allCards.Add(subtext);
            startingCards.Add(subtext);
            
            // Strength 1级 - 情感记忆/Emotional Memory (开局, 0能量, 将弃牌堆所有牌放入卡池并洗牌, 抽2张牌)
            Card emotionalMemory = CreateCard("Strength_EmotionalMemory", CardType.Strength, CardRarity.Junior, 0, 0, 0, 
                "情感记忆", "将弃牌堆所有牌放入卡池并洗牌，抽2张牌", 
                drawCardCount: 2, poolType: CardPoolType.StartingPool);
            allCards.Add(emotionalMemory);
            startingCards.Add(emotionalMemory);
            
            // Strength 1级 - 讽刺/Irony (开局, 0能量, 敌人30%增伤, 敌人减伤50%)
            Card irony = CreateCard("Strength_Irony", CardType.Strength, CardRarity.Junior, 0, 0, 0, 
                "讽刺", "你向所有敌人施加了30%增伤，并使敌人对你本轮减伤50%", 
                poolType: CardPoolType.StartingPool);
            allCards.Add(irony);
            startingCards.Add(irony);
            
            // Strength 2级 - 提示词/Cue (Roguelike, 2能量, 获得本轮所有伤害同等值的屏障, 恢复15点生命)
            Card cue = CreateCard("Strength_Cue", CardType.Strength, CardRarity.Senior, 2, 0, 0, 
                "提示词", "获得本轮所有伤害同等值的屏障，并恢复15点生命", 
                poolType: CardPoolType.RoguelikePool);
            allCards.Add(cue);
            roguelikeCards.Add(cue);
            
            // ========== FUNCTION CARDS ==========
            
            // Function - 笑场/Corpsing (Roguelike, 0能量, 对所有敌人造成13点伤害, 一次性使用)
            Card corpsing = CreateCard("Function_Corpsing", CardType.Function, CardRarity.Basic, 0, 13, 0, 
                "笑场", "对所有敌人造成13点伤害，一次性使用。", 
                isExhaust: true, poolType: CardPoolType.RoguelikePool);
            allCards.Add(corpsing);
            roguelikeCards.Add(corpsing);
            
            // Function - 抢戏/Upstaging (Roguelike, 3能量, 抽三张牌, 每使用一次费用-1)
            Card upstaging = CreateCard("Function_Upstaging", CardType.Function, CardRarity.Junior, 3, 0, 0, 
                "抢戏", "抽三张牌。你每使用一次这张牌，需消耗的天赋点-1。", 
                drawCardCount: 3, poolType: CardPoolType.RoguelikePool);
            allCards.Add(upstaging);
            roguelikeCards.Add(upstaging);
            
            // Function - 魅惑 (Roguelike, 1能量, 本轮中对方将等值伤害随机攻击同伴或自己)
            Card charm = CreateCard("Function_Charm", CardType.Function, CardRarity.Junior, 1, 0, 0, 
                "魅惑", "本轮中对方将等值伤害随机攻击同伴或自己。", 
                poolType: CardPoolType.RoguelikePool);
            allCards.Add(charm);
            roguelikeCards.Add(charm);
            
            // Function - 煽情 (Roguelike, 2能量, 抽三张牌, 这三张牌的费用变为0, 消耗所有手牌)
            Card emotional = CreateCard("Function_Emotional", CardType.Function, CardRarity.Junior, 2, 0, 0, 
                "煽情", "抽三张牌，这三张牌的天赋点变为0.消耗所有手牌。", 
                drawCardCount: 3, poolType: CardPoolType.RoguelikePool);
            allCards.Add(emotional);
            roguelikeCards.Add(emotional);
            
            // Function - 假戏真做 (Roguelike, 2能量, 造成卡池中剩余卡牌数量2倍的伤害, 获得4点屏障)
            Card realAct = CreateCard("Function_RealAct", CardType.Function, CardRarity.Junior, 2, 0, 4, 
                "假戏真做", "造成你卡池中剩余卡牌数量2倍的伤害，获得4点屏障", 
                poolType: CardPoolType.RoguelikePool);
            allCards.Add(realAct);
            roguelikeCards.Add(realAct);
            
            // Function - 剧透/Spoiled Alert (Roguelike, 3能量, 损失5点生命, 打出剩余生命值的伤害给生命最少的敌人, 获得12点屏障)
            Card spoiledAlert = CreateCard("Function_SpoiledAlert", CardType.Function, CardRarity.Senior, 3, 0, 12, 
                "剧透", "损失5点生命，打出剩余生命值的伤害给生命最少的敌人，获得12点屏障。", 
                poolType: CardPoolType.RoguelikePool);
            allCards.Add(spoiledAlert);
            roguelikeCards.Add(spoiledAlert);
            
            // Function - 反派/Antagonist (Roguelike, 2能量, 获得最大35生命, 本轮减伤30%, 消耗手牌中所有功能牌)
            Card antagonist = CreateCard("Function_Antagonist", CardType.Function, CardRarity.Senior, 2, 0, 0, 
                "反派", "获得最大35生命，本轮减伤30%。消耗手牌中所有功能牌。", 
                poolType: CardPoolType.RoguelikePool);
            allCards.Add(antagonist);
            roguelikeCards.Add(antagonist);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"Created {allCards.Count} cards total:");
            Debug.Log($"- {startingCards.Count} starting cards (开局 in card pool)");
            Debug.Log($"- {roguelikeCards.Count} roguelike pool cards");
            Debug.Log($"Cards saved in {path}");
            
            // Create CardDatabase
            CreateCardDatabase(startingCards, roguelikeCards, allCards);
        }
        
        private static Card CreateCard(string name, CardType type, CardRarity rarity, int energy, int attack, int defence, 
            string displayName, string description, int strengthValue = 0, int drawCardCount = 0, bool isExhaust = false, 
            CardPoolType poolType = CardPoolType.StartingPool)
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
            card.drawCardCount = drawCardCount;
            card.isExhaust = isExhaust;
            card.poolType = poolType;
            
            string assetPath = $"Assets/Data/Cards/{name}.asset";
            AssetDatabase.CreateAsset(card, assetPath);
            
            return card;
        }
        
        private static void CreateCardDatabase(List<Card> startingCards, List<Card> roguelikeCards, List<Card> allCards)
        {
            // Check if CardDatabase already exists
            string[] existingDBs = AssetDatabase.FindAssets("t:CardDatabase");
            CardDatabase cardDatabase = null;
            
            if (existingDBs.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(existingDBs[0]);
                cardDatabase = AssetDatabase.LoadAssetAtPath<CardDatabase>(path);
                Debug.Log($"Found existing CardDatabase at {path}, updating it...");
            }
            else
            {
                // Create new CardDatabase
                cardDatabase = ScriptableObject.CreateInstance<CardDatabase>();
                string dbPath = "Assets/Data/CardDatabase.asset";
                
                // Create Data folder if needed
                if (!AssetDatabase.IsValidFolder("Assets/Data"))
                {
                    AssetDatabase.CreateFolder("Assets", "Data");
                }
                
                AssetDatabase.CreateAsset(cardDatabase, dbPath);
                Debug.Log($"Created new CardDatabase at {dbPath}");
            }
            
            // Assign cards to database
            cardDatabase.startingCards = startingCards.ToArray();
            cardDatabase.roguelikePoolCards = roguelikeCards.ToArray();
            cardDatabase.allCards = allCards.ToArray();
            
            EditorUtility.SetDirty(cardDatabase);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("CardDatabase created/updated successfully!");
            Debug.Log($"- Starting Cards: {cardDatabase.startingCards.Length}");
            Debug.Log($"- Roguelike Pool Cards: {cardDatabase.roguelikePoolCards.Length}");
            Debug.Log($"- All Cards: {cardDatabase.allCards.Length}");
        }
    }
}
#endif

