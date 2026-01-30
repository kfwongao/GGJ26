#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using MaskMYDrama.Core;
using MaskMYDrama.Combat;
using MaskMYDrama.UI;

namespace MaskMYDrama.Editor
{
    /// <summary>
    /// Editor utility to automatically create all UI GameObjects for the combat scene.
    /// Run this from menu: MaskMYDrama > Setup Combat UI
    /// </summary>
    public class UISetupHelper : EditorWindow
    {
        [MenuItem("MaskMYDrama/Setup Combat UI")]
        public static void SetupCombatUI()
        {
            // Get or create the active scene
            if (EditorSceneManager.GetActiveScene().name == "Untitled")
            {
                EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            }

            // Create Canvas
            GameObject canvasObj = CreateCanvas();
            
            // Create EventSystem if it doesn't exist
            CreateEventSystem();
            
            // Create all UI elements
            GameObject combatUI = CreateCombatUI(canvasObj);
            CreateTopBar(canvasObj);
            CreateCombatArea(canvasObj);
            CreateCardHand(canvasObj);
            CreateBottomBar(canvasObj);
            
            // Create Card UI Prefab
            CreateCardUIPrefab();
            
            // Create Managers
            CreateManagers();
            
            // Link all references
            LinkReferences(combatUI);
            
            Debug.Log("Combat UI setup complete! Remember to assign card prefab and starting cards.");
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        
        private static GameObject CreateCanvas()
        {
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                canvas = new GameObject("Canvas");
                Canvas canvasComponent = canvas.AddComponent<Canvas>();
                canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.AddComponent<CanvasScaler>();
                canvas.AddComponent<GraphicRaycaster>();
                
                // Set Canvas Scaler
                CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;
            }
            return canvas;
        }
        
        private static void CreateEventSystem()
        {
            if (GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }
        
        private static GameObject CreateCombatUI(GameObject parent)
        {
            GameObject combatUI = new GameObject("CombatUI");
            combatUI.transform.SetParent(parent.transform, false);
            combatUI.AddComponent<CombatUI>();
            return combatUI;
        }
        
        private static void CreateTopBar(GameObject parent)
        {
            // Top Bar Container
            GameObject topBar = new GameObject("TopBar");
            topBar.transform.SetParent(parent.transform, false);
            RectTransform topBarRect = topBar.AddComponent<RectTransform>();
            topBarRect.anchorMin = new Vector2(0, 1);
            topBarRect.anchorMax = new Vector2(1, 1);
            topBarRect.pivot = new Vector2(0.5f, 1);
            topBarRect.anchoredPosition = Vector2.zero;
            topBarRect.sizeDelta = new Vector2(0, 80);
            
            Image topBarBg = topBar.AddComponent<Image>();
            topBarBg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            // Player Info (Left)
            GameObject playerInfo = new GameObject("PlayerInfo");
            playerInfo.transform.SetParent(topBar.transform, false);
            RectTransform playerInfoRect = playerInfo.AddComponent<RectTransform>();
            playerInfoRect.anchorMin = new Vector2(0, 0);
            playerInfoRect.anchorMax = new Vector2(0.3f, 1);
            playerInfoRect.offsetMin = Vector2.zero;
            playerInfoRect.offsetMax = Vector2.zero;
            
            // Player Name
            GameObject playerName = new GameObject("PlayerName");
            playerName.transform.SetParent(playerInfo.transform, false);
            RectTransform playerNameRect = playerName.AddComponent<RectTransform>();
            playerNameRect.anchorMin = new Vector2(0, 0.5f);
            playerNameRect.anchorMax = new Vector2(1, 1);
            playerNameRect.offsetMin = new Vector2(10, 0);
            playerNameRect.offsetMax = new Vector2(-10, -5);
            TextMeshProUGUI playerNameText = playerName.AddComponent<TextMeshProUGUI>();
            playerNameText.text = "Player";
            playerNameText.fontSize = 24;
            playerNameText.color = Color.white;
            playerName.name = "PlayerNameText";
            
            // Level Text
            GameObject level = new GameObject("LevelText");
            level.transform.SetParent(playerInfo.transform, false);
            RectTransform levelRect = level.AddComponent<RectTransform>();
            levelRect.anchorMin = new Vector2(0, 0);
            levelRect.anchorMax = new Vector2(1, 0.5f);
            levelRect.offsetMin = new Vector2(10, 5);
            levelRect.offsetMax = new Vector2(-10, 0);
            TextMeshProUGUI levelText = level.AddComponent<TextMeshProUGUI>();
            levelText.text = "Level 1";
            levelText.fontSize = 18;
            levelText.color = Color.white;
            level.name = "LevelText";
            
            // Health Bar (Right side of top bar)
            GameObject healthContainer = new GameObject("PlayerHealthBar");
            healthContainer.transform.SetParent(topBar.transform, false);
            RectTransform healthRect = healthContainer.AddComponent<RectTransform>();
            healthRect.anchorMin = new Vector2(0.7f, 0);
            healthRect.anchorMax = new Vector2(1, 1);
            healthRect.offsetMin = new Vector2(10, 10);
            healthRect.offsetMax = new Vector2(-10, -10);
            
            // Health Slider
            GameObject healthSlider = new GameObject("HealthSlider");
            healthSlider.transform.SetParent(healthContainer.transform, false);
            RectTransform sliderRect = healthSlider.AddComponent<RectTransform>();
            sliderRect.anchorMin = Vector2.zero;
            sliderRect.anchorMax = Vector2.one;
            sliderRect.offsetMin = Vector2.zero;
            sliderRect.offsetMax = Vector2.zero;
            Slider slider = healthSlider.AddComponent<Slider>();
            slider.minValue = 0;
            slider.maxValue = 100;
            slider.value = 100;
            
            // Health Slider Background
            GameObject sliderBg = new GameObject("Background");
            sliderBg.transform.SetParent(healthSlider.transform, false);
            RectTransform bgRect = sliderBg.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            Image bgImage = sliderBg.AddComponent<Image>();
            bgImage.color = new Color(0.3f, 0.3f, 0.3f);
            slider.targetGraphic = bgImage;
            
            // Health Slider Fill
            GameObject sliderFill = new GameObject("Fill Area");
            sliderFill.transform.SetParent(healthSlider.transform, false);
            RectTransform fillAreaRect = sliderFill.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = Vector2.zero;
            fillAreaRect.offsetMax = Vector2.zero;
            
            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(sliderFill.transform, false);
            RectTransform fillRect = fill.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = new Vector2(1, 1);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            Image fillImage = fill.AddComponent<Image>();
            fillImage.color = Color.red;
            slider.fillRect = fillRect;
            
            // Health Text
            GameObject healthText = new GameObject("HealthText");
            healthText.transform.SetParent(healthContainer.transform, false);
            RectTransform healthTextRect = healthText.AddComponent<RectTransform>();
            healthTextRect.anchorMin = Vector2.zero;
            healthTextRect.anchorMax = Vector2.one;
            healthTextRect.offsetMin = Vector2.zero;
            healthTextRect.offsetMax = Vector2.zero;
            TextMeshProUGUI healthTextComp = healthText.AddComponent<TextMeshProUGUI>();
            healthTextComp.text = "100/100";
            healthTextComp.fontSize = 20;
            healthTextComp.color = Color.white;
            healthTextComp.alignment = TextAlignmentOptions.Center;
            
            // Add HealthBar component
            HealthBar healthBar = healthContainer.AddComponent<HealthBar>();
            healthBar.healthSlider = slider;
            healthBar.healthText = healthTextComp;
        }
        
        private static void CreateCombatArea(GameObject parent)
        {
            // Combat Area Container
            GameObject combatArea = new GameObject("CombatArea");
            combatArea.transform.SetParent(parent.transform, false);
            RectTransform combatRect = combatArea.AddComponent<RectTransform>();
            combatRect.anchorMin = new Vector2(0, 0.2f);
            combatRect.anchorMax = new Vector2(1, 0.8f);
            combatRect.offsetMin = Vector2.zero;
            combatRect.offsetMax = Vector2.zero;
            
            // Player Area (Left)
            GameObject playerArea = new GameObject("PlayerArea");
            playerArea.transform.SetParent(combatArea.transform, false);
            RectTransform playerAreaRect = playerArea.AddComponent<RectTransform>();
            playerAreaRect.anchorMin = new Vector2(0, 0);
            playerAreaRect.anchorMax = new Vector2(0.5f, 1);
            playerAreaRect.offsetMin = Vector2.zero;
            playerAreaRect.offsetMax = Vector2.zero;
            
            // Player placeholder (will be replaced with art)
            GameObject playerObj = new GameObject("Player");
            playerObj.transform.SetParent(playerArea.transform, false);
            RectTransform playerRect = playerObj.AddComponent<RectTransform>();
            playerRect.anchorMin = new Vector2(0.5f, 0);
            playerRect.anchorMax = new Vector2(0.5f, 0);
            playerRect.pivot = new Vector2(0.5f, 0);
            playerRect.sizeDelta = new Vector2(200, 300);
            playerRect.anchoredPosition = new Vector2(0, 50);
            Image playerImage = playerObj.AddComponent<Image>();
            playerImage.color = new Color(0.5f, 0.5f, 1f, 0.5f);
            playerObj.AddComponent<Player>();
            
            // Enemy Area (Right)
            GameObject enemyArea = new GameObject("EnemyArea");
            enemyArea.transform.SetParent(combatArea.transform, false);
            RectTransform enemyAreaRect = enemyArea.AddComponent<RectTransform>();
            enemyAreaRect.anchorMin = new Vector2(0.5f, 0);
            enemyAreaRect.anchorMax = new Vector2(1, 1);
            enemyAreaRect.offsetMin = Vector2.zero;
            enemyAreaRect.offsetMax = Vector2.zero;
            
            // Enemy placeholder
            GameObject enemyObj = new GameObject("Enemy");
            enemyObj.transform.SetParent(enemyArea.transform, false);
            RectTransform enemyRect = enemyObj.AddComponent<RectTransform>();
            enemyRect.anchorMin = new Vector2(0.5f, 0);
            enemyRect.anchorMax = new Vector2(0.5f, 0);
            enemyRect.pivot = new Vector2(0.5f, 0);
            enemyRect.sizeDelta = new Vector2(200, 300);
            enemyRect.anchoredPosition = new Vector2(0, 50);
            Image enemyImage = enemyObj.AddComponent<Image>();
            enemyImage.color = new Color(1f, 0.5f, 0.5f, 0.5f);
            enemyObj.AddComponent<Enemy>();
            
            // Enemy Health Bar
            GameObject enemyHealthContainer = new GameObject("EnemyHealthBar");
            enemyHealthContainer.transform.SetParent(enemyArea.transform, false);
            RectTransform enemyHealthRect = enemyHealthContainer.AddComponent<RectTransform>();
            enemyHealthRect.anchorMin = new Vector2(0.5f, 0);
            enemyHealthRect.anchorMax = new Vector2(0.5f, 0);
            enemyHealthRect.pivot = new Vector2(0.5f, 0);
            enemyHealthRect.sizeDelta = new Vector2(300, 40);
            enemyHealthRect.anchoredPosition = new Vector2(0, 10);
            
            // Enemy Health Slider
            GameObject enemySlider = new GameObject("HealthSlider");
            enemySlider.transform.SetParent(enemyHealthContainer.transform, false);
            RectTransform enemySliderRect = enemySlider.AddComponent<RectTransform>();
            enemySliderRect.anchorMin = Vector2.zero;
            enemySliderRect.anchorMax = Vector2.one;
            enemySliderRect.offsetMin = Vector2.zero;
            enemySliderRect.offsetMax = Vector2.zero;
            Slider enemySliderComp = enemySlider.AddComponent<Slider>();
            enemySliderComp.minValue = 0;
            enemySliderComp.maxValue = 100;
            enemySliderComp.value = 100;
            
            // Enemy Slider Background
            GameObject enemyBg = new GameObject("Background");
            enemyBg.transform.SetParent(enemySlider.transform, false);
            RectTransform enemyBgRect = enemyBg.AddComponent<RectTransform>();
            enemyBgRect.anchorMin = Vector2.zero;
            enemyBgRect.anchorMax = Vector2.one;
            enemyBgRect.offsetMin = Vector2.zero;
            enemyBgRect.offsetMax = Vector2.zero;
            Image enemyBgImage = enemyBg.AddComponent<Image>();
            enemyBgImage.color = new Color(0.3f, 0.3f, 0.3f);
            enemySliderComp.targetGraphic = enemyBgImage;
            
            // Enemy Slider Fill
            GameObject enemyFillArea = new GameObject("Fill Area");
            enemyFillArea.transform.SetParent(enemySlider.transform, false);
            RectTransform enemyFillAreaRect = enemyFillArea.AddComponent<RectTransform>();
            enemyFillAreaRect.anchorMin = Vector2.zero;
            enemyFillAreaRect.anchorMax = Vector2.one;
            enemyFillAreaRect.offsetMin = Vector2.zero;
            enemyFillAreaRect.offsetMax = Vector2.zero;
            
            GameObject enemyFill = new GameObject("Fill");
            enemyFill.transform.SetParent(enemyFillArea.transform, false);
            RectTransform enemyFillRect = enemyFill.AddComponent<RectTransform>();
            enemyFillRect.anchorMin = Vector2.zero;
            enemyFillRect.anchorMax = new Vector2(1, 1);
            enemyFillRect.offsetMin = Vector2.zero;
            enemyFillRect.offsetMax = Vector2.zero;
            Image enemyFillImage = enemyFill.AddComponent<Image>();
            enemyFillImage.color = Color.red;
            enemySliderComp.fillRect = enemyFillRect;
            
            // Enemy Health Text
            GameObject enemyHealthText = new GameObject("HealthText");
            enemyHealthText.transform.SetParent(enemyHealthContainer.transform, false);
            RectTransform enemyHealthTextRect = enemyHealthText.AddComponent<RectTransform>();
            enemyHealthTextRect.anchorMin = Vector2.zero;
            enemyHealthTextRect.anchorMax = Vector2.one;
            enemyHealthTextRect.offsetMin = Vector2.zero;
            enemyHealthTextRect.offsetMax = Vector2.zero;
            TextMeshProUGUI enemyHealthTextComp = enemyHealthText.AddComponent<TextMeshProUGUI>();
            enemyHealthTextComp.text = "100/100";
            enemyHealthTextComp.fontSize = 18;
            enemyHealthTextComp.color = Color.white;
            enemyHealthTextComp.alignment = TextAlignmentOptions.Center;
            
            // Add HealthBar component
            HealthBar enemyHealthBar = enemyHealthContainer.AddComponent<HealthBar>();
            enemyHealthBar.healthSlider = enemySliderComp;
            enemyHealthBar.healthText = enemyHealthTextComp;
        }
        
        private static void CreateCardHand(GameObject parent)
        {
            // Card Hand Container
            GameObject cardHand = new GameObject("CardHand");
            cardHand.transform.SetParent(parent.transform, false);
            RectTransform cardHandRect = cardHand.AddComponent<RectTransform>();
            cardHandRect.anchorMin = new Vector2(0, 0);
            cardHandRect.anchorMax = new Vector2(1, 0.2f);
            cardHandRect.offsetMin = new Vector2(0, 0);
            cardHandRect.offsetMax = new Vector2(0, 0);
            
            // Horizontal Layout Group
            HorizontalLayoutGroup layout = cardHand.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 20;
            layout.padding = new RectOffset(20, 20, 20, 20);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            
            ContentSizeFitter fitter = cardHand.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        
        private static void CreateBottomBar(GameObject parent)
        {
            // Bottom Bar Container
            GameObject bottomBar = new GameObject("BottomBar");
            bottomBar.transform.SetParent(parent.transform, false);
            RectTransform bottomBarRect = bottomBar.AddComponent<RectTransform>();
            bottomBarRect.anchorMin = new Vector2(0, 0);
            bottomBarRect.anchorMax = new Vector2(1, 0.2f);
            bottomBarRect.offsetMin = Vector2.zero;
            bottomBarRect.offsetMax = Vector2.zero;
            
            Image bottomBarBg = bottomBar.AddComponent<Image>();
            bottomBarBg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            // Energy Display (Left)
            GameObject energyContainer = new GameObject("EnergyDisplay");
            energyContainer.transform.SetParent(bottomBar.transform, false);
            RectTransform energyRect = energyContainer.AddComponent<RectTransform>();
            energyRect.anchorMin = new Vector2(0, 0);
            energyRect.anchorMax = new Vector2(0.2f, 1);
            energyRect.offsetMin = new Vector2(20, 10);
            energyRect.offsetMax = new Vector2(-10, -10);
            
            GameObject energyText = new GameObject("EnergyText");
            energyText.transform.SetParent(energyContainer.transform, false);
            RectTransform energyTextRect = energyText.AddComponent<RectTransform>();
            energyTextRect.anchorMin = Vector2.zero;
            energyTextRect.anchorMax = Vector2.one;
            energyTextRect.offsetMin = Vector2.zero;
            energyTextRect.offsetMax = Vector2.zero;
            TextMeshProUGUI energyTextComp = energyText.AddComponent<TextMeshProUGUI>();
            energyTextComp.text = "3/3";
            energyTextComp.fontSize = 32;
            energyTextComp.color = Color.cyan;
            energyTextComp.alignment = TextAlignmentOptions.Center;
            
            EnergyDisplay energyDisplay = energyContainer.AddComponent<EnergyDisplay>();
            energyDisplay.energyText = energyTextComp;
            
            // Deck Counts (Left Center)
            GameObject deckCounts = new GameObject("DeckCounts");
            deckCounts.transform.SetParent(bottomBar.transform, false);
            RectTransform deckCountsRect = deckCounts.AddComponent<RectTransform>();
            deckCountsRect.anchorMin = new Vector2(0.2f, 0);
            deckCountsRect.anchorMax = new Vector2(0.4f, 1);
            deckCountsRect.offsetMin = new Vector2(10, 10);
            deckCountsRect.offsetMax = new Vector2(-10, -10);
            
            VerticalLayoutGroup deckLayout = deckCounts.AddComponent<VerticalLayoutGroup>();
            deckLayout.spacing = 5;
            deckLayout.childControlWidth = true;
            deckLayout.childControlHeight = false;
            
            // Pool Count
            GameObject poolCount = new GameObject("PoolCount");
            poolCount.transform.SetParent(deckCounts.transform, false);
            TextMeshProUGUI poolCountText = poolCount.AddComponent<TextMeshProUGUI>();
            poolCountText.text = "Pool: 12";
            poolCountText.fontSize = 18;
            poolCountText.color = Color.white;
            
            // Abandoned Count
            GameObject abandonedCount = new GameObject("AbandonedCount");
            abandonedCount.transform.SetParent(deckCounts.transform, false);
            TextMeshProUGUI abandonedCountText = abandonedCount.AddComponent<TextMeshProUGUI>();
            abandonedCountText.text = "Abandoned: 0";
            abandonedCountText.fontSize = 18;
            abandonedCountText.color = Color.white;
            
            // End Turn Button (Right)
            GameObject endTurnButton = new GameObject("EndTurnButton");
            endTurnButton.transform.SetParent(bottomBar.transform, false);
            RectTransform buttonRect = endTurnButton.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.8f, 0);
            buttonRect.anchorMax = new Vector2(1, 1);
            buttonRect.offsetMin = new Vector2(10, 10);
            buttonRect.offsetMax = new Vector2(-20, -10);
            
            Image buttonImage = endTurnButton.AddComponent<Image>();
            buttonImage.color = new Color(0.3f, 0.6f, 0.3f);
            
            Button button = endTurnButton.AddComponent<Button>();
            button.targetGraphic = buttonImage;
            
            // Button Text
            GameObject buttonText = new GameObject("Text");
            buttonText.transform.SetParent(endTurnButton.transform, false);
            RectTransform buttonTextRect = buttonText.AddComponent<RectTransform>();
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.offsetMin = Vector2.zero;
            buttonTextRect.offsetMax = Vector2.zero;
            TextMeshProUGUI buttonTextComp = buttonText.AddComponent<TextMeshProUGUI>();
            buttonTextComp.text = "End Turn";
            buttonTextComp.fontSize = 24;
            buttonTextComp.color = Color.white;
            buttonTextComp.alignment = TextAlignmentOptions.Center;
        }
        
        private static void CreateCardUIPrefab()
        {
            // Create Card UI Prefab
            GameObject cardPrefab = new GameObject("CardUI");
            RectTransform cardRect = cardPrefab.AddComponent<RectTransform>();
            cardRect.sizeDelta = new Vector2(200, 280);
            
            // Card Background
            Image cardBg = cardPrefab.AddComponent<Image>();
            cardBg.color = new Color(0.9f, 0.9f, 0.9f);
            
            // Card Name
            GameObject cardName = new GameObject("CardName");
            cardName.transform.SetParent(cardPrefab.transform, false);
            RectTransform nameRect = cardName.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0.8f);
            nameRect.anchorMax = new Vector2(1, 1);
            nameRect.offsetMin = new Vector2(10, 0);
            nameRect.offsetMax = new Vector2(-10, -5);
            TextMeshProUGUI nameText = cardName.AddComponent<TextMeshProUGUI>();
            nameText.text = "Card Name";
            nameText.fontSize = 20;
            nameText.color = Color.black;
            nameText.fontStyle = FontStyles.Bold;
            
            // Card Description
            GameObject cardDesc = new GameObject("CardDescription");
            cardDesc.transform.SetParent(cardPrefab.transform, false);
            RectTransform descRect = cardDesc.AddComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0, 0.3f);
            descRect.anchorMax = new Vector2(1, 0.8f);
            descRect.offsetMin = new Vector2(10, 5);
            descRect.offsetMax = new Vector2(-10, -5);
            TextMeshProUGUI descText = cardDesc.AddComponent<TextMeshProUGUI>();
            descText.text = "Card Description";
            descText.fontSize = 14;
            descText.color = Color.black;
            descText.alignment = TextAlignmentOptions.TopLeft;
            
            // Energy Cost (Top Right)
            GameObject energyCost = new GameObject("EnergyCost");
            energyCost.transform.SetParent(cardPrefab.transform, false);
            RectTransform energyRect = energyCost.AddComponent<RectTransform>();
            energyRect.anchorMin = new Vector2(0.8f, 0.8f);
            energyRect.anchorMax = new Vector2(1, 1);
            energyRect.offsetMin = Vector2.zero;
            energyRect.offsetMax = Vector2.zero;
            TextMeshProUGUI energyText = energyCost.AddComponent<TextMeshProUGUI>();
            energyText.text = "1";
            energyText.fontSize = 24;
            energyText.color = Color.yellow;
            energyText.alignment = TextAlignmentOptions.Center;
            energyText.fontStyle = FontStyles.Bold;
            
            // Add CardUI component
            CardUI cardUI = cardPrefab.AddComponent<CardUI>();
            cardUI.cardNameText = nameText;
            cardUI.descriptionText = descText;
            cardUI.energyCostText = energyText;
            cardUI.cardBackground = cardBg;
            
            // Save as prefab
            string prefabPath = "Assets/Prefabs/CardUIPrefab.prefab";
            string prefabDir = System.IO.Path.GetDirectoryName(prefabPath);
            if (!System.IO.Directory.Exists(prefabDir))
            {
                System.IO.Directory.CreateDirectory(prefabDir);
            }
            
            PrefabUtility.SaveAsPrefabAsset(cardPrefab, prefabPath);
            DestroyImmediate(cardPrefab);
            
            Debug.Log($"Card UI Prefab created at {prefabPath}");
        }
        
        private static void CreateManagers()
        {
            // GameManager
            GameObject gameManager = GameObject.Find("GameManager");
            if (gameManager == null)
            {
                gameManager = new GameObject("GameManager");
                gameManager.AddComponent<GameManager>();
            }
            
            // CombatManager
            GameObject combatManager = GameObject.Find("CombatManager");
            if (combatManager == null)
            {
                combatManager = new GameObject("CombatManager");
                combatManager.AddComponent<CombatManager>();
                combatManager.AddComponent<DeckManager>();
            }
        }
        
        private static void LinkReferences(GameObject combatUI)
        {
            CombatUI combatUIComp = combatUI.GetComponent<CombatUI>();
            if (combatUIComp == null) return;
            
            // Find managers
            combatUIComp.combatManager = GameObject.FindObjectOfType<CombatManager>();
            combatUIComp.deckManager = GameObject.FindObjectOfType<DeckManager>();
            combatUIComp.player = GameObject.FindObjectOfType<Player>();
            combatUIComp.enemy = GameObject.FindObjectOfType<Enemy>();
            
            // Find UI elements
            combatUIComp.playerHealthBar = GameObject.Find("TopBar/PlayerHealthBar").GetComponent<HealthBar>();
            combatUIComp.enemyHealthBar = GameObject.Find("CombatArea/EnemyArea/EnemyHealthBar").GetComponent<HealthBar>();
            combatUIComp.energyDisplay = GameObject.Find("BottomBar/EnergyDisplay").GetComponent<EnergyDisplay>();
            combatUIComp.cardHandParent = GameObject.Find("CardHand").transform;
            combatUIComp.endTurnButton = GameObject.Find("BottomBar/EndTurnButton").GetComponent<Button>();
            
            // Find text elements
            TextMeshProUGUI[] texts = GameObject.FindObjectsOfType<TextMeshProUGUI>();
            foreach (var text in texts)
            {
                if (text.name == "PoolCount")
                    combatUIComp.poolCountText = text;
                else if (text.name == "AbandonedCount")
                    combatUIComp.abandonedCountText = text;
                else if (text.name == "PlayerNameText")
                    combatUIComp.playerNameText = text;
                else if (text.name == "LevelText")
                    combatUIComp.levelText = text;
            }
            
            // Load card prefab
            GameObject cardPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/CardUIPrefab.prefab");
            if (cardPrefab != null)
            {
                combatUIComp.cardUIPrefab = cardPrefab;
            }
            
            // Link CombatManager references
            if (combatUIComp.combatManager != null)
            {
                combatUIComp.combatManager.player = combatUIComp.player;
                combatUIComp.combatManager.enemy = combatUIComp.enemy;
                combatUIComp.combatManager.deckManager = combatUIComp.deckManager;
            }
        }
    }
}
#endif

